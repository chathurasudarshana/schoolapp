namespace SCH.Shared.Utility
{
    using Newtonsoft.Json;

    internal class JsonUtility: IJsonUtility
    {

        string IJsonUtility.Serialize<T>(T obj)
        {
            return Serialize(obj);
        }

        string IJsonUtility.SerializeNonFormattingIncludeNullValue<T>(T obj)
        {
            return SerializeNonFormattingIncludeNullValue(obj);
        }

        T IJsonUtility.Deserialize<T>(string json)
        {
            return Deserialize<T>(json);
        }

        T IJsonUtility.DeserializeIgnoreNullValue<T>(string json)
        {
            return DeserializeIgnoreNullValue<T>(json);
        }

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string SerializeNonFormattingIncludeNullValue<T>(T obj)
        {
            return JsonConvert.SerializeObject(
                obj, 
                Newtonsoft.Json.Formatting.None, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        }

        public static T? Deserialize<T>(string json)
        {


            return JsonConvert.DeserializeObject<T>(json) ;
        }

        public static T? DeserializeIgnoreNullValue<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(
                json, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
