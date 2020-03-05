using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace TokenCredentialSample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                .Create("555f95bc-ea6e-4dae-b28d-a0fbd2bc5f24").WithRedirectUri("http://localhost:1234")
                .Build();

            AuthenticationResult authenticationResult = await publicClientApplication
                .AcquireTokenInteractive(new string[] {"User.Read", "Mail.Read", "Calendars.Read", "Notes.ReadWrite.All" }).ExecuteAsync();
            DelegateAuthenticationProvider authenticationProvider = new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", authenticationResult.AccessToken);

                return Task.FromResult(0);
            });

            // Create http GET request.
            HttpRequestMessage httpRequestMessage1 =
                new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");

            // Create http POST request.
            string jsonContent = "{" +
                                 "\"displayName\": \"My Notebook1\"" +
                                 "}";
            HttpRequestMessage httpRequestMessage2 = new HttpRequestMessage(HttpMethod.Post,
                "https://graph.microsoft.com/v1.0/me/onenote/notebooks")
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            // Create batch request steps with request ids.
            BatchRequestStep requestStep1 = new BatchRequestStep("1", httpRequestMessage1, null);
            BatchRequestStep requestStep2 = new BatchRequestStep("2", httpRequestMessage2, new List<string> { "1" });

            // Add batch request steps to BatchRequestContent.
            BatchRequestContent batchRequestContent = new BatchRequestContent();
            batchRequestContent.AddBatchRequestStep(requestStep1);
            batchRequestContent.AddBatchRequestStep(requestStep2);

            BaseClient baseClient = new BaseClient("https://graph.microsoft.com/v1.0/", authenticationProvider);
            var responses = await baseClient.Batch.Request().PostAsync(batchRequestContent);
            foreach (var responsConetHttpResponseMessagee in await responses.GetResponsesAsync())
            {
                if (responsConetHttpResponseMessagee.Value.Content != null)
                {
                    Console.WriteLine("Content for Request");
                    Console.WriteLine(await responsConetHttpResponseMessagee.Value.Content.ReadAsStringAsync());
                    Console.WriteLine();
                }
            }
        }



        private static async Task TestBasics(HttpClient httpClient)
        {
            // Create http GET request.
            HttpRequestMessage httpRequestMessage1 =
                new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");

            // Create http POST request.
            string jsonContent = "{" +
                                 "\"displayName\": \"My Notebook\"" +
                                 "}";
            HttpRequestMessage httpRequestMessage2 = new HttpRequestMessage(HttpMethod.Post,
                "https://graph.microsoft.com/v1.0/me/onenote/notebooks")
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            // Create batch request steps with request ids.
            BatchRequestStep requestStep1 = new BatchRequestStep("1", httpRequestMessage1, null);
            BatchRequestStep requestStep2 = new BatchRequestStep("2", httpRequestMessage2, new List<string> { "1" });

            // Add batch request steps to BatchRequestContent.
            BatchRequestContent batchRequestContent = new BatchRequestContent();
            batchRequestContent.AddBatchRequestStep(requestStep1);
            batchRequestContent.AddBatchRequestStep(requestStep2);

            // Send batch request with BatchRequestContent.
            HttpResponseMessage response = await httpClient.PostAsync("https://graph.microsoft.com/v1.0/$batch", batchRequestContent);

            // Handle http responses using BatchResponseContent.
            BatchResponseContent batchResponseContent = new BatchResponseContent(response);
            Dictionary<string, HttpResponseMessage> responses = await batchResponseContent.GetResponsesAsync();
            foreach (var responsConetHttpResponseMessagee in responses)
            {
                if (responsConetHttpResponseMessagee.Value.Content != null)
                {
                    Console.WriteLine("Content for Request");
                    Console.WriteLine(await responsConetHttpResponseMessagee.Value.Content.ReadAsStringAsync());
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            HttpResponseMessage httpResponse1 = await batchResponseContent.GetResponseByIdAsync("1");
            if (httpResponse1.Content != null)
            {
                Console.WriteLine("Content for Request 1");
                Console.WriteLine(await httpResponse1.Content.ReadAsStringAsync());
            }
            Console.WriteLine();
            HttpResponseMessage httpResponse2 = await batchResponseContent.GetResponseByIdAsync("2");
            if (httpResponse2.Content != null)
            {
                Console.WriteLine("Content for Request 2");
                Console.WriteLine(await httpResponse2.Content.ReadAsStringAsync());
            }

            string nextLink = await batchResponseContent.GetNextLinkAsync();
            Console.WriteLine("Next Link");
            Console.WriteLine(nextLink);
        }
    }
}
