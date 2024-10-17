using System.Dynamic;
using System.Xml.Linq;
using Ebao.V2.DPEM.Api.Data;
using Ebao.V2.DPEM.Api.Data.Responses;
using Ebao.V2.DPEM.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ebao.V2.DPEM.Services;

public class DataService : ServiceBase<DataService>
{
    private readonly IDataApi _dataApi;

    public DataService(ILogger<DataService> logger, IMemoryCache cache, RequestInfo requestInfo, IDataApi dataApi) :
        base(logger, cache, requestInfo)
    {
        _dataApi = dataApi;
    }

    public async ValueTask<V3AddressResponse> GetAddressInfoByCepAsync(string cep)
    {
        Logger.LogInformation("Obtendo informações de endereço do cep {Cep}.", cep);
        var xml = await _dataApi.GetCepInfoAsync(cep);
        var doc = XDocument.Parse(xml);
        var jsonText = JsonConvert.SerializeXNode(doc);
        dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText)!;

        var address = dyn.address;
        var stateId = int.Parse(address.regionLv2);
        var city = address.address01.ToString();
        var district = address.address02.ToString();
        var street = address.address04.value.ToString();

        return new V3AddressResponse(stateId, street, district, city, cep, "", "");
    }

    public int GetStateCd(string state)
    {
        var states = new Dictionary<string, int>()
        {
            { "AC", 3001 }, // Acre
            { "AL", 3002 }, // Alagoas
            { "AP", 3003 }, // Amapá
            { "AM", 3004 }, // Amazonas
            { "BA", 3005 }, // Bahia
            { "CE", 3006 }, // Ceará
            { "DF", 3007 }, // Distrito Federal
            { "ES", 3008 }, // Espírito Santo
            { "GO", 3009 }, // Goiás
            { "MA", 3010 }, // Maranhão
            { "MT", 3011 }, // Mato Grosso
            { "MS", 3012 }, // Mato Grosso do Sul
            { "MG", 3013 }, // Minas Gerais
            { "PR", 3016 }, // Paraná
            { "PB", 3015 }, // Paraíba
            { "PA", 3014 }, // Pará
            { "PE", 3017 }, // Pernambuco
            { "PI", 3018 }, // Piauí
            { "RN", 3020 }, // Rio Grande do Norte
            { "RS", 3021 }, // Rio Grande do Sul
            { "RJ", 3019 }, // Rio de Janeiro
            { "RO", 3022 }, // Rondônia
            { "RR", 3023 }, // Roraima
            { "SC", 3024 }, // Santa Catarina
            { "SE", 3026 }, // Sergipe
            { "SP", 3025 }, // São Paulo
            { "TO", 3027 } // Tocantins
        };

        return states[state];
    }
}