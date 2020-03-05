using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace TestCredentialSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Sending out values");
            await GetUsingAzureCoreInteractiveCredential();
            Console.ReadKey();
        }
        private static async Task GetUsingAzureCoreInteractiveCredential()
        {
            string clientId = "d662ac70-7482-45af-9dc3-c3cde8eeede4";
            string[] scopes = new[] { "User.Read", "Mail.ReadWrite" };

            InteractiveBrowserCredential myBrowserCredential = new InteractiveBrowserCredential(clientId);
            TokenCredentialAuthProvider tokenCredentialAuthProvider = new TokenCredentialAuthProvider(myBrowserCredential, scopes);

            //Try to get something from the Graph!!
            HttpClient httpClient = GraphClientFactory.Create(tokenCredentialAuthProvider);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

            //Print out the response :)
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);
        }

        private static async Task TestWithIntergratedWindowsAuth()
        {
            string clientId = "d662ac70-7482-45af-9dc3-c3cde8eeede4";
            string authority = "https://login.microsoftonline.com/organizations/";
            string[] scopes = new[] { "User.Read", "Mail.ReadWrite" };

            IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .Build();

            IntegratedWindowsTokenCredential integratedWindowsTokenCredential = new IntegratedWindowsTokenCredential(publicClientApplication);
            TokenCredentialAuthProvider tokenCredentialAuthProvider = new TokenCredentialAuthProvider(integratedWindowsTokenCredential, scopes);

            //Try to get something from the Graph!!
            HttpClient httpClient = GraphClientFactory.Create(tokenCredentialAuthProvider);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

            //Print out the response :)
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);


        }
    }
}
