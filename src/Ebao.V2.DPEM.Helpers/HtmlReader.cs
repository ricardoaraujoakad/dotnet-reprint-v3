using Ebao.V2.DPEM.Helpers.Model;
using HtmlAgilityPack;

namespace Ebao.V2.DPEM.Helpers;

public class HtmlReader
{
    public HtmlDocument Document { get; private set; }

    public void Load(string html)
    {
        Document = new HtmlDocument();
        Document.LoadHtml(html);
    }

    public static HtmlReader CreateReader(string html)
    {
        var reader = new HtmlReader();
        reader.Load(html);
        return reader;
    }

    public string? GetValueByName(string value, string? defaultValue = "") =>
        GetValueByAttribute("name", value, defaultValue);

    public HtmlNode? GetNodeByName(string value) => GetNodeFromXPath($"//*[@name='{value}']");

    public string[] GetValuesByName(string value) => GetNodesFromXPath($"//*[@name='{value}']")
        .Select(x => x.GetAttributeValue("value", null))
        .Where(x => x != null)
        .ToArray();

    public string GetValueByNameOrThrow(string value) => GetValueByAttributeOrThrow("name", value);

    public string? GetValueByAttribute(string attribute, string value, string? defaultValue = "") =>
        GetValueFromXPath($"//*[@{attribute}='{value}']") ?? defaultValue;

    public string GetValueByAttributeOrThrow(string tag, string value) =>
        GetValueFromXPathOrThrow($"//*[@{tag}=\"{value}\"]");

    public string? GetValueFromXPath(string xpath)
    {
        var node = GetNodeFromXPath(xpath);
        return node?.GetAttributeValue("value", null);
    }

    public string GetValueFromXPathOrThrow(string xpath)
    {
        var value = GetValueFromXPath(xpath);

        if (string.IsNullOrEmpty(value))
            throw new NullReferenceException($"{xpath} not found.");

        return value;
    }

    public HtmlNode? GetNodeFromXPath(string xpath) => Document.DocumentNode.SelectSingleNode(xpath);
    public HtmlNodeCollection GetNodesFromXPath(string xpath) => Document.DocumentNode.SelectNodes(xpath);

    public IEnumerable<SelectOption> ReadSelectOptions(string name)
    {
        return GetNodeFromXPath($"//*[@name='{name}']")!.SelectNodes("option")
            .Select(x => new
            {
                Key = x.GetAttributeValue("value", null),
                Value = x.GetAttributeValue("title", null),
                IsSelected = x.Attributes.Any(w => w.Name == "selected")
            })
            .Where(x => x.Key != "")
            .Select(s => new SelectOption(s.Key, s.Value, s.IsSelected));
    }
}