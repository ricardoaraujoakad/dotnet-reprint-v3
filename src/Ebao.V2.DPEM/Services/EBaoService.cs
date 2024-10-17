using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Ebao.V2.DPEM.Api.Quotation.Responses;
using Ebao.V2.DPEM.Exceptions;
using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Models.ViewModels.Auth.Requests;
using Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PhoneNumbers;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Ebao.V2.DPEM.Services;

public class EBaoService : ServiceBase<EBaoService>
{
    private readonly AuthService _authService;
    private readonly QuotationService _quotationService;
    private readonly PaymentService _paymentService;
    private static readonly int[] _validDDDs = [   11, 12, 13, 14, 15, 16, 17, 18, 19, 21, 22, 24, 27, 28, 31, 32, 33, 34, 35, 37, 38, 41, 42, 43, 44, 45, 46, 47, 48, 49,
        51, 53, 54, 55, 61, 62, 63, 64, 65, 66, 67, 68, 69, 71, 73, 74, 75, 77, 79, 81, 82, 83, 84, 85, 86, 87, 88, 89, 91, 92,
        93, 94, 95, 96, 97, 98, 99 ];

    public EBaoService(ILogger<EBaoService> logger, IMemoryCache cache, RequestInfo requestInfo,
        AuthService authService, QuotationService quotationService, PaymentService paymentService) : base(logger, cache,
        requestInfo)
    {
        _authService = authService;
        _quotationService = quotationService;
        _paymentService = paymentService;
    }

    public async ValueTask LogoutAsync()
    {
        await _authService.LogoutAsync();
    }

    public async ValueTask<(string Certificate, string OrderId)> SendToEbaoAsync(string xml)
    {
        var xmlParser = new XmlSerializer(typeof(Transactions));
        var reader = new StringReader(xml);
        var transactionXml = (Transactions)xmlParser.Deserialize(reader);
        var transaction = transactionXml.TransactionList.First();
        var productId = transaction.ACORD.InsuranceSvcRs.ErrorsAndOmissionsPolicyQuoteInqRs
            .ErrorsAndOmissionsLineBusiness
            .CompanyProductId;

        if (productId != Config.ProductId)
            throw new InvalidProductException($"O produto {productId} não é suportado por este bot.");

        Validate(transaction);

        var authRequest = new AuthRequest()
        {
            Username = Config.IntegrationConfig.EbaoLogin,
            Password = Config.IntegrationConfig.EbaoPassword
        };

        await _authService.LoginAsync(authRequest);

        string certificate;

        try
        {
            var policyRequest = CreatePolicyRequest(transaction);

            var quotation = await _quotationService.CreateQuotationAsync(policyRequest);
            var payment = await _paymentService.SetQuotationAsPaidAsync(quotation.Id.ToString());
            certificate = await _quotationService.IssueQuotationAsync(payment, policyRequest);
        }
        catch (DuplicateCertificateException duplicateCertificateException)
        {
            Logger.LogWarning("Já existe uma apólice ({Certificate}) emitida para este certificado. Será reutilizada.",
                duplicateCertificateException.Certificate);

            certificate = duplicateCertificateException.Certificate;
        }

        return (certificate, transaction.TransactionId);
    }

    public void Validate(Transaction transaction)
    {
        var errosAndOmission = transaction.ACORD.InsuranceSvcRs.ErrorsAndOmissionsPolicyQuoteInqRs;

        ValidateCertificate(transaction.PolicyId);
        ValidateEffectiveDate(errosAndOmission.CommlPolicy.ContractTerm.EffectiveDt);
        ValidateExpiryDate(errosAndOmission.CommlPolicy.ContractTerm.ExpirationDt);
        ValidateProtocolDate(errosAndOmission.ErrorsAndOmissionsLineBusiness.Protocol);
        ValidateLmi(errosAndOmission.ErrorsAndOmissionsLineBusiness.CommlCoverages
            .MaxBy(x => x.Limit.FormatCurrencyAmt.Amt)!.Limit.FormatCurrencyAmt.Amt);
        ValidateExpectedPremium(transaction.ACORD.AccountingSvcRs.BillingNotifyRs.BillingPolicyInfo.PaymentOption
            .InstallmentInfo.InstallmentAmt.Amt);
        ValidateVessel(errosAndOmission.CommlPolicy.VesselInformation);
        ValidateInsured(errosAndOmission.InsuredOrPrincipal);
        ValidateCoverages(errosAndOmission.ErrorsAndOmissionsLineBusiness.CommlCoverages.ToList());
        ValidatePaymentDate(transaction.ACORD.AccountingSvcRs.BillingNotifyRs.BillingPolicyInfo.PaymentOption
            .InstallmentInfo.InstallmentDueDt);
    }

