using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace SIMS
{
    public static class Helper
    {
        static string APIURL = Environment.GetEnvironmentVariable("api") ?? "http://localhost:8888";
        
        public static string URL_getToken = $"{APIURL}/AuthService?username=%1&password=%2";
        public static string URL_checkToken = $"{APIURL}/AuthService/check?username=%1&token=%2";
        
        private static string STOP_API = Environment.GetEnvironmentVariable("stopFunctionApi") ?? "http://localhost:8888";
        private static string STOP_KEY = Environment.GetEnvironmentVariable("stopFunctionKey") ?? "%1%2%3%4";
        private static string URL_STOP_FUNCTION = $"{STOP_API}?code={STOP_KEY}&resourceId=%1";
        public static string getToken(string username, string password)
        {
            RestClient client = new RestClient(Helper.URL_getToken.Replace("%1", username).Replace("%2", password));
            RestRequest request = new RestRequest("", Method.Post);
            RestResponse response = client.Execute(request);
            string token = response.Content ?? "";
            token = token.Replace("\"", "");
            return token;
        }

        public static bool checkToken(string username, string token)
        {
            RestClient client = new RestClient(Helper.URL_checkToken.Replace("%1", username).Replace("%2", token));
            RestRequest request = new RestRequest("", Method.Get);
            RestResponse response = client.Execute(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK ? true : false;
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) { builder.Append(bytes[i].ToString("x2")); }
                return builder.ToString();
            }
        }

        public static bool StopResource(string resourceId)
        {
            RestClient client = new RestClient(Helper.URL_STOP_FUNCTION.Replace("%1", resourceId));
            RestRequest request = new RestRequest("", Method.Get);
            RestResponse response = client.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }

}
