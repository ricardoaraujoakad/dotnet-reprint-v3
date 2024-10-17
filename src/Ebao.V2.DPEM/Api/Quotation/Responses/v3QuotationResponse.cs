using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Responses;

public class V3QuotationResponse
{
    [AliasAs("_ctId")]
    public string CtId { get; set; }

    [AliasAs("_ctType")]
    public string CtType { get; set; }

    [AliasAs("_id")]
    public string Id2 { get; init; }

    [AliasAs("_insuredCateCode")]
    public int InsuredCateCode { get; set; }

    // [AliasAs("ms__id439")]
    // public int MsId439 { get; set; }
    //
    // [AliasAs("ms__id431")]
    // public int MsId431 { get; set; }
    //
    // [AliasAs("ms__id429")]
    // public int MsId429 { get; set; }

    [AliasAs("adjustedNetPremium")]
    public double AdjustedNetPremium { get; set; }

    [AliasAs("agreementId")]
    public string AgreementId { get; set; }

    [AliasAs("annualPremium")]
    public double AnnualPremium { get; set; }

    [AliasAs("backUrl")]
    public string BackUrl { get; set; }

    [AliasAs("bankName")]
    public string BankName { get; set; }

    [AliasAs("boletoBank")]
    public int BoletoBank => 1;

    [AliasAs("commCurType")]
    public int CommCurType { get; set; }

    [AliasAs("commercialPrdt")]
    public string CommercialPrdt { get; set; }

    [AliasAs("commRateCharger")]
    public string CommRateCharger { get; set; }

    [AliasAs("contactorEmail")]
    public string ContactorEmail { get; set; }

    [AliasAs("partnership")]
    public string PartnerShip { get; set; }

    // [AliasAs("_id279")]
    // public int TransType { get; set; }
    //
    // [AliasAs("_id289")]
    // public int Segment { get; set; }
    //
    // [AliasAs("_id293")]
    // public int Policy { get; set; } = 1;
    //
    // [AliasAs("_id291")]
    // public int ClaimBaseType { get; set; } = 2;
    //
    // [AliasAs("_id296")]
    // public int PropulsionId { get; set; }
    //
    // [AliasAs("_id299")]
    // public int VesselTypeId { get; set; }
    //
    // [AliasAs("_id301")]
    // public int NavigationTypeId { get; set; }
    //
    // [AliasAs("_id303")]
    // public int UseOfTheVesselId { get; set; }
    //
    // [AliasAs("_id297")]
    // public string VesselActivityId { get; set; }

    [AliasAs("contactorName")]
    public string ContactorName { get; set; }

    [AliasAs("contactorPartyId")]
    public string ContactorPartyId { get; set; }

    [AliasAs("coverNoteNo")]
    public string CoverNoteNo { get; set; }

    [AliasAs("coverNoteType")]
    public string CoverNoteType { get; set; }

    [AliasAs("ctx.bizProductCode")]
    public string CtxBizProductCode { get; set; }

    [AliasAs("ctx.ctCode")]
    public string CtxCtCode { get; set; }

    [AliasAs("ctx.ctId")]
    public string CtxCtId { get; set; }

    [AliasAs("ctx.insuredCateCode")]
    public string CtxInsuredCateCode { get; set; }

    [AliasAs("ctx.insuredId")]
    public long CtxInsuredId { get; set; }

    public bool CtxIsEndorsement { private get; set; }

    [AliasAs("ctx.isEndorsement")]
    public string CtxIsEndorsementString => CtxIsEndorsement.ToString().ToLower();

    public bool CtxIsInDataEntry { private get; set; }

    [AliasAs("ctx.isInDataEntry")]
    public string CtxIsInDataEntryString => CtxIsInDataEntry.ToString().ToLower();

    public bool CtxIsInManualPricing { private get; set; }

    [AliasAs("ctx.isInManualPricing")]
    public string CtxIsInManualPricingString => CtxIsInManualPricing.ToString().ToLower();

    public bool CtxIsInQuotation { private get; set; }

    [AliasAs("ctx.isInQuotation")]
    public string CtxIsInQuotationString => CtxIsInQuotation.ToString().ToLower();

    public bool CtxIsNewBiz { private get; set; }

    [AliasAs("ctx.isNewBiz")]
    public string CtxIsNewBizString => CtxIsNewBiz.ToString().ToLower();

    public bool CtxIsRenewal { private get; set; }

    [AliasAs("ctx.isRenewal")]
    public string CtxIsRenewalString => CtxIsRenewal.ToString().ToLower();

    [AliasAs("ctx.policyCtId")]
    public string CtxPolicyCtId { get; set; }

    [AliasAs("ctx.policyId")]
    public long CtxPolicyId { get; set; }

    [AliasAs("ctx.productCode")]
    public string CtxProductCode { get; set; }

    [AliasAs("ctx.productId")]
    public long CtxProductId { get; set; }

    [AliasAs("descritorId")]
    public string DescritorId { get; set; }

