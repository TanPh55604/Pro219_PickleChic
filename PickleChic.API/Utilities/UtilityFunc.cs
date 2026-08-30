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
            MailjetClient client = new MailjetClient("d6c5e816cacd0645137110f9f3401997", "6a39d9d6578839fd30be7993fc049d4b");
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
               .Property(Send.FromEmail, "hlk9@proton.me")
               .Property(Send.FromName, "PickleChic")
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

            const string upperChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijkmnopqrstuvwxyz";
            const string digitChars = "23456789";
            const string specialChars = "!@#$%^*_+-=";
            const string allChars = upperChars + lowerChars + digitChars + specialChars;

            using var rng = RandomNumberGenerator.Create();
            char Pick(string source)
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var index = BitConverter.ToUInt32(bytes, 0) % (uint)source.Length;
                return source[(int)index];
            }

            var result = new char[count];
            var guaranteed = Math.Min(count, 4);
            if (guaranteed >= 1) result[0] = Pick(upperChars);
            if (guaranteed >= 2) result[1] = Pick(lowerChars);
            if (guaranteed >= 3) result[2] = Pick(digitChars);
            if (guaranteed >= 4) result[3] = Pick(specialChars);

            for (int i = guaranteed; i < count; i++)
            {
                result[i] = Pick(allChars);
            }

            for (int i = result.Length - 1; i > 0; i--)
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var j = (int)(BitConverter.ToUInt32(bytes, 0) % (uint)(i + 1));
                (result[i], result[j]) = (result[j], result[i]);
            }

            return new string(result);
        }

    }
}
