using System.Net;
using Ebao.V2.DPEM.Api.Auth;
using Ebao.V2.DPEM.Api.Binding;
using Ebao.V2.DPEM.Api.Broker;
using Ebao.V2.DPEM.Api.Client;
using Ebao.V2.DPEM.Api.Coverage;
using Ebao.V2.DPEM.Api.Data;
using Ebao.V2.DPEM.Api.Digital;
using Ebao.V2.DPEM.Api.Handlers;
using Ebao.V2.DPEM.Api.Payment;
using Ebao.V2.DPEM.Api.Print;
using Ebao.V2.DPEM.Api.Quotation;
using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Services;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Serilog;

namespace Ebao.V2.DPEM;

public static class ServicesRegister
{
    public static string Session = "FIXED";

    public static ServiceProvider BuildServiceProvider()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.RegisterServices();
        return collection.BuildServiceProvider();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddRefit();
        services.AddServices();
        services.AddScoped(_ => new RequestInfo(Session));
        services.AddSerilog();
    }

    private static void AddRefit(this IServiceCollection services)
    {
        var eBaoUrl = Config.IntegrationConfig.EbaoUrl;

        services.AddTransient<RequestHandler>();
        services.AddTransient<BindingHandler>();
        services.AddTransient<AuthHandler>();

        var handler = new HttpClientHandler()
        {
            UseCookies = true,
            CookieContainer = Config.CookieContainer
        };

        services.AddRefitClient<IStartApi>(new RefitSettings
        {
            HttpMessageHandlerFactory = () =>
            {
                handler.CookieContainer = new CookieContainer();
                return handler;
            }
        })
            .ConfigurePrimaryHttpMessageHandler<AuthHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));

        services.AddRefitClient<IAuthApi>(new RefitSettings
        {
            HttpMessageHandlerFactory = () =>
            {
                handler.CookieContainer = new CookieContainer();
                return handler;
            }
        })
            .ConfigurePrimaryHttpMessageHandler<AuthHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));

        services.AddRefitClient<IQuotationApi>()
            .AddHttpMessageHandler<RequestHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));

        services.AddRefitClient<IBrokerApi>()
            .AddHttpMessageHandler<RequestHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));

        services.AddRefitClient<IClientApi>()
            .AddHttpMessageHandler<RequestHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));

        services.AddRefitClient<IDataApi>()
            .AddHttpMessageHandler<RequestHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));

        services.AddRefitClient<ICoverageApi>()
            .AddHttpMessageHandler<RequestHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));

        services.AddRefitClient<IPaymentApi>()
            .AddHttpMessageHandler<RequestHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));

        services.AddRefitClient<IPrintApi>()
            .AddHttpMessageHandler<RequestHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(eBaoUrl));
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<UserService>();
        services.AddScoped<QuotationService>();
        services.AddScoped<BrokerService>();
        services.AddScoped<DataService>();
        services.AddScoped<ClientService>();
        services.AddScoped<CoverageService>();
        services.AddScoped<PaymentService>();
        services.AddScoped<PrintService>();
        services.AddScoped<EBaoService>();
        services.AddScoped<PrintService>();

        services.AddMemoryCache();
    }
}