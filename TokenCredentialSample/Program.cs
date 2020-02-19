using Azure.Core;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Identity;

namespace TokenCredentialSample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            //Internal credential
            // await GetUseInternalInteractiveTokenCredential();
            // await GetUsingAzureCoreInteractiveCredential();
            //Username password credential
            await GetUseUsernamePasswordCredential();

        }

        private static async Task GetUseUsernamePasswordCredential()
        {
            string[] scopes = new[] { "User.Read"};
            string clientId = "cdc858be-9aaa-4339-94e1-86414d05a056";
            UsernamePasswordCredential usernamePasswordCredential = new UsernamePasswordCredential("admin@m365x638680.onmicrosoft.com", "X5u3qG9oaY",
                "9cacb64e-358b-418b-967a-3cabc2a0ea95", clientId);
            TokenCredentialAuthProvider tokenCredentialAuthProvider = new TokenCredentialAuthProvider(usernamePasswordCredential, scopes);

            //Try to get something from the Graph!!
            HttpClient httpClient = GraphClientFactory.Create(tokenCredentialAuthProvider);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

            //Print out the response :)
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);

        }

        public static async Task GetUsingAzureCoreInteractiveCredential()
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
        public static async Task GetUseInternalInteractiveTokenCredential()
        {
            string clientId = "d662ac70-7482-45af-9dc3-c3cde8eeede4";
            string[] scopes = new[] { "User.Read", "Mail.ReadWrite" };

            //Create the msal application
            IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                .Create(clientId).WithRedirectUri("http://localhost:1234")
                .Build();

            //Create the token credential 
            InteractiveMsalTokenCredential msalTokenCredential = new InteractiveMsalTokenCredential(publicClientApplication);

            //Pass the token credential to the AuthProvider
            TokenCredentialAuthProvider tokenCredentialAuthProvider = new TokenCredentialAuthProvider(msalTokenCredential, scopes);

            //TRy to get something from the Graph!!
            HttpClient httpClient = GraphClientFactory.Create(tokenCredentialAuthProvider);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

            //Print out the response :)
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);
        }
    }
}