    private PolicyRequest CreatePolicyRequest(Transaction transaction)
    {
        var errosAndOmission = transaction.ACORD.InsuranceSvcRs.ErrorsAndOmissionsPolicyQuoteInqRs;
        var vesselInfo = errosAndOmission.CommlPolicy.VesselInformation;
        var insuredOrPrincipal = errosAndOmission.InsuredOrPrincipal.GeneralPartyInfo;
        var coverages = errosAndOmission.ErrorsAndOmissionsLineBusiness.CommlCoverages;

        var effectiveDate = errosAndOmission.CommlPolicy.ContractTerm.EffectiveDt.Date.AddDays(1).AddSeconds(-60);

        return new PolicyRequest
        {
            Certificate = transaction.PolicyId,
            EffectiveDate = effectiveDate,
            ExpiryDate = errosAndOmission.CommlPolicy.ContractTerm.ExpirationDt,
            ProtocolDate = errosAndOmission.ErrorsAndOmissionsLineBusiness.Protocol,
            Lmi = coverages.MaxBy(x => x.Limit.FormatCurrencyAmt.Amt).Limit.FormatCurrencyAmt.Amt,
            ExpectedPremium = transaction.ACORD.AccountingSvcRs.BillingNotifyRs.BillingPolicyInfo.PaymentOption
                .InstallmentInfo.InstallmentAmt.Amt,
            CommissionRate = (double)transaction.ACORD.InsuranceSvcRs.ErrorsAndOmissionsPolicyQuoteInqRs.Producer.CommissionRate / 100,
            Vessel = new VesselRequest
            {
                Name = vesselInfo.VesselName,
                Crews = vesselInfo.NumberOfCrew,
                Number = vesselInfo.VesselNumber,
                ActivityId = vesselInfo.ServiceOrActivity,
                PropulsionId = TableConverter.PropulsionTypeDict[vesselInfo.Propulsion],
                TypeId = TableConverter.VesselTypeDict[vesselInfo.TypeOfVessel],
                NavigationTypeId = TableConverter.TypeOfNavigationDict[vesselInfo.TypeOfNavigation],
                UseOfTheVesselId = TableConverter.UseOfVesselDict[vesselInfo.UseOfTheVessel],
                NumberOfPassengers = vesselInfo.NumberOfPassengers,
                SegmentId = 19
            },
            Insured = new InsuredRequest
            {
                FirstName = NameForIdentity(insuredOrPrincipal.NameInfo.PersonName.RegisterFirstName,
                    insuredOrPrincipal.NameInfo.NonTaxIdentity.NonTaxId),
                LastName = insuredOrPrincipal.NameInfo.FamilyName.RegisterLastName,

                SocialFirstName = insuredOrPrincipal.NameInfo.PersonName.GivenName,
                SocialLastName = insuredOrPrincipal.NameInfo.FamilyName.FamilyNames,

                BirthDate = errosAndOmission.InsuredOrPrincipal.InsuredOrPrincipalInfo.PersonInfo.BirthDt,
                Phone = GetPhoneNumber(insuredOrPrincipal.Communications.PhoneInfo.PhoneNumber),
                Identity = insuredOrPrincipal.NameInfo.NonTaxIdentity.NonTaxId,
                Address = new AddressRequest
                {
                    Cep = insuredOrPrincipal.Addr.PostalCode.Replace("-", string.Empty),
                    City = insuredOrPrincipal.Addr.City,
                    State = insuredOrPrincipal.Addr.StateCd,
                    Number = insuredOrPrincipal.Addr.DetailAddr.UnitNumber,
                    Street = insuredOrPrincipal.Addr.DetailAddr.StreetName,
                    Complement = insuredOrPrincipal.Addr.DetailAddr.Complementary,
                    Neighborhood = insuredOrPrincipal.Addr.Region
                }
            },
            Coverages = coverages.Select(w => new CoverageRequest
            {
                Id = w.IterationNumber,
                Lmi = w.Limit.FormatCurrencyAmt.Amt
            }).ToList(),
            PaymentDate = transaction.ACORD.AccountingSvcRs.BillingNotifyRs.BillingPolicyInfo.PaymentOption
                .InstallmentInfo.InstallmentDueDt
        };
    }

