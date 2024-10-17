using System.Globalization;
using System.Text;

namespace Ebao.V2.DPEM;

public static class TableConverter
{
    public static readonly Dictionary<string, string> Activities = new(ComparerFactory.CreateComparer())
    {
        { "PAS", "PAS" },
        { "123", "PAS" },
        { "Xxx", "PAS" },
        { "Passeio", "PAS" },
        { "ESP", "ESP" },
        { "ESPORTE E RECREIO", "ESP" },
        { "PAC", "PAC" },
        { "CAR", "CAR" },
        { "REB", "REB" },
        { "OUT", "OUT" },
        { "PES", "PES" }
    };

    public static readonly Dictionary<string, int> UseOfVesselDict = new(ComparerFactory.CreateComparer())
    {
        { "esporte e embarcações miúdas", 1 },
        { "moto náutica", 2 },
        { "comercial pesca", 3 },
        { "comercial pesca e outros", 3 },
        { "comercial outros", 4 },
        { "comercial carga ou passageiro (até 100 pessoas)", 5 },
        { "comercial carga/passageiro 100", 5 },
        { "comercial carga/passageiro 100+", 6 },
        { "comercial carga ou passageiro (acima de 100 pessoas)", 6 },
        { "comercial carga ou passgeiro (acima de 100 pessoas)", 6 }
    };

    public static readonly Dictionary<string, int> TypeOfNavigationDict = new(ComparerFactory.CreateComparer())
    {
        { "LON", 1 },
        { "CAB", 2 },
        { "MAR", 3 },
        { "ABERTO", 3 },
        { "INT", 4 },
        { "123", 4 },
        { "XXX", 4 },
        { "NAVEGAÇÃO INTERIOR", 4 },
        { "APM", 5 },
        { "APP", 6 }
    };

    public static readonly Dictionary<string, int> PropulsionTypeDict = new(ComparerFactory.CreateComparer())
    {
        { "motor", 1 },
        { "vela", 2 },
        { "vela/motor", 3 },
        { "sem propulsão", 4 },
        { "Sem propulso", 4 }
    };

    public static readonly Dictionary<string, int> VesselTypeDict = new(ComparerFactory.CreateComparer())
    {
        { "alvarenga", 26 },
        { "anfíbia", 27 },
        { "apoio à manobra", 28 },
        { "apoio a rov", 29 },
        { "cábrea", 30 },
        { "caiaque", 31 },
        { "caique", 32 },
        { "carga geral", 33 },
        { "cisterna (fsu)", 34 },
        { "curral", 35 },
        { "dique flutuante", 36 },
        { "estimulador de poço", 37 },
        { "floating, production, storage and off-loading u", 38 },
        { "flotel", 39 },
        { "hidroavião", 40 },
        { "iate", 41 },
        { "lançador de linhas", 42 },
        { "laser", 43 },
        { "manuseio de âncora (ahts)", 44 },
        { "manuseio de linhas", 45 },
        { "multipurpose", 46 },
        { "oceanográfico", 47 },
        { "oceonagráfico", 47 },
        { "outros", 48 },
        { "passageiro", 49 },
        { "passageiro / carga geral", 50 },
        { "passageiro de alta velocidade (hsc passageiro)", 51 },
        { "pesquisa", 52 },
        { "petroleiro", 53 },
        { "plataforma auto-elevável", 54 },
        { "plataforma fixa", 55 },
        { "plataforma semi-submersível", 56 },
        { "porta-contentor", 57 },
        { "quebra-gelo", 58 },
        { "químico", 59 },
        { "químico e gaseiro", 60 },
        { "roll-on / roll-off passageiro (ferry boat)", 61 },
        { "sonda", 62 },
        { "supridor (supply)", 63 },
        { "tanque (transporte de granéis líquidos)", 64 },
        { "transporte de gases liquefeitos", 65 },
        { "transporte escolar", 66 },
        { "balsa", 1 },
        { "ferry", 1 },
        { "barcaça", 2 },
        { "barge", 2 },
        { "bote", 3 },
        { "boat", 3 },
        { "canoa", 4 },
        { "canoe", 4 },
        { "multicasco (catamarã, trimarã, tetramarã, etc)", 5 },
        { "catamaran", 5 },
        { "chata", 6 },
        { "boring", 6 },
        { "draga", 7 },
        { "dredge", 7 },
        { "empurrador", 8 },
        { "pusher", 8 },
        { "escuna", 9 },
        { "schooner", 9 },
        { "flutuante", 10 },
        { "floating", 10 },
        { "graneleiro", 11 },
        { "bulk carrier", 11 },
        { "hovercraft", 12 },
        { "jangada", 13 },
        { "raft", 13 },
        { "jet boat", 14 },
        { "jet ski", 14 },
        { "lancha", 15 },
        { "speedboat", 15 },
        { "moto-aquática/similar", 16 },
        { "ship", 17 },
        { "oil-oil", 18 },
        { "pesqueiro", 19 },
        { "fishing", 19 },
        { "rebocador", 20 },
        { "tug", 20 },
        { "roll-on / roll-off carga", 21 },
        { "ro-ro", 21 },
        { "saveiro", 22 },
        { "traineira", 23 },
        { "trawler", 23 },
        { "veleiro", 24 },
        { "sailboat", 24 },
        { "batelão", 25 }
    };
}

public static class ComparerFactory
{
    public static IEqualityComparer<string> CreateComparer()
    {
        return new CustomStringComparer();
    }
}

public class CustomStringComparer : IEqualityComparer<string>
{
    public bool Equals(string x, string y)
    {
        return StringComparer.NormalizeString(x) == StringComparer.NormalizeString(y);
    }

    public int GetHashCode(string obj)
    {
        return StringComparer.NormalizeString(obj).GetHashCode();
    }
}

public static class StringComparer
{
    public static string NormalizeString(string input)
    {
        // Remove diacríticos (acentos)
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // Remove espaços e transforma em minúsculas
        return stringBuilder.ToString().Replace(" ", "").ToLowerInvariant();
    }
}