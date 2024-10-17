using System.Net;

namespace Ebao.V2.DPEM;

public static class Config
{
    public static string Username => IntegrationConfig.EbaoLogin;
    public static string ProductId => "210001264";
    public static string ProductCode => "DPEM";
    
    public static IntegrationConfig IntegrationConfig { get; set; }
    public static CookieContainer CookieContainer = new CookieContainer();
    public static object State { get; set; }
}