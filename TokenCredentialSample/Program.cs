﻿using Azure.Core;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;

namespace TokenCredentialSample
{
    class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
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
