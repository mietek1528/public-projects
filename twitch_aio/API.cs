using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace twitch_aio
{
    internal class API
    {
        public static bool follow(int channelid,string token)
        {
            dynamic obj = sendRequest("https://gql.twitch.tv/gql", "POST", token, "[{\"operationName\":\"FollowButton_FollowUser\",\"variables\":{\"input\":{\"disableNotifications\":false,\"targetID\":\""+channelid+"\"}},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"800e7346bdf7e5278a3c1d3f21b2b56e2639928f86815677a7126b093b2fdd08\"}}},{\"operationName\":\"AvailableEmotesForChannel\",\"variables\":{\"channelID\":\"ch_id\",\"withOwner\":true},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"b9ce64d02e26c6fe9adbfb3991284224498b295542f9c5a51eacd3610e659cfb\"}}}]");
            if (!obj.ToString().Contains("ERROR") & JArray.Parse(obj)[0]["data"]["followUser"]["error"] == null) return true;
            else return false;
        }
        public static int getUserByName(string name)
        {
            try
            {
                dynamic result = sendRequest("https://gql.twitch.tv/gql", "POST", "m1vwh2ots9y8rrdok839vun4ui99d4", "[{\"operationName\":\"SearchResultsPage_SearchResults\",\"variables\":{\"query\":\"user\",\"options\":null,\"requestID\":\"e4f176dc-8f92-496f-82c4-11ef053afc81\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ee977ac21b324669b4c109be49ed3032227e8850bea18503d0ced68e8156c2a5\"}}}]".Replace("user", name));
                JArray obj = JArray.Parse(result);
                return Convert.ToInt32(obj[0]["data"]["searchFor"]["channels"]["edges"][0]["item"]["id"]);
            }catch (Exception) { return 0; }
        }
        private static UTF8Encoding encoding => new UTF8Encoding();
        private static object sendRequest(string url,string method,string token,object content)
        {
            try
            {
              HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/json";
                if (!string.IsNullOrEmpty(token)) request.Headers["Authorization"] = "OAuth " + token;
                request.Headers["Client-Id"] = "kimne78kx3ncx6brgo4mv6wki5h1ko";
                Stream stream = request.GetRequestStream();
                WriteContent(ref stream, content);
                stream.Close();
                dynamic response;
                ReadResponse(out response,request.GetResponse());
                return response;
            }
            catch (Exception e)
            {
                return "ERROR! " + e.Message;
            }
        }
        private static void ReadResponse(out dynamic content,WebResponse s)
        {
            StreamReader reader = new StreamReader(s.GetResponseStream());
            content = reader.ReadToEnd();
            reader.Close();
        }
        private static void WriteContent(ref Stream s,object content)
        {
            byte[] bytes = null;
            if (content is string)bytes = encoding.GetBytes(content.ToString());
            else bytes = encoding.GetBytes(JsonConvert.SerializeObject(content));
            s.Write(bytes, 0, bytes.Length);
            s.Flush();
        }
    }
}
