using Ebao.V2.DPEM.Api.Data.Responses;

namespace Ebao.V2.DPEM.Api.Client.Responses;

public record V3ClientResponse(long Id, string Identity, V3AddressResponse Address);