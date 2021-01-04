using System;
using System.IO;
using System.Net;
using System.Text;

namespace mail_thomaslinder_at.Logic.Nodes
{
    public static class KHueHttpClient
    {
        public static void ExecuteCommand(string jsonContent, string bridgeIpAddress, string username, int lightId)
        {
            var request = (HttpWebRequest)WebRequest.Create($"http://{bridgeIpAddress}/api/{username}/lights/{lightId}/state");
            request.Method = "PUT";
            request.ContentType = "application/json";

            using (var requestWriter = new StreamWriter(request.GetRequestStream(), Encoding.ASCII))
            {
                requestWriter.Write(jsonContent);
                requestWriter.Close();
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new KHueException($"Error when reading from Hue bridge: {ex.Message}");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                response.Close();
                throw new KHueException($"GetResponse returned {response.StatusCode.ToString()}");
            }

            using (var reader = new StreamReader(response.GetResponseStream() ?? Stream.Null, Encoding.UTF8))
            {
                var responseText = reader.ReadToEnd();
                if (!responseText.Contains("success"))
                {
                    response.Close();
                    throw new KHueException($"Reading the body content returned: {responseText}");
                }
            }

            // calls Dispose
            response.Close();
        }

    }
}
