using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VPNLib;

namespace NetworkLib
{
    public class RestAnswer
    {

        public List<string> Question;
        public List<string> Answer;
    }
    public class RestAnswers
    {
        public List<RestAnswer> rezult = new List<RestAnswer>();
    }

    public class RestCommander
    {


        public RestCommander()
        {


        }


        public JObject GET(string url, string token)
        {
            //  HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.url+url);
            //  request.Method = "GET";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            // client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            try
            {
                //  WebResponse response = request.GetResponse();
                //      HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                //    HttpResponseMessage response2 = client.GetAsync().Result;
                //    using (Stream responseStream = response.GetResponseStream())
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {

                    var dataObjects = response.Content.ReadAsStringAsync().Result;// .ReadAsAsync<IEnumerable<DataObject>>().Result;
                    try
                    {
                        return JsonDataSerializer.ToJObiect(dataObjects);
                    }
                    catch
                    {
                        return JsonDataSerializer.ToJObiect(dataObjects);
                    }
                }
                else
                {
                    throw new Exception("Error " + response.StatusCode + " " + response.ReasonPhrase);
                }

            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();

                    throw new Exception(ex + " " + errorText);
                }
                throw ex;
            }
        }

        public string POST(string url, string Content)
        {

            /***********************************************************/
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = @"application/x-www-form-urlencoded";
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(Content);

            request.ContentLength = byteArray.Length;


            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            long length = 0;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                { //  response.ContentType = @"application/x-www-form-urlencoded";
                    length = response.ContentLength;
                    Stream resultStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(resultStream, Encoding.GetEncoding("utf-8"));
                    string responseString = reader.ReadToEnd();
                    return responseString;
                }
            }
            catch (WebException ex)
            {

                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();

                    throw new Exception(ex + " " + errorText);
                }
                throw ex;

            }

        }



    }
}
