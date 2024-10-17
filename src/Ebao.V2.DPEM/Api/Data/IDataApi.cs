using Ebao.V2.DPEM.Helpers.Model;
using Ebao.V2.DPEM.Models.Contants;
using Refit;

namespace Ebao.V2.DPEM.Api.Data;

public interface IDataApi
{
    [Get("/insurance/gs/servlet/com.ebao.gs.pub.js.dataexchange.DataExchangeAction")]
    Task<IReadOnlyList<KeyValue>> GetPaymentMethodsAsync([Query] int order = 6,
        string tableName = TableNames.PaymentMethod, string where = Conditions.PaymentMethod);

    [Get("/insurance/gs/servlet/com.ebao.gs.pub.js.dataexchange.DataExchangeAction")]
    Task<IReadOnlyList<KeyValue>> GetCedentesAsync([Query] int order = 6, string tableName = TableNames.Cedentes,
        string where = Conditions.Cedentes);
    
    [Get("/insurance/getCodeDescInfo.do")]
    Task<IReadOnlyList<KeyValue>> GetActorsAsync([Query] int order = 6, string tableName = TableNames.Users,
        string where = Conditions.ActorsConditions);

    [Get("/insurance/getCodeDescInfo.do")]
    Task<string> GetBankCodeDetails([Query] [AliasAs("code_id")] string code,
        [Query] [AliasAs("params")] string parameters = "", [Query] [AliasAs("sAction")] string action = "getCodeDesc",
        string entryflag = "js", [AliasAs("table_name")] string tableName = TableNames.BankCode);

    [Get("/insurance/getCodeDescInfo.do")]
    Task<string> GetBankAgencyDetails([Query] [AliasAs("code_id")] string code,
        [Query] [AliasAs("params")] string parameters = "", [Query] [AliasAs("sAction")] string action = "getCodeDesc",
        string entryflag = "js", [AliasAs("table_name")] string tableName = TableNames.BankAgency);

    [Get("/insurance/party/CodeAction.do")]
    Task<string> GetCepInfoAsync([Query] string code, [Query] string method = "addressFilling",
        [Query] [AliasAs(Parameters.Syskey)] string sysKey = "");

    private class TableNames
    {
        public const string PaymentMethod = "5E3D176B88240C1DC9EB9SMMcATbNNyGkxOGxfGXCdcz89WR3c9fN1cwuNRs%3D";
        public const string Cedentes = "T_CTS_BOLETO_BANK";
        public const string BankCode = "V_PTM_BANK_CODE_002";
        public const string BankAgency = "V_PTM_BANK_AGENCY_CODE";
        public const string Users = "4B2ED07E011F6897571F5aMdgA2Uc";
    }

    private class Conditions
    {
        public const string PaymentMethod = "{params:[0, 28]}";
        public const string Cedentes = " RECORD_ID IN (1)";

        public const string ActorsConditions =
            "4B2ED07E011F6897571F5egVjEcSvdDGn0jbAUBezRCMCgzRrZyYEx6bvMAalEhoRcPM2VzFgF7MU8jMiJVsnZjSGN35xtkVSqvAxAlc3YbB38jVDstIFW0f3NSc2DiFmFfM7QTCyB3fR8UZDBFMywiX6RlakV3YOofc0QmpxQLJn9kBgB3M0I0KDNGsXVjQ3Ju8wRqViCjEQ8za3UaDXU1TyMzJFe8dGpIY3TgFmtQJa8DFCN7cRIGfyNWNyAiUrN%2Fc1N7ZucTZ18zthMBI3B1HxRkNEIxJCdfpGRlR3dl5x9zQCqnFAskf2QHA3o1RzIoM0Sxd2RAem7zBGdUJKAUDzNgcBYHcTtPIzMjWrxyZEhjd%2BcTYVAlrwMVI3N0EgN%2FI1QyLCNbs39zU3Zj6xRmXzO1GgEid30fFGQwRTYkIF%2BkZGdEcWXjH3NHJ6sRAyZ%2FZAUBezRDNigzRLB1akZzbvMFYlUkoxEPM2Z3GwF2Nk8jMyVUvXFrSGN05hRqVyevAxQndX0SDX8jWzIiI1Oxf3NTe2bgGmZfM7ETAyt3dhIIYyRDNCEqU7hjdER7Z%2BMab0M7ohsDJHd4AxN1MkExJS9DoHFmQ3Vn7wN3WiqkGgcvY2AXDHEwQD80NFO2e2JDf3L0G2dXI6MfEzR1dBoFcj9TJSclV7Jyb1RlYecRalAvswUGJXd9EwhjJUc6IiJQuGN1TXth6xJvQzSmFwQmc3gDEHA6SjchL0Oie2NDe2bvA3ZbIKIbCi9jYxYAdDFDPzQ7UbF6a0R%2FcvQaYFIlpB8TNXR8FA16P1MrJSpRtnZvVGdl6xBlUC%2BzBQUgcnMTCGMmRzYhJ1O4Y3VNcWLqGm9DNKsXByJzeAMccjdENycvQ6J2a0F0Zu8DdFYnpBMCL2NgEQB7Mko%2FNDRbtnphR39y9hdrVCakHxMwdXASAXc%2FUyYkK1awdm9UZGLqFWJbL7MGBydzfRoIYyVGOyEjVLhje0ZxYOAQb0MyqxoEIXd4AxNzOkcyJy9DoXZrRXFq7wN3WiqrEwQvY2AQDXo7Qz80N1C9emtDf3L2FmtSIaofEzZ7cRYDdz9TJCAkVLBxb1RkZOQWalsvswUKJHJ0FQhjJkowJiZVuGN1Q3Zn5RBvQzKiFgIrc3gDF3QyQTMiL0Oid2dEcWDvA3RTK6ARBi9jZRcEdzNPIzMqW7Z7YEhjd%2BEVZFInrwMUJ3N0FwR%2FI1c0LCNRtX9zUnVh4RNlXzO3FAYqcHUfFGU6SjQiIl%2BkZGNEemvnH3NEJqoWByF%2FZAANdDtBNygzQrd3Z0BzbvMHZFsgpBMPM2R0GwFyO08jMiJVsntiSGN04xtmUSGvAxUidXITAn8jVDYlJFW9f3NSdmrkFmFfM7cUBCRydR8UZDFGNScjX6RrY0Z6Y%2BEac0Jd12MWYDYmXGAqYAJheHYDqXN0WgRy8%3D";
    }
}