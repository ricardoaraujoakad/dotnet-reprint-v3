using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Ebao.V2.DPEM;
using Ebao.V2.DPEM.Exceptions;
using Ebao.V2.DPEM.Helpers.Extensions;
using Ebao.V2.DPEM.Services;
using Medallion.Threading.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;
using Serilog.Events;


var basePath = GetApplicationDirectory();

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var configFilePath = Path.Combine(basePath, "Configs/appsettings-prod.json");
var configFile = await File.ReadAllTextAsync(configFilePath);
Config.IntegrationConfig = JsonSerializer.Deserialize<IntegrationConfig>(configFile);


var logPath = Path.Combine(basePath, "Logs",
    Config.IntegrationConfig.EbaoLogin.Replace(".", "_") + ".txt");

var outputTemplate =
    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{BotUser}-{ProposalId} {Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Logger(lg => lg.WriteTo.Console(outputTemplate: outputTemplate)
        .Filter.ByExcluding(x => x.MessageTemplate.Text.Contains("HTTP")))
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Verbose,
        outputTemplate: outputTemplate
    )
    .CreateLogger();


using var _ = LogContext.PushProperty("BotUser", Config.IntegrationConfig.EbaoLogin);


Log.Information("Usuário eBao: {user}.", Config.IntegrationConfig.EbaoLogin);
Log.Information("URL eBao: {url}.", Config.IntegrationConfig.EbaoUrl);

Console.Title = Config.IntegrationConfig.EbaoLogin;

var provider = ServicesRegister.BuildServiceProvider();


await using var scope = provider.CreateAsyncScope();

var ebaoService = scope.ServiceProvider.GetRequiredService<EBaoService>();
var printService = scope.ServiceProvider.GetRequiredService<PrintService>();
var userService = scope.ServiceProvider.GetRequiredService<UserService>();


var sw = Stopwatch.StartNew();

await userService.AuthenticateAsync();

var certificates = await ReadCertificatesFromFileAsync("ApolicesReprint.txt");

if (certificates.Length == 0)
{
    Log.Warning("Não há apólices para reprint.");

    return;
}

var count = 0;
foreach (var policy in certificates)
{
    try
    {
        count++;
        Log.Logger.Information("*** {count} de {total} Iniciando reprint da Apolice {policy} no eBao.", count, certificates.Length, policy);

        var result = await printService.PrintProcessAndSendEmailAsync(policy);

        Log.Logger.Information("*** Reprint da Apolice {policy} realizado no eBao com sucesso.", policy);

    }
    catch (Exception ex)
    {
        Log.Error(ex, "*** Erro ao enviar fazer o reprint da Apolice {policy} no eBao.", policy);
    }
}

Log.Information("Tempo de processamento: {Elapsed}", sw.Elapsed);

await ebaoService.LogoutAsync();



static string GetApplicationDirectory()
{
    //Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    // Option 1: Use the location of the executing assembly
    var path = System.AppContext.BaseDirectory;

    if (!string.IsNullOrEmpty(path))
        return path;

    return AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
}


static async Task<string[]> ReadCertificatesFromFileAsync(string fileName)
{
    string filePath = Path.Combine(GetApplicationDirectory(), fileName);

    try
    {
        string[] certificates = await File.ReadAllLinesAsync(filePath);
        Log.Information("Certificates loaded from file: {FilePath}", filePath);

        return certificates.Where(c => !string.IsNullOrWhiteSpace(c))
                           .Select(c => c.Trim())
                           .Distinct()
                           .ToArray();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to read certificates from file: {FilePath}", filePath);
        return Array.Empty<string>();
    }
}