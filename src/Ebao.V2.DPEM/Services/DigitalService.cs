using Ebao.V2.DPEM.Api.Digital;
using Microsoft.Extensions.Caching.Memory;
using Ebao.V2.DPEM.Api.Digital.Requests;

namespace Ebao.V2.DPEM.Services;

public class DigitalService
{
    private readonly IAuthDigitalApi _digitalApi;
    private readonly ICheckoutApi _checkoutApi;
    private readonly IMemoryCache _memoryCache;
    private const string TokenCacheKey = "DigitalServiceAuthToken";

    public DigitalService(IAuthDigitalApi digitalApi, ICheckoutApi checkoutApi, IMemoryCache memoryCache)
    {
        _digitalApi = digitalApi;
        _checkoutApi = checkoutApi;
        _memoryCache = memoryCache;
    }

    // Método público que utiliza o token para enviar informações ao serviço digital
    public async Task SendInformationToDigitalAsync(string orderId, string certificate)
    {
        const int maxAttempts = 3;
        int attempt = 0;
        bool success = false;

        var request = new UpdateEbaoPolicyNumberRequest()
        {
            IdOrder = orderId,
            EbaoPolicyNumber = certificate
        };

        while (attempt < maxAttempts && !success)
        {
            attempt++;
            try
            {
                var token = await GetAuthTokenAsync();

                // Usa o token para autorizar a requisição ao serviço digital
                var response = await _checkoutApi.UpdateEbaoPolicyNumberAsync($"Bearer {token}",
                    Config.IntegrationConfig.DigitalSubscriptionKey, request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to update ebao policy number: {response.Error.Content}");
                }

                success = true; // Marca como sucesso se não houver exceção
            }
            catch (Exception ex)
            {
                if (attempt >= maxAttempts)
                {
                    // Lança a exceção após a última tentativa falhar
                    throw new Exception($"Failed to send information to digital after {maxAttempts} attempts.", ex);
                }

                // Opcional: Adicione um delay antes de tentar novamente
                await Task.Delay(1000); // Aguarda 1 segundo antes de tentar novamente
            }
        }
    }


    // Método privado que lida com a autenticação e cacheia o token por 40 minutos
    private async Task<string> GetAuthTokenAsync()
    {
        if (!_memoryCache.TryGetValue(TokenCacheKey, out string token))
        {
            // Se não houver um token no cache, solicita um novo token
            var authRequest = new TokenRequest
            {
                Username = Config.IntegrationConfig.DigitalLogin,
                Password = Config.IntegrationConfig.DigitalPassword,
            };

            var authResponse =
                await _digitalApi.GetTokenAsync(authRequest, Config.IntegrationConfig.DigitalSubscriptionKey);

            if (authResponse.IsSuccessStatusCode)
            {
                token = authResponse.Content.AccessToken;

                // Cacheia o token por 40 minutos
                _memoryCache.Set(TokenCacheKey, token, TimeSpan.FromMinutes(40));
            }
            else
            {
                throw new Exception("Failed to authenticate with the digital API");
            }
        }

        return token;
    }
}