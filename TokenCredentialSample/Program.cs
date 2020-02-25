using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Azure.Identity;

namespace TokenCredentialSample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // //Internal credential
            // await GetUseInternalInteractiveTokenCredential();
            // await GetUsingAzureCoreInteractiveCredential();
            //
            // //Username password credential
            // await GetUseUsernamePasswordCredential();
            //
            // //DeviceCodeCredential credential
            // await GetClientSecretCredentialCredential();
            //
            // //ClientSecretCredentials credential
            // await GetDeviceCodeCredential();
            //
            // //ClientSecretCredentials credential
            // await GetDeviceCodeCredential();
            //
            // //Authorization Code credential
            // await GetAuthorizationCodeCredential();

            // Client Secret with certificate
            // await GetClientSecretCredentialCredentialWithCertificate();
        }

        private static async Task GetAuthorizationCodeCredential()
        {
            string[] scopes = new[] { "https://graph.microsoft.com/.default" };
            string clientId = "317bd2d8-58b7-4be6-b5bc-d5567a6df8db";

            //TODO needs to be refreshed
            string authCode = "OAQABAAIAAABeAFzDwllzTYGDLh_qYbH8-fS3d_J-R8zEdrSFxP3SMigriWF5cdAiV5KkrWu2E_M5m-OBUZBftegjZymsS3dgFA1ZGorEmSwMjlRzrcyRDOMatyEspA8QnFy7-84aZIMGKPPaQ4FF6g2Ll5J4Jewk0lEKjBkWo1IY8Eja_kly0kuZDgOJyGao_5VJRJYFdcDRXOkwattPyY2v6MeL5dsRxTqzBUducnBA9D54jOkbxVehxLzyYaF7DWNC7teei-PzJ-DOhgAkiuIbtbDObFYvmQDnOLwwxvf3PRdQS_xqw79TxdFKNFMIbuwjIhtS-e_FjClLMZcHohrs11FcWo-fuTwMoQt14HbD9gt0aaxgCgy8CaLH7tJnDyDYGfJTriq1FXC1S76iWgxj_30teP9Ul01vliD1Rmi8hGiejHP2zN0J5RE3HGToDMnGLCbHFYGDiAM5Ju9L6o4QuijuI1UY2059RPKtwjy6P5eBJPUdROv8D7Qm9jSmy6pYH8IYPeVg1l1C6ALAgzDl3Q2RU0v37-i3xhBXz-ZpWQrXVXreeeZz5z5HS1oo28VuBsMV4KMHgIslQZ2vJw4XI4-EUyCfw6avx3Cgv2G22BtqyPGBi5Fm2nkaORdnxQcsp6OxpGvjSksSFmjV8F-12KmdEif_0__rtz7t2dmQrZ6Hg12uNSAA";
            AuthorizationCodeCredential authorizationCodeCredential = new AuthorizationCodeCredential("9cacb64e-358b-418b-967a-3cabc2a0ea95",
                clientId,
                "ahYBc9/Nejqg=b@GzXvo[2xlGgLHIq59",
                authCode);
            TokenCredentialAuthProvider tokenCredentialAuthProvider = new TokenCredentialAuthProvider(authorizationCodeCredential, scopes);

            //Try to get something from the Graph!!
            HttpClient httpClient = GraphClientFactory.Create(tokenCredentialAuthProvider);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

            //Print out the response :)
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);
        }

        private static async Task GetDeviceCodeCredential()
        {
            string[] scopes = new[] { "User.Read" };
            string clientId = "cdc858be-9aaa-4339-94e1-86414d05a056";
            string expectedCode = "This is a test";

            DeviceCodeCredential deviceCodeCredential =new DeviceCodeCredential((code, cancelToken) => VerifyDeviceCode(code, expectedCode), 
                "9cacb64e-358b-418b-967a-3cabc2a0ea95" , clientId);
            TokenCredentialAuthProvider tokenCredentialAuthProvider = new TokenCredentialAuthProvider(deviceCodeCredential, scopes);
        
            //Try to get something from the Graph!!
            HttpClient httpClient = GraphClientFactory.Create(tokenCredentialAuthProvider);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        
            //Print out the response :)
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);
        }

        private static Task VerifyDeviceCode(DeviceCodeInfo code, string message)
        {
            Console.WriteLine(code.Message);
            return Task.CompletedTask;
        }

        private static async Task GetClientSecretCredentialCredential()
        {
            string[] scopes = new[] { "https://graph.microsoft.com/.default" };//see https://stackoverflow.com/questions/51781898/aadsts70011-the-provided-value-for-the-input-parameter-scope-is-not-valid/51789899

            ClientSecretCredential clientSecretCredential = new ClientSecretCredential("9cacb64e-358b-418b-967a-3cabc2a0ea95", "cdc858be-9aaa-4339-94e1-86414d05a056", "AI6ju0@Jn4ECkg1rv[QOrW_.hn4_VD26");
            TokenCredentialAuthProvider tokenCredentialAuthProvider = new TokenCredentialAuthProvider(clientSecretCredential, scopes);
            
            //Try to get something from the Graph!!
            HttpClient httpClient = GraphClientFactory.Create(tokenCredentialAuthProvider);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/users/admin@m365x638680.onmicrosoft.com/");
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
            
            //Print out the response :)
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);
        }

        private static async Task GetClientSecretCredentialCredentialWithCertificate()
        {
            string[] scopes = new[] { "https://graph.microsoft.com/.default" };//see https://stackoverflow.com/questions/51781898/aadsts70011-the-provided-value-for-the-input-parameter-scope-is-not-valid/51789899

            // ClientSecretCredential clientSecretCredential = new ClientSecretCredential("9cacb64e-358b-418b-967a-3cabc2a0ea95", "cdc858be-9aaa-4339-94e1-86414d05a056", "AI6ju0@Jn4ECkg1rv[QOrW_.hn4_VD26");
            
            X509Certificate2 x509Certificate2 = new X509Certificate2("mycert.pfx", "Nemore11");
            ClientCertificateCredential clientCertificateCredential = new ClientCertificateCredential("9cacb64e-358b-418b-967a-3cabc2a0ea95", "317bd2d8-58b7-4be6-b5bc-d5567a6df8db", x509Certificate2);
            TokenCredentialAuthProvider tokenCredentialAuthProvider = new TokenCredentialAuthProvider(clientCertificateCredential, scopes);

            //Try to get something from the Graph!!
            HttpClient httpClient = GraphClientFactory.Create(tokenCredentialAuthProvider);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/users/admin@m365x638680.onmicrosoft.com/");
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

            //Print out the response :)
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);
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

        private static async Task GetUseInternalInteractiveTokenCredential()
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
