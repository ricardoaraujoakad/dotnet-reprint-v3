using System.Net;
using Ebao.V2.DPEM.Api.Client;
using Ebao.V2.DPEM.Api.Client.Requests;
using Ebao.V2.DPEM.Api.Client.Responses;
using Ebao.V2.DPEM.Api.Data.Responses;
using Ebao.V2.DPEM.Helpers;
using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PhoneNumbers;

namespace Ebao.V2.DPEM.Services;

public class ClientService : ServiceBase<ClientService>
{
    private readonly UserService _userService;
    private readonly IClientApi _clientApi;
    private readonly DataService _dataService;

    public ClientService(ILogger<ClientService> logger, IMemoryCache cache, RequestInfo requestInfo,
        IClientApi clientApi, UserService userService, DataService dataService) : base(logger, cache, requestInfo)
    {
        _clientApi = clientApi;
        _userService = userService;
        _dataService = dataService;
    }

    public async ValueTask<V3ClientResponse?> TryGetUserAsync(string identity)
    {
        Logger.LogInformation("Verificando se o assegurado {Identity} já existe.", identity);

        var isCompany = identity.Length == 14;
        var request = new V3SearchByIdentityRequest(identity, isCompany ? "company" : "ind");
        request.Syskey = _userService.GetSyskeyRequestToken();
        var pageHtml = await _clientApi.FindClientAsync(request);

        if (pageHtml.Contains("does not exist!');"))
            return null;

        var insured = GetClientInfoFromPage(pageHtml, identity);

        if (insured == null)
            Logger.LogInformation("O assegurado não existe.");
        else
            Logger.LogInformation("O assegurado já existente. Id: {InsuredId}.", insured.Id);

        return insured;
    }

    public async ValueTask<V3ClientResponse> CreateClientAsync(InsuredRequest insuredInfo)
    {
        Logger.LogInformation("Efetuando criação do usuário.");

        var (areaCode, number) = GetPhoneInfo(insuredInfo.Phone);


        //
        // try
        // {
        //     address = await _dataService.GetAddressInfoByCepAsync(insuredInfo.Address.Cep);
        // }
        // catch (Exception ex)
        // {
        //     Logger.LogError(ex, "Erro ao buscar informações de endereço pelo CEP.");
        //     address = new V3AddressResponse();
        // }
        //

        V3AddressResponse address = new V3AddressResponse
        {
            City = insuredInfo.Address.City,
            Complement = insuredInfo.Address.Complement,
            District = insuredInfo.Address.Neighborhood,
            Number = insuredInfo.Address.Number,
            Street = insuredInfo.Address.Street,
            ZipCode = insuredInfo.Address.Cep,
            StateId = _dataService.GetStateCd(insuredInfo.Address.State)
        };

        string pageHtml;

        if (insuredInfo.Identity.Length == 11)
        {
            var request =
                new V3CreateClientRequest(insuredInfo, areaCode, number, address, _userService.GetSyskeyRequestToken());

            Logger.LogInformation("Enviando usuário para criação.");
            pageHtml = await _clientApi.CreateClientAsync(request);
        }
        else
        {
            var request =
                new V3CreateCompanyRequest(insuredInfo, areaCode, number, address,
                    _userService.GetSyskeyRequestToken());

            Logger.LogInformation("Enviando usuário para criação.");
            pageHtml = await _clientApi.CreateClientAsync(request);
        }


        return GetClientInfoFromPage(pageHtml, insuredInfo.Identity)!;
    }

    public async ValueTask AssignOrCreateUserQuotationAsync(InsuredRequest insuredInfo)
    {
        Logger.LogInformation("Atribuindo o assegurado a cotação.");
        var client = await TryGetUserAsync(insuredInfo.Identity) ?? await CreateClientAsync(insuredInfo);
        
        Logger.LogInformation("Id do usuário: {ClientId}.", client.Id);
        
        client.Address.City = insuredInfo.Address.City;
        client.Address.Complement = insuredInfo.Address.Complement;
        client.Address.District = insuredInfo.Address.Neighborhood;
        client.Address.Number = insuredInfo.Address.Number;
        client.Address.Street = insuredInfo.Address.Street;
        client.Address.ZipCode = insuredInfo.Address.Cep;
        client.Address.StateId = _dataService.GetStateCd(insuredInfo.Address.State);

        var request = new V3AssignClientRequest(client, _userService.GetSyskeyRequestToken());
        await _clientApi.AssignClientQuotationAsync(request);

        Logger.LogInformation("Assegurado atribuído a cotação.");
    }

    private V3ClientResponse? GetClientInfoFromPage(string pageHtml, string identity)
    {
        Logger.LogInformation("Efetuando leitura das informações do usuário página HTML.");
        var reader = new HtmlReader();
        reader.Load(pageHtml);

        var prefix = identity.Length == 11 ? "I" : "C";

        Logger.LogInformation("Prefixo do usuário: {Prefix}.", prefix);
        var exists = pageHtml.Contains($"{prefix}CUST");

        if (!exists)
            return null;

        var selectedPartyId = reader.GetValueByName("selectedPartyId");
        long id;

        if (!selectedPartyId.IsNullOrEmpty())
            id = long.Parse(selectedPartyId!);
        else
            id = long.Parse(identity.Length == 11
                ? reader.GetValueByNameOrThrow("indInsured")
                : reader.GetValueByNameOrThrow("orgInsured"));


        var street = WebUtility.HtmlDecode(reader.GetValueByName("address04"));
        var city = WebUtility.HtmlDecode(reader.GetValueByName("address01"));

        var stateLv = reader.GetValueByName("regionLv2");

        if (!int.TryParse(stateLv, out var stateId))
        {
            Logger.LogWarning("Não foi possível obter o stateid para valor {State}.", stateLv);
            stateId = 0;
        }

        var district = WebUtility.HtmlDecode(reader.GetValueByName("address02"));
        var zipCode = WebUtility.HtmlDecode(reader.GetValueByName("address07"));
        var complement = WebUtility.HtmlDecode(reader.GetValueByName("address06"));
        var number = reader.GetValueByName("address05");

        var address = new V3AddressResponse(stateId, street, district, city, zipCode, number, complement);
        return new V3ClientResponse(id, identity, address);
    }

    private static (int areaCode, ulong phone) GetPhoneInfo(string phone)
    {
        var instance = PhoneNumberUtil.GetInstance();
        var parsed = instance.Parse(phone, "BR");

        if (parsed.CountryCode != 55)
            throw new ArgumentException("O número de telefone deve ser do Brasil.");

        var nationalNumber = instance.GetNationalSignificantNumber(parsed);
        var areCodeSize = instance.GetLengthOfNationalDestinationCode(parsed);

        var areaCode = int.Parse(nationalNumber[..areCodeSize]); //Extrai o DDD do número
        var number = ulong.Parse(nationalNumber[areCodeSize..]);

        return (areaCode, number);
    }
}