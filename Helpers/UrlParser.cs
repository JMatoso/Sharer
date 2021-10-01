using System;
using System.Text;

namespace Sharer.Helpers
{
    public class UrlParser
    {
        public string Base64Encode(string plainText) 
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

        public string Base64Decode(string base64EncodedData)
            => Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
    }
}