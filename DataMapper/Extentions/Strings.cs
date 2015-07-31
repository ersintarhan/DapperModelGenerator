#region

using System.Globalization;

#endregion

namespace DataMapper.Extentions
{
    public static class Strings
    {
        public static string ToLetterCase(this string s)
        {
            return new CultureInfo("en-GB").TextInfo.ToTitleCase(s.ToLower()).Replace("_", "").Replace("ı","i");
        }
    }
}