    private string GetPhoneNumber(string phoneNumber)
    {
        try
        {
            var instance = PhoneNumberUtil.GetInstance();

            var parsedPhone = instance.Parse(phoneNumber, "BR");

            if (instance.IsValidNumber(parsedPhone))
            {
                return phoneNumber;
            }
        }
        catch (Exception)
        {

        }

        var cleanedNumber = Regex.Replace(phoneNumber, @"\D", "");

        cleanedNumber = cleanedNumber.Length < 3 ? "1140001246" : cleanedNumber;

        var ddd = int.Parse(cleanedNumber[..2]);

        if (!_validDDDs.Contains(ddd))
        {
            ddd = 11;
        }

        var phone = cleanedNumber[2..];

        phone = phone.Length < 9 ? phone.PadLeft(9, '9') : phone;
        phone = phone.Length > 9 ? phone[..9] : phone;

        return $"{ddd}{phone}";
    }

    private void ValidateCertificate(string certificate)
    {
        if (string.IsNullOrEmpty(certificate))
            throw new ArgumentException("Certificate cannot be null or empty.");
    }

    private void ValidateEffectiveDate(DateTime effectiveDate)
    {
        if (effectiveDate == default)
            throw new ArgumentException("EffectiveDate is not a valid date.");
    }

    private void ValidateExpiryDate(DateTime expiryDate)
    {
        if (expiryDate == default)
            throw new ArgumentException("ExpiryDate is not a valid date.");
        if (expiryDate <= DateTime.Now)
            throw new ArgumentException("ExpiryDate must be in the future.");
    }

    private void ValidateProtocolDate(DateTime protocolDate)
    {
        if (protocolDate == default)
            throw new ArgumentException("ProtocolDate is not a valid date.");
    }

    private void ValidateLmi(decimal lmi)
    {
        if (lmi <= 0)
            throw new ArgumentException("Lmi must be greater than zero.");
    }

    private void ValidateExpectedPremium(decimal expectedPremium)
    {
        if (expectedPremium <= 0)
            throw new ArgumentException("ExpectedPremium must be greater than zero.");
    }

    private void ValidateVessel(VesselInformation vessel)
    {
        if (vessel == null)
            throw new ArgumentException("Vessel information must be provided.");

        if (string.IsNullOrEmpty(vessel.VesselName))
            throw new ArgumentException("Vessel name cannot be null or empty.");

        if (vessel.NumberOfCrew < 0)
            throw new ArgumentException("Number of crews must be non-negative.");

        if (string.IsNullOrEmpty(vessel.VesselNumber))
            throw new ArgumentException("Vessel number cannot be null or empty.");

        if (vessel.ServiceOrActivity.IsNullOrEmpty())
            throw new ArgumentException("Invalid ActivityId.");

        if (TableConverter.PropulsionTypeDict[vessel.Propulsion] <= 0)
            throw new ArgumentException("Invalid PropulsionId.");

        if (TableConverter.VesselTypeDict[vessel.TypeOfVessel] <= 0)
            throw new ArgumentException("Invalid TypeId.");

        if (TableConverter.TypeOfNavigationDict[vessel.TypeOfNavigation] <= 0)
            throw new ArgumentException("Invalid NavigationTypeId.");

        if (TableConverter.UseOfVesselDict[vessel.UseOfTheVessel] <= 0)
            throw new ArgumentException("Invalid UseOfTheVesselId.");

        if (vessel.NumberOfPassengers < 0)
            throw new ArgumentException("Number of passengers must be non-negative.");
    }

