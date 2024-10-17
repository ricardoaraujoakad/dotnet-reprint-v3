namespace Ebao.V2.DPEM.Models;

public class UserInfo
{
    public string Syskey { get; set; }
    public string SessionId { get; set; }

    public UserInfo(string syskey, string sessionId)
    {
        Syskey = syskey;
        SessionId = sessionId;
    }
}