namespace Ebao.V2.DPEM.Helpers.Model;

public record KeyValue(string Key, string Value);
public record KeyValue<TKey, TValue>(TKey Key, TValue Value);