using System.Security.Cryptography;
using System.Text;
using Mailjet.Client;
using Mailjet.Client.Resources;
using System;
using Newtonsoft.Json.Linq;

namespace PickleChic.API.Utilities
{
    public class UtilityFunc
    {
        public string HashPassword(string password)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            md5.Clear();
            return sb.ToString();

        }

        public async Task<bool> SendEmailToAddress(string emailAddress,string nameRecivecer, string subject, string body, string bodyHTML)
        {
            MailjetClient client = new MailjetClient("76c6220ae1a01e1008c6f48c42e8ab3d", "9d3aedd2a7d39b8517775e5080b65031");
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
               .Property(Send.FromEmail, "hoangpkph38105@fpt.edu.vn")
               .Property(Send.FromName, "ITeam")
               .Property(Send.Subject, subject)
               .Property(Send.TextPart, body)
               .Property(Send.HtmlPart, bodyHTML)
               .Property(Send.Recipients, new JArray {
                new JObject {
                 {"Email",emailAddress}
                 }
                   });
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                return false;
            }
        }

        public string GenerateRandomString(int count)
        {
            if (count < 1) return string.Empty;

            const string normalChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            Random random = new Random();
            char[] result = new char[count];

            result[0] = specialChars[random.Next(specialChars.Length)];

            for (int i = 1; i < count; i++)
            {
                result[i] = normalChars[random.Next(normalChars.Length)];
            }

            return new string(result.OrderBy(x => random.Next()).ToArray());
        }

    }
}
