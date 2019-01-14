using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Tools
{
    class RestClient
    {
        public static async Task<bool> LoginAndGetSession(string username, string pass, HttpClient client)
        {

            string test = "{\"username\":\"" + username + "\",\"password\":\"" + pass + "\"}";
            var buffer = System.Text.Encoding.UTF8.GetBytes(test);
            var byteContent = new ByteArrayContent(buffer);
            var response = await client.PostAsync("http://examonitoring.ap.be/api/users/checkUser", byteContent);

            string result = await response.Content.ReadAsStringAsync();
            if (result.ToLower().Contains("false"))
            {
                MessageBox.Show("wrong credentials");
                return false;
            }
            else
            {
                bool gotcookie = await TrySetSessionCookie(client, response);
                return gotcookie;
            }
            
        }

        public static async Task GetActiveSessions(HttpClient client)
        {
            var authc = await client.GetStringAsync("http://examonitoring.ap.be/api/users/authCheck");
            if (authc.Contains("true"))

            {
                var res3 = await client.GetStringAsync(new Uri("http://examonitoring.ap.be/api/sessions/getActiveSessions"));
                MessageBox.Show(res3);
            }
            else
                MessageBox.Show("Not logged in anymore");
        }

        public static async Task<bool> TrySetSessionCookie(HttpClient client, HttpResponseMessage response)
        {
            try
            {


                string result = await response.Content.ReadAsStringAsync();

                var res1 = response.Headers.Where(p => p.Key == "Set-Cookie").First().Value.First();
                string cookie = res1.Split(';')[0].Split('=').Last();


                client.DefaultRequestHeaders.Add("Cookie", $"ci_session={cookie}");
                return true;
            }
            catch (Exception)
            {
                //TODO:No cookie found
            }
            return true;
        }
    }
}
