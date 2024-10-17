using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Ebao.V2.DPEM.Api.Print.Requests
{
    public class V3PrintRequest
    {
        [AliasAs("sActionType")]
        public string ActionType => "query";

        [AliasAs("issueType")]
        public int IssueType => 1;

        [AliasAs("moduleType")]
        public int ModuleType => 1;

        [AliasAs("tmpId")]
        public string TmpId => "";

        [AliasAs("policyId")]
        public string PolicyId => "";

        [AliasAs("policyType")]
        public int PolicyType => 1;

        [AliasAs("policyNo")]
        public string PolicyNo { get; set; }

        [AliasAs("dropdown_productCodetxt")]
        public string DropdownProductCodeTxt => "Please select";

        [AliasAs("productCode")]
        public string ProductCode => "";

        [AliasAs("agentCode")]
        public string AgentCode => "";

        [AliasAs("masterPolicyNo")]
        public string MasterPolicyNo => "";

        [AliasAs("customName")]
        public string CustomName => "";

        [AliasAs("nameFuzzySearch")]
        public int NameFuzzySearch => 1;

        [AliasAs("policyCate")]
        public int PolicyCate => 1;

        [AliasAs("propDateFrom")]
        public string PropDateFrom => "";

        [AliasAs("propDateTo")]
        public string PropDateTo { get; set; }

        [AliasAs("effDateFrom")]
        public string EffDateFrom => "";

        [AliasAs("effDateTo")]
        public string EffDateTo => "";

        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; set; }

        public V3PrintRequest(string policyNo, string propDateTo, string syskeyRequestToken)
        {
            PolicyNo = policyNo;
            PropDateTo = propDateTo;
            SyskeyRequestToken = syskeyRequestToken;
        }
    }
}
