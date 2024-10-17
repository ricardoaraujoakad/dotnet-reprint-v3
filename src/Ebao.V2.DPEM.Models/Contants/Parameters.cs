namespace Ebao.V2.DPEM.Models.Contants;

public static class Parameters
{
    public const string Syskey = "syskey_request_token";
    public const string Session = "JSESSIONID";
    public static readonly string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://ebaoapphomol.akadseguros.com.br";
}