    [AliasAs("edit")]
    public string Edit { get; set; }

    [AliasAs("endoId")]
    public string EndoId { get; set; }

    [AliasAs("er_message")]
    public string ErrorMessage { get; set; }

    [AliasAs("er_result")]
    public string ErrorResult { get; set; }

    [AliasAs("er1")]
    public double Error1 { get; set; }

    [AliasAs("er2")]
    public double Error2 { get; set; }

    [AliasAs("er3")]
    public double Error3 { get; set; }

    [AliasAs("initCommRateCharger")]
    public string InitCommRateCharger { get; set; }

    [AliasAs("innerMemo")]
    public string InnerMemo { get; set; }

    [AliasAs("instalNo")]
    public string InstalNo { get; set; }

    [AliasAs("insuredId")]
    public string InsuredId { get; set; }

    [AliasAs("introducerPtyrId")]
    public string IntroducerPtyrId { get; set; }

    [AliasAs("key_wincony_xmlData")]
    public string Xml { get; set; }

    [AliasAs("masterPolicyCopy")]
    public string MasterPolicyCopy { get; set; }

    [AliasAs("modelName")]
    public string ModelName { get; set; }

    [AliasAs("needSave")]
    public string NeedSave { get; set; }

    [AliasAs("nettPremium")]
    public double NettPremium { get; set; }

    [AliasAs("nextPageType")]
    public string NextPageType { get; set; }

    [AliasAs("oldAgreementId")]
    public string OldAgreementId { get; set; }

    [AliasAs("operId")]
    public string OperId { get; set; }

    [AliasAs("pageType")]
    public string PageType { get; set; }

    [AliasAs("payMode")]
    public string PayMode { get; set; }

    [AliasAs("planId")]
    public string PlanId { get; set; }

    [AliasAs("platformCerti")]
    public string PlatformCerti { get; set; }

    [AliasAs("policyId")]
    public string PolicyId { get; set; }

    //Não é engano de cópia!!! O eBao envia duas vezes o policyId com o mesmo nome e valor.
    [AliasAs("policyId")]
    public string PolicyId2 => PolicyId;

    [AliasAs("policyProtocolFinalizedDateStr")]
    public string PolicyProtocolFinalizedDateStr { get; set; }

    [AliasAs("PremCurrencyCode")]
    public string PremCurrencyCode { get; set; }

    [AliasAs("premiumPayable")]
    public double PremiumPayable { get; set; }

    [AliasAs("prePageType")]
    public string PrePageType { get; set; }

    [AliasAs("prepaymentFlag")]
    public string PrePaymentFlag { get; set; }

    [AliasAs("previousPolicyNo")]
    public string PreviousPolicyNo { get; set; }

    [AliasAs("productId")]
    public long ProductId { get; set; }

    [AliasAs("propDate")]
    public string PropDate { get; set; }

    [AliasAs("quoDate")]
    public string QuoDate { get; set; }

    [AliasAs("quoteNo")]
    public long Id { get; set; }

    [AliasAs("referenceDateEditableFlag")]
    public string ReferenceDateEditableFlag { get; set; }

    [AliasAs("referenceDateType")]
    public string ReferenceDateType { get; set; }

    [AliasAs("reinsuranceReferenceDateStr")]
    public string ReinsuranceReferenceDateStr { get; set; }

    public bool RenewalFlag { private get; set; }

    [AliasAs("renewalFlag")]
    public string RenewalFlagString => RenewalFlag.ToString().ToLower();

    [AliasAs("renewalMemo")]
    public string RenewalMemo { get; set; }

    [AliasAs("rewriteType")]
    public string RewriteType { get; set; }

    [AliasAs("sActionType")]
    public string SActionType { get; set; }

    [AliasAs("salesPlatform")]
    public int SalesPlatform { get; set; }

    [AliasAs("selectInstallmentMethod")]
    public string SelectInstallmentMethod { get; set; }

    [AliasAs("selectInstallmentNumber")]
    public string SelectInstallmentNumber { get; set; }

    [AliasAs("selectOptionId")]
    public string SelectOptionId { get; set; }

    [AliasAs("selectPaymentPlan")]
    public string SelectPaymentPlan { get; set; }

    [AliasAs("selectPrepayFlag")]
    public string SelectPrepayFlag { get; set; }

    [AliasAs("siCurrencyCode")]
    public string SiCurrencyCode { get; set; }

    [AliasAs("specRule")]
    public string SpecRule { get; set; }

    [AliasAs("specRuleForSchedule")]
    public string SpecRuleForSchedule { get; set; }

    [AliasAs("subNo")]
    public string SubNo { get; set; }

    [AliasAs("syskey_page_token")]
    public string SyskeyPageToken { get; set; }

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; set; }

    [AliasAs("tableName")]
    public string TableName { get; set; }

    [AliasAs("tariffPremium")]
    public double TariffPremium { get; set; }

    public string OperatorId { get; set; }
    public string OperatorName { get; set; }
}