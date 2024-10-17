using Ebao.V2.DPEM.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ebao.V2.DPEM.Services;

public class ServiceBase<TService>
{
    public ServiceBase(ILogger<TService> logger, IMemoryCache cache, RequestInfo requestInfo)
    {
        Logger = logger;
        Cache = cache;
        RequestInfo = requestInfo;
    }

    protected ILogger<TService> Logger { get; set; }
    protected IMemoryCache Cache { get; set; }
    protected RequestInfo RequestInfo { get; set; }
}