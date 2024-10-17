using HandlebarsDotNet;

namespace Ebao.V2.DPEM.Helpers.Templates;

public class TemplateWriter
{
    private readonly TemplateInfo[] _templates;

    public TemplateWriter(params TemplateInfo[] templates)
    {
        _templates = templates;
    }

    public async ValueTask<string> GenerateAsync(object data)
    {
        var handle = Handlebars.Create();

        var source = _templates[0];

        foreach (var (key, name) in _templates[1..])
        {
            var xml = await ReadTemplateAsync(name);
            handle.RegisterTemplate(key, xml);
        }

        handle.Configuration.FormatterProviders.Add(new DateTimeFormatter());
        
        var template = handle.Compile(await ReadTemplateAsync(source.Name));
        return template(data);
    }

    private static async ValueTask<string> ReadTemplateAsync(string templateName)
    {
        var templatesPath = Path.Combine(Environment.CurrentDirectory, "Templates","Files"); 
        var templatePath = Path.Combine(templatesPath, templateName + ".xml");
        return await File.ReadAllTextAsync(templatePath);
    }
}