using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Ebao.V2.DPEM.Api.Print.Requests
{
    public class EmailDocumentRequest
    {
        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; set; }

        [AliasAs("actionType")]
        public string ActionType => "emailDoc";

        [AliasAs("policyNo")]
        public string PolicyNo { get; set; }

        [AliasAs("endoNo")]
        public string EndoNo => "";

        [AliasAs("policyId")]
        public string PolicyId { get; set; }

        [AliasAs("endoId")]
        public string EndoId => "null";

        [AliasAs("transactionType")]
        public string TransactionType => "Issue";

        [AliasAs("transactionNo")]
        public string TransactionNo { get; set; }

        [AliasAs("declarationId")]
        public string DeclarationId => "null";

        [AliasAs("transaction")]
        public string Transaction => "SP Policy issuance";

        [AliasAs("path")]
        public string Path => "";

        [AliasAs("fromProcess")]
        public string FromProcess => "IssuancePrintingProcess";
    }
}
