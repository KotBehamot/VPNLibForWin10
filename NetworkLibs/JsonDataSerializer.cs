using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;


namespace VPNLib
{
    public static class JsonDataSerializer
    {

        public static string ToJsonString(this object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static JObject ToJObiect(this String obj)
        {
            try
            {
                return JObject.Parse(obj);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public static JObject LoadJsonFromFile(string PATCH)
        {
            try
            {
                if (File.Exists(PATCH))
                {
                    return JObject.Parse(File.ReadAllText(PATCH));
                }
                else
                {
                    //doto load from web
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static void SaveJsonToFile(string PATCH, JObject obiect)
        {
            try
            {
                if (!File.Exists(PATCH))
                {
                    File.Create(PATCH);
                }
                string json = obiect.ToString();
                File.WriteAllText(PATCH, json);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


    }



}