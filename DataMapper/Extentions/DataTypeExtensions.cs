namespace DataMapper.Extentions
{
    public class DataTypeExtensions
    {
        public static string GetNetString(string type, bool nullable)
        {
            if (nullable && type != "String" && type != "Byte[]")
            {
                return type + "?";
            }

            return type;
        }
    }
}