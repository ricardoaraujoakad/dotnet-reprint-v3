using System.Xml.Serialization;

namespace Ebao.V2.DPEM;

[XmlRoot("Transactions")]
public class Transactions
{
    [XmlElement("Transaction")]
    public List<Transaction> TransactionList { get; set; }
}

public class Transaction
{
    public string TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; }
    public string PolicyId { get; set; }
    public string BusinessUnit { get; set; }
    public bool IsRenewal { get; set; }
    public string CertificateUrl { get; set; }
    public ACORD ACORD { get; set; }
}

public class ACORD
{
    public SignonRs SignonRs { get; set; }
    public InsuranceSvcRs InsuranceSvcRs { get; set; }
    [XmlElement("AccountingSvcRs")]
    public AccountingSvcRs AccountingSvcRs { get; set; }
}

public class SignonRs
{
    public DateTime ClientDt { get; set; }
    public ClientApp ClientApp { get; set; }
    public DateTime ServerDt { get; set; }
}

public class ClientApp
{
    public string Org { get; set; }
    public string Product { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
}

public class InsuranceSvcRs
{
    public ErrorsAndOmissionsPolicyQuoteInqRs ErrorsAndOmissionsPolicyQuoteInqRs { get; set; }
}

public class ErrorsAndOmissionsPolicyQuoteInqRs
{
    public ErrorsAndOmissionsLineBusiness ErrorsAndOmissionsLineBusiness { get; set; }
    public CommlPolicy CommlPolicy { get; set; }
    public InsuredOrPrincipal InsuredOrPrincipal { get; set; }
    public Producer Producer { get; set; }
    public AffinityGroup AffinityGroup { get; set; }
}

public class ErrorsAndOmissionsLineBusiness
{
    public string CompanyProductId { get; set; }
    public string CommercialProduct { get; set; }
    public string OperationId { get; set; }
    public string OperationName { get; set; }
    public DateTime Protocol { get; set; }
    public ClaimsMadeInfo ClaimsMadeInfo { get; set; }
    public string Segment { get; set; }
    [XmlElement("CommlCoverage")]
    public List<CommlCoverage> CommlCoverages { get; set; }
}

public class ClaimsMadeInfo
{
    public DateTime CurrentRetroactiveDt { get; set; }
    public DateTime ComplementaryExpiryDate { get; set; }
}

public class CommlCoverage
{
    public string IterationNumber { get; set; }
    public string CoverageDesc { get; set; }
    public Limit Limit { get; set; }
}

public class Limit
{
    public FormatCurrencyAmt FormatCurrencyAmt { get; set; }
}

public class FormatCurrencyAmt
{
    public int Precision { get; set; }
    public decimal Amt { get; set; }
}

public class CommlPolicy
{
    public DateTime OriginalInceptionDt { get; set; }
    public ContractTerm ContractTerm { get; set; }
    public OtherOrPriorPolicy OtherOrPriorPolicy { get; set; }
    public PaymentOption PaymentOption { get; set; }
    public decimal WrittenPremiumInterest { get; set; }
    public VesselInformation VesselInformation { get; set; }
}

public class ContractTerm
{
    public DateTime EffectiveDt { get; set; }
    public DateTime ExpirationDt { get; set; }
}

public class OtherOrPriorPolicy
{
    public string PolicyNumber { get; set; }
}

public class PaymentOption
{
    public int NumPayments { get; set; }
    public bool Prepayment { get; set; }
    public bool PrepaymentFlag { get; set; }
    public InstallmentInfo InstallmentInfo { get; set; }

}

public class VesselInformation
{
    public string VesselName { get; set; }
    public string VesselNumber { get; set; }
    public int NumberOfCrew { get; set; }
    public int NumberOfPassengers { get; set; }
    public string Propulsion { get; set; }
    public string TypeOfVessel { get; set; }
    public string TypeOfNavigation { get; set; }
    public string UseOfTheVessel { get; set; }
    public string ServiceOrActivity { get; set; }
}

public class InsuredOrPrincipal
{
    public GeneralPartyInfo GeneralPartyInfo { get; set; }
    public InsuredOrPrincipalInfo InsuredOrPrincipalInfo { get; set; }
}

public class GeneralPartyInfo
{
    public NameInfo NameInfo { get; set; }
    public Addr Addr { get; set; }
    public Communications Communications { get; set; }
}

public class NameInfo
{
    public PersonName PersonName { get; set; }
    public FamilyName FamilyName { get; set; }
    public NonTaxIdentity NonTaxIdentity { get; set; }
}

public class PersonName
{
    public string GivenName { get; set; }
    public string RegisterFirstName { get; set; }
}

public class FamilyName
{
    public string FamilyNames { get; set; }
    public string RegisterLastName { get; set; }
}

public class NonTaxIdentity
{
    public string NonTaxId { get; set; }
    public string NonTaxIdTypeCd { get; set; }
}

public class Addr
{
    public DetailAddr DetailAddr { get; set; }
    public string Region { get; set; }
    public string City { get; set; }
    public string StateCd { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
}

public class DetailAddr
{
    public string StreetName { get; set; }
    public string UnitNumber { get; set; }
    public string Complementary { get; set; }
}

public class Communications
{
    public PhoneInfo PhoneInfo { get; set; }
    public EmailInfo EmailInfo { get; set; }
}

public class PhoneInfo
{
    public string PhoneNumber { get; set; }
}

public class EmailInfo
{
    public string EmailAddr { get; set; }
}

public class InsuredOrPrincipalInfo
{
    public string InsuredOrPrincipalType { get; set; }
    public PersonInfo PersonInfo { get; set; }
}

public class PersonInfo
{
    public string GenderCd { get; set; }
    public DateTime BirthDt { get; set; }
}

public class Producer
{
    public bool Principal { get; set; }
    public decimal CommissionRate { get; set; }
    public ProducerInfo ProducerInfo { get; set; }
}

public class ProducerInfo
{
    public string Name { get; set; }
    public string ContractNumber { get; set; }
}
public class AffinityGroup
{
    public int IdAffinityGroup { get; set; }
    public bool HasAffinityGroup { get; set; }
    public GeneralPartyInfo GeneralPartyInfo { get; set; }

    [XmlElement("ResponsibleForPayment")]
    public string ResponsibleForPaymentString { get; set; }

    [XmlIgnore]
    public bool ResponsibleForPayment
    {
        get { return bool.TryParse(ResponsibleForPaymentString, out bool result) && result; }
        set { ResponsibleForPaymentString = value.ToString().ToLower(); }
    }
}

public class AccountingSvcRs
{
    [XmlAttribute("id")]
    public string Id { get; set; }
    public BillingNotifyRs BillingNotifyRs { get; set; }
}

public class BillingNotifyRs
{
    public BillingPolicyInfo BillingPolicyInfo { get; set; }
}

public class BillingPolicyInfo
{
    public decimal CommissionRate { get; set; }
    public PaymentOption PaymentOption { get; set; }
}

public class InstallmentInfo
{
    public DateTime InstallmentDueDt { get; set; }
    public int InstallmentNumber { get; set; }
    public FormatCurrencyAmt InstallmentAmt { get; set; }
}