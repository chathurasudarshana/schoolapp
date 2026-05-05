namespace SCH.Shared.Utility
{
    public interface IJsonUtility : IUtility
    {
        string Serialize<T>(T obj);

        string SerializeNonFormattingIncludeNullValue<T>(T obj);

        T? Deserialize<T>(string json);

        T? DeserializeIgnoreNullValue<T>(string json);
    }
}