    private void ValidateInsured(InsuredOrPrincipal insuredOrPrincipal)
    {
        if (insuredOrPrincipal == null)
            throw new ArgumentException("Insured information must be provided.");

        var generalPartyInfo = insuredOrPrincipal.GeneralPartyInfo;
        var nameInfo = generalPartyInfo.NameInfo;

        if (string.IsNullOrEmpty(nameInfo.PersonName.RegisterFirstName) ||
            string.IsNullOrEmpty(nameInfo.FamilyName.RegisterLastName))
            throw new ArgumentException("Insured first and last names cannot be null or empty.");

        var birthDate = insuredOrPrincipal.InsuredOrPrincipalInfo?.PersonInfo?.BirthDt;
        if (nameInfo.NonTaxIdentity.NonTaxId.Length == 11 && (birthDate == null || birthDate == default))
            throw new ArgumentException("Insured birth date is not valid.");

        var phone = generalPartyInfo.Communications.PhoneInfo.PhoneNumber;
        if (string.IsNullOrEmpty(phone))
            throw new ArgumentException("Insured phone number cannot be null or empty.");

        if (string.IsNullOrEmpty(nameInfo.NonTaxIdentity.NonTaxId))
            throw new ArgumentException("Insured identity cannot be null or empty.");

        //var instance = PhoneNumberUtil.GetInstance();
        //var parsed = instance.Parse(phone, "BR");

        //if (!instance.IsValidNumber(parsed))
        //    throw new ArgumentException("Invalid phone number.");

        //if (parsed.CountryCode != 55)
        //    throw new ArgumentException("O número de telefone deve ser do Brasil.");

        ValidateAddress(generalPartyInfo.Addr);
    }


    private void ValidateAddress(Addr address)
    {
        if (address == null)
            throw new ArgumentException("Address information must be provided.");

        if (string.IsNullOrEmpty(address.PostalCode))
            throw new ArgumentException("CEP cannot be null or empty.");

        if (string.IsNullOrEmpty(address.City))
            throw new ArgumentException("City cannot be null or empty.");

        if (string.IsNullOrEmpty(address.StateCd))
            throw new ArgumentException("State cannot be null or empty.");

        if (string.IsNullOrEmpty(address.DetailAddr.StreetName))
            throw new ArgumentException("Street cannot be null or empty.");

        if (string.IsNullOrEmpty(address.DetailAddr.UnitNumber))
            throw new ArgumentException("Number cannot be null or empty.");
    }

    private void ValidateCoverages(List<CommlCoverage> coverages)
    {
        if (coverages == null || !coverages.Any())
            throw new ArgumentException("At least one coverage must be provided.");

        foreach (var coverage in coverages)
        {
            if (coverage.IterationNumber.IsNullOrEmpty())
                throw new ArgumentException("Invalid coverage Id.");

            if (coverage.Limit.FormatCurrencyAmt.Amt <= 0)
                throw new ArgumentException("Coverage Lmi must be greater than zero.");
        }
    }

    private void ValidatePaymentDate(DateTime paymentDate)
    {
        if (paymentDate == default)
            throw new ArgumentException("PaymentDate is not a valid date.");
    }

    static string NameForIdentity(string name, string identity)
    {
        if (identity.Length != 14) return name;

        const int cnpjMatrixNumber = 1;

        var branchNumberStr = identity.Substring(8, 4);
        if (int.TryParse(branchNumberStr, out var branchNumber))
        {
            return branchNumber == cnpjMatrixNumber ? name : $"{name} #{branchNumber}";
        }

        return name;
    }
}