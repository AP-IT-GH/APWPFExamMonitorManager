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
        public static HttpClient Client;
        private static  string mainusername = "";
        private static string mainpassword = "";
        private static bool IAMIN = false;

        public static async void ReLogin()
        {
            try
            {
                await LoginAndGetSession(mainusername, mainpassword);
            }
            catch(Exception e)
            {
                MessageBox.Show("Relogin failed");
            }
        }

            public static async Task<bool> LoginAndGetSession(string username, string pass)
        {
            Client = new HttpClient();
            string test = "{\"username\":\"" + username + "\",\"password\":\"" + pass + "\"}";
            var buffer = System.Text.Encoding.UTF8.GetBytes(test);
            var byteContent = new ByteArrayContent(buffer);
            var response = await Client.PostAsync("http://examonitoring.ap.be/api/users/checkUser", byteContent);

            string result = await response.Content.ReadAsStringAsync();
            if (result.ToLower().Contains("false"))
            {
                //MessageBox.Show("wrong credentials");
                return false;
            }
            else
            {
                IAMIN = await TrySetSessionCookie(response);
                mainusername = username;
                mainpassword = pass;
                return IAMIN;
            }

        }

        public static async Task<string> GetActiveSessions()
        {
            var authc = await Client.GetStringAsync("http://examonitoring.ap.be/api/users/authCheck");
            if (authc.Contains("true"))

            {
                var res3 = await Client.GetStringAsync(new Uri("http://examonitoring.ap.be/api/sessions/getActiveSessions"));
                return res3;
                MessageBox.Show(res3);
            }
            else
            {
                //Retry login
                ReLogin();
                var res3 = await Client.GetStringAsync(new Uri("http://examonitoring.ap.be/api/sessions/getActiveSessions"));
                return res3;
            }
        }

        public static async Task CloseSession(string id)
        {
            

            var res = await Client.GetStringAsync(new Uri($"http://examonitoring.ap.be/api/sessions/finishSession/{id}"));
        }

        public static async Task<bool> TrySetSessionCookie(HttpResponseMessage response)
        {
            try
            {


                string result = await response.Content.ReadAsStringAsync();

                var res1 = response.Headers.Where(p => p.Key == "Set-Cookie").First().Value.First();
                string cookie = res1.Split(';')[0].Split('=').Last();


                Client.DefaultRequestHeaders.Add("Cookie", $"ci_session={cookie}");
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
