using Ebao.V2.DPEM.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ebao.V2.DPEM.Services;

public class UserService : ServiceBase<UserService>
{
    public string GetSyskeyRequestToken()
    {
        var user = Cache.Get<UserInfo>(RequestInfo.Auth);

        if (user == null)
            throw new NullReferenceException("User not found.");

        return user.Syskey;
    }

    public UserService(ILogger<UserService> logger, IMemoryCache cache, RequestInfo requestInfo) : base(logger, cache,
        requestInfo)
    {
    }
}