using Refit;

namespace Ebao.V2.DPEM.Api.Client.Requests;

public record V3SearchByIdentityRequest
{
    [AliasAs("customerType")]
    public string CustomerType { get; }

    [AliasAs("nacionality")]
    public string Nacionality => CustomerType == "company" ? null : "26";

    [AliasAs("newIdNumberCritia")]
    public string NewIdNumberCritia { get; }

    [AliasAs("registerNumberCritia")]
    public string RegisterNumberCritia { get; }

    [AliasAs("sActionType")]
    public string SActionType { get; } = "doQueryCustomer";

    [AliasAs("entryMode")]
    public string EntryMode { get; set; } = "search";

    [AliasAs("syskey_request_token")]
    public string Syskey { get; set; }

    public V3SearchByIdentityRequest(string newIdNumberCritia, string customerType)
    {
        if (customerType == "company")
            RegisterNumberCritia = newIdNumberCritia;
        else
            NewIdNumberCritia = newIdNumberCritia;
        CustomerType = customerType;
    }
}