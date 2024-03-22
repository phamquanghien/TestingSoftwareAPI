using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TestingSoftwareAPI.Models.Process
{
    public class StringProcess
    {
        public string RemoveAccents(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormKD);
            Regex regex = new Regex("[\\p{M}]");
            string result = regex.Replace(normalizedString, "").ToLower().Trim();
            result = result.Replace("đ","d");
            result = result.Replace("  "," ");
            return result;
        }
        public static string Sha256Encryption(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}