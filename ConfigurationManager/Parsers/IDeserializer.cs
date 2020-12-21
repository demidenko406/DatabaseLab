namespace ConfigurationManager.Parsers
{
    public interface IDeserializer
    {
        T Deserialize<T>(string stringRepresentatoin);
    }
}