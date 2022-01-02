using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static RTech.Demo.Utilities.WDMS;

namespace RTech.Demo.Utilities
{
    public class WebrequestService
    {
        WebClient wc = null;

        public string BaseUrl { get; private set; }
        public bool NoRequest { get; private set; }

        public WebrequestService(string baseUrl)
        {
            wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
           // wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wc.Headers.Add("Accept", "application/json");
            this.BaseUrl = baseUrl;
            
            this.NoRequest = RiddhaSession.EnableADMS==false;
            Log.SytemLog(RiddhaSession.EnableADMS.ToString());
        }
        public WebrequestService(string baseUrl,string contentType)
        {
            wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            //wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wc.Headers.Add("Accept", contentType);
            this.BaseUrl = baseUrl;

        }
        public async Task<T> Get<T>(string url)
        {
            //var factory =new Microsoft.Extensions.Logging.LoggerFactory();
            //var logger = new Microsoft.Extensions.Logging.Logger<Rest.RestClient>(factory);

            //Rest.RestClient rc = new Rest.RestClient(logger);
            //rc.GetAsync(url,"",);
            try
            {
                if(NoRequest)
                {
                    return DeserializeObject<T>("");
                }

                var handler = new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    MaxAutomaticRedirections = 100,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                var client = new HttpClient(handler);

                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri(BaseUrl);
                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
                string response = client.GetStringAsync(url).Result;
                T data = DeserializeObject<T>(response);
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }

            // var json = wc.DownloadString(url);


        }

        public async Task<T> Post<T>(string url, string jsonSerializeObject)
        {

            try
            {
                if (NoRequest)
                {
                    return DeserializeObject<T>("");
                }
                var handler = new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    MaxAutomaticRedirections = 100,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                var client = new HttpClient(handler);

                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri(BaseUrl);

                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
                var content = new StringContent(jsonSerializeObject.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response =  client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();

                    T data = DeserializeObject<T>(str);
                    return data;
                }
                else
                {
                    return DeserializeObject<T>("");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<T> PostNepalPolice<T>(string url, string jsonSerializeObject , string computercode)
        {

            string username = "pmisadmin";
            string password = "pmis@#2021";

            try
            {
                string body = "";
                // Create the web request

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
             
                request.Headers.Add("Cache-Control", "no-cache");
                request.Method = "POST";

                byte[] utf8bytes = Encoding.UTF8.GetBytes(jsonSerializeObject.ToString());

                byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
                request.ContentType = "application/json";

                var content = new StringContent(jsonSerializeObject.ToString(), Encoding.UTF8, "application/json");



                request.ContentLength = jsonSerializeObject.Length;
                var sw = new StreamWriter(request.GetRequestStream());
                sw.Write(jsonSerializeObject);
                sw.Close();
                request.ServicePoint.Expect100Continue = false;
                try
                {
                    try
                    {
                        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                        {
                            // Get the response stream
                            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                            body = reader.ReadToEnd();
                        }
                    }
                    catch (WebException wex)
                    {
                        StreamReader reader = new StreamReader(wex.Response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                        body = reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {

                }
                var result = DeserializeObject<T>(body);
                return result;
            }
            catch (Exception ex)
            {

                return DeserializeObject<T>("");
            }
        }

        public string SerializeObject<T>(T obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            return content;
        }

        public T DeserializeObject<T>(string obj)
        {
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj);
            return items;

        }
    }
}
