using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Ebao.V2.DPEM.Api.Print.Requests
{
    public class EmailProcessRequest
    {
        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; set; }

        [AliasAs("actionType")]
        public string ActionType => "emailProcess";

        [AliasAs("fromProcess")]
        public string FromProcess => "IssuancePrintingProcess";

        [AliasAs("docDecision")]
        public string DocDecision => "email";

        [AliasAs("transactionType")]
        public string TransactionType => "Issue";

        [AliasAs("transaction")]
        public string Transaction => "SP Policy issuance";

        [AliasAs("agpi")]
        public string Agpi => "emailProcess";

        [AliasAs("policyId")]
        public string PolicyId { get; set; }

        [AliasAs("transactionNo")]
        public string TransactionNo { get; set; }
    }
}
