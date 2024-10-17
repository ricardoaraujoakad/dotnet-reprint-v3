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


var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
path = Path.Combine(path, "Bots");

if (!Directory.Exists(path))
    Directory.CreateDirectory(path);

var lockDirectory = new DirectoryInfo(path);

Console.WriteLine($"Cross-lock path: {path}.", path);

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var allConfigsFiles = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Configs"), "*.json");

FileDistributedLockHandle? userHandle = null;

foreach (var configFilePath in allConfigsFiles)
{
    Console.WriteLine($"Trying to read config file: {configFilePath}.");

    var @lock = new FileDistributedLock(lockDirectory, configFilePath);
    userHandle = await @lock.TryAcquireAsync();

    if (userHandle == null)
    {
        Console.WriteLine($"Já existe outro processo com o arquivo de configuração {configFilePath}. Obtendo outro");
        continue;
    }

    Log.Information($"Reading config file: {configFilePath}.");
    var configFile = await File.ReadAllTextAsync(configFilePath);
    Config.IntegrationConfig = JsonSerializer.Deserialize<IntegrationConfig>(configFile);
    break;
}

if (userHandle == null)
{
    Console.WriteLine("Não há bots disponíveis para emissão.");
    return;
}

var logPath = Path.Combine(Environment.CurrentDirectory, "Logs",
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

await using (userHandle)
{
    Log.Information("Usuário eBao: {user}.", Config.IntegrationConfig.EbaoLogin);
    Log.Information("URL eBao: {url}.", Config.IntegrationConfig.EbaoUrl);

    Log.Information("Usuário Binding: {user}.", Config.IntegrationConfig.BindingLogin);
    Log.Information("URL Biding: {url}.", Config.IntegrationConfig.BindingUrl);

    Log.Information("Usuário Digital: {user}.", Config.IntegrationConfig.DigitalLogin);
    Log.Information("URL Digital: {url}.", Config.IntegrationConfig.DigitalUrl);

    Console.Title = Config.IntegrationConfig.EbaoLogin;

    var provider = ServicesRegister.BuildServiceProvider();

    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

    int retry = 0;

    do
    {
        await using var scope = provider.CreateAsyncScope();

        var ebaoService = scope.ServiceProvider.GetRequiredService<EBaoService>();
        //var bindingService = scope.ServiceProvider.GetRequiredService<BindingService>();
        //var digitalService = scope.ServiceProvider.GetRequiredService<DigitalService>();

        Log.Information("Obtendo lista de propostas para envio.");

        retry++;
        var proposals = await bindingService.GetNextProposalAsync();

        if (proposals != null)
        {
            foreach (var proposal in proposals)
            {
                retry = 0;

                using var __ = LogContext.PushProperty("ProposalId", proposal.Id);

                var @lock = new FileDistributedLock(lockDirectory, proposal.Id.ToString());
                await using var handle = await @lock.TryAcquireAsync();

                if (handle == null)
                {
                    Log.Logger.Warning("Já existe outro processo efetuando a emissão da proposta {Proposal}.",
                        proposal.ProposalId);

                    continue;
                }

                var sw = Stopwatch.StartNew();

                (string Certificate, string OrderId) result = default;

                try
                {
                    result = await ebaoService.SendToEbaoAsync(proposal.XmlMessage);
                    await bindingService.UpdateProposalStatusAsync(proposal, sw.Elapsed, result.Certificate,
                        DateTime.Now.ConvertToTimeZone(), null);

                    Log.Logger.Information("Proposta {Proposal} enviada para o eBao com sucesso.", proposal.ProposalId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Erro ao enviar proposta {Proposal} para o eBao.", proposal.ProposalId);
                    await bindingService.UpdateProposalStatusAsync(proposal, sw.Elapsed, null, null, ex);
                }


                if (result != default)
                {
                    try
                    {
                        //Este trecho apenas funciona em produção
                        await digitalService.SendInformationToDigitalAsync(result.OrderId, result.Certificate);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Falha ao enviar para o digital a proposal {Proposal}.", proposal.ProposalId);
                    }
                }

                Log.Information("Tempo de processamento: {Elapsed}", sw.Elapsed);

                await ebaoService.LogoutAsync();
                Config.State = new object();
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        var secondsToAwait = Math.Min(retry * 2, 120);
        Log.Information("Aguardando {Seconds} segundos para nova tentativa de obter as apólices.", secondsToAwait);
        await Task.Delay(TimeSpan.FromSeconds(secondsToAwait));
    } while (true);
}