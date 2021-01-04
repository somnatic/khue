using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using RestSharp;


namespace KHueHelper
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {

            string json;

            try
            {
                var restDiscoveryClient = new RestClient("https://discovery.meethue.com/")
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                    Timeout = -1
                };
                var restDiscoveryRequest = new RestRequest(Method.GET);
                IRestResponse restDiscoveryResponse = restDiscoveryClient.Execute(restDiscoveryRequest);
                json = restDiscoveryResponse.Content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't find Hue Bridge @ https://discovery.meethue.com/; error message: {ex.Message}");
                Console.ReadLine();
                return;
            }

            var hueBridgeInfo = JsonSerializer.Deserialize<JsonClasses.HueBridgeInfo[]>(json);
            if (hueBridgeInfo.Length < 1)
            {
                Console.WriteLine($"Couldn't find Hue Bridge @ https://discovery.meethue.com/; retrieved empty list");
                Console.ReadLine();
                return;
            }

            foreach (var info in hueBridgeInfo)
            {
                Console.WriteLine($"Hue Bridge: Id={info.Id}, Internal IP Address: {info.InternalIPAddress}");
            }


            Console.WriteLine();
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1) Register new user");
            Console.WriteLine("2) List lights");
            var input = Console.ReadLine();

            if (input == "1")
            {
                Console.WriteLine("Please press the connect button on the hue bridge. Then, press <Enter>");

                bool connectionSuccess = false;

                var restAuthClient = new RestClient(new Uri($"https://{hueBridgeInfo[0].InternalIPAddress}/api"))
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                    Timeout = -1
                };
                var restAuthRequest = new RestRequest(Method.POST);
                restAuthRequest.AddParameter("application/json", "{\"devicetype\":\"khue#khue\"}", ParameterType.RequestBody);


                while (!connectionSuccess)
                {
                    IRestResponse restAuthResponse = restAuthClient.Execute(restAuthRequest);

                    var stringResponse = restAuthResponse.Content;
                    var authResponse = JsonSerializer.Deserialize<JsonClasses.HueBridgeAuthRoot[]>(stringResponse);
                    if (authResponse.Length < 1)
                    {
                        Console.WriteLine($"Didn't receive a proper response to the user creation request: {stringResponse}, retrying in 5s");
                        Thread.Sleep(5000);
                    }

                    if (authResponse[0].Error != null)
                    {
                        Console.WriteLine($"Error from user creation request: {authResponse[0].Error.Description}, retrying in 5s");
                        Thread.Sleep(5000);
                    }

                    if (authResponse[0].Success != null)
                    {
                        Console.WriteLine($"Success: User Created = {authResponse[0].Success.Username}");
                        File.WriteAllText("user.txt", authResponse[0].Success.Username);
                        connectionSuccess = true;
                    }

                }

                Console.ReadLine();


            }
            else if (input == "2")
            {
                string username;

                if (File.Exists("user.txt"))
                {
                    username = File.ReadAllText("user.txt");
                }
                else
                {
                    Console.Write("Please enter username: ");
                    username = Console.ReadLine();
                }

                var restListClient =
                    new RestClient(new Uri($"https://{hueBridgeInfo[0].InternalIPAddress}/api/{username}/lights"))
                    {
                        RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                        Timeout = -1
                    };
                var restListRequest = new RestRequest(Method.GET);
                IRestResponse restListResponse = restListClient.Execute(restListRequest);

                var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(restListResponse.Content);
                foreach (var entry in dictionary)
                {
                    var element = JsonSerializer.Deserialize<Dictionary<string, object>>(entry.Value.ToString());
                    Console.WriteLine($"Id={entry.Key}: {element["name"]} ({element["productname"]})");
                }

            }





        }
    }
}
