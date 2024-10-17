using HandlebarsDotNet;
using HandlebarsDotNet.IO;

namespace Ebao.V2.DPEM.Helpers.Templates;

public class DateTimeFormatter : IFormatter, IFormatterProvider
{
    public void Format<T>(T value, in EncodedTextWriter writer)
    {
        if (value is not DateTime dateTime)
            throw new ArgumentException("Invalid date!");

        writer.Write($"{dateTime:yyyyMMddHHmmss}");
    }

    public bool TryCreateFormatter(Type type, out IFormatter formatter)
    {
        if (type != typeof(DateTime))
        {
            formatter = null;
            return false;
        }

        formatter = this;
        return true;
    }
}