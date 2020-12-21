namespace ConfigurationManager.Parsers
{
    public interface ISerrializer
    {
        string Serialize(object obj);
    }
}