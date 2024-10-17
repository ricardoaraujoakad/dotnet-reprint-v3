namespace Ebao.V2.DPEM.Models;

public class RequestInfo
{
    public string Auth { get; set; }

    public RequestInfo(string auth)
    {
        Auth = auth;
    }
}