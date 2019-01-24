using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using DiffLib;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace WebWatcher
{
    public static class WebWatcherFunction
    {
        private class RootObject
        {
            public List<Website> websites { get; set; }
        }

        private class Website
        {
            public string Url;
            public string MostRecentHashValue;
            public string MostRecentHtmlContent;
            public int NumberOfFailedAttempts;
            public bool IsIgnored;

            public Website(string url)
            {
                Url = url;
                MostRecentHashValue = String.Empty;
                MostRecentHtmlContent = String.Empty;
                NumberOfFailedAttempts = 0;
                IsIgnored = false;
            }
        }

        private static string _emailAddressTo;
        private static string _sendGridApiKey;
        private static string _blobConnectionString;
        private static string endDiv = "</div>";
        private static string lightGreen = "#ccffcc";
        private static string lightRed = "#ffcccc";
        private static string nonBreakingSpace = "\x00a0";

        [FunctionName("WebWatcherFunction")]
        public static async void Run([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ReadSecretsFromKeyVault(log);

            List<Website> websites = ReadFromBlob(log);

            using (HttpClient client = new HttpClient())
            {
                foreach (Website site in websites)
                {
                    if (site.IsIgnored)
                    {
                        log.LogError($"Too many failed attempts or page was disabled for {site.Url}. Ignoring...");
                    }
                    else
                    {
                        //Detect and respect robots
                        string rootUrl = new Uri(site.Url, UriKind.Absolute).GetLeftPart(UriPartial.Authority);
                        using (HttpResponseMessage response = client.GetAsync(rootUrl + "/robots.txt").Result)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                log.LogInformation($"Detected robots.txt file on {rootUrl}");
                                //TODO: parse the contents of the file. and filter the urls 
                                //site.IsIgnored = true;
                                //continue;
                            }
                        }

                        log.LogInformation($"Site: {site.Url}. Old hash value: {site.MostRecentHashValue}.");
                        try
                        {
                            using (HttpResponseMessage response = client.GetAsync(site.Url).Result)
                            {
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    using (HttpContent content = response.Content)
                                    {
                                        string currentHtml = content.ReadAsStringAsync().Result;
                                        string hashedValue = HashWebData(currentHtml);
                                        if (!hashedValue.Equals(site.MostRecentHashValue))
                                        {
                                            string message =
                                                $"Site: {site.Url} has been modified. \nOld hash: {site.MostRecentHashValue} \nNew hash: {hashedValue}. \n\n";
                                            site.MostRecentHashValue = hashedValue;
                                            log.LogDebug($"{DateTime.Now}: {message}");

                                            //If the most recent html is empty, then the entire page is an addition. 
                                            if (!String.IsNullOrEmpty(site.MostRecentHtmlContent) &&
                                                !String.IsNullOrEmpty(currentHtml))
                                            {
                                                string htmlDiff = GetHtmlDiffFromPreviousContent(site.MostRecentHtmlContent,
                                                    currentHtml, log);
                                                if (!htmlDiff.Contains("nonce"))
                                                {
                                                    //if htmlDiff contains 'nonce', don't add it to the email body;
                                                    message += htmlDiff;
                                                }
                                            }

                                            site.MostRecentHtmlContent = currentHtml;

                                            SendEmail(log, site.Url, message);
                                        }
                                        else
                                        {
                                            log.LogInformation($"Nothing changed for site: {site.Url}");
                                        }
                                    }
                                }
                                else
                                {
                                    log.LogWarning($"Possible redirect detected for {site.Url} - Status Code {response.StatusCode}.");
                                    site.MostRecentHashValue = response.StatusCode.ToString();
                                }
                            }
                        }
                        catch (System.Net.WebException webException)
                        {
                            log.LogWarning($"Error: {webException}. {webException.Message}");
                            site.NumberOfFailedAttempts++;
                            if (site.NumberOfFailedAttempts >= 5)
                            {
                                site.IsIgnored = true;
                            }
                        }
                    }
                }
            }
            WriteToBlob(log, websites);
        }

        private static string GetHtmlDiffFromPreviousContent(string previousHtml, string currentHtml, ILogger log)
        {
            var htmlDiff = new StringBuilder();
            string[] previousHtmlArr = previousHtml.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
            string[] currentHtmlArr = currentHtml.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);

            if (previousHtmlArr.Length > 0 && currentHtmlArr.Length > 0)
            {
                IEnumerable<DiffSection> testsections = Diff.CalculateSections(previousHtmlArr, currentHtmlArr);
                IEnumerable<DiffElement<string>> elements = Diff.AlignElements(previousHtmlArr, currentHtmlArr, testsections, new StringSimilarityDiffElementAligner());

                htmlDiff.Append("<div style='font-family: courier;'>");

                Func<string, string> filter = input =>
                    input.Replace(" ", nonBreakingSpace).Replace("&", "&amp;").Replace("<", "&lt;")
                        .Replace(">", "&gt;");

                foreach (var element in elements)
                {
                    DiffOperation prevDiffOperation = DiffOperation.Match;
                    switch (element.Operation)
                    {
                        case DiffOperation.Insert:
                            htmlDiff.Append($"<div style='background-color: {lightGreen};'>+{nonBreakingSpace}" + filter(element.ElementFromCollection2.Value) + endDiv);
                            break;

                        case DiffOperation.Delete:
                            htmlDiff.Append($"<div style='background-color: {lightRed};'>-{nonBreakingSpace}" + filter(element.ElementFromCollection1.Value) + endDiv);
                            break;

                        case DiffOperation.Replace:
                        case DiffOperation.Modify:
                            var sections = Diff.CalculateSections(element.ElementFromCollection1.Value.ToCharArray(), element.ElementFromCollection2.Value.ToCharArray()).ToArray();
                            int ii1 = 0;
                            int ii2 = 0;
                            htmlDiff.Append($"<div>");
                            foreach (var section in sections)
                            {
                                if (!section.IsMatch)
                                {
                                    //If the previous section was a match, add some context
                                    if (prevDiffOperation == DiffOperation.Match && ii1 - 15 > 0)
                                    {
                                        htmlDiff.Append($"{ii1 - 15}:");

                                    }
                                    htmlDiff.Append($"<span style='background-color: {lightRed};'>" + filter(element.ElementFromCollection1.Value.Substring(ii1, section.LengthInCollection1)) + "</span>");
                                    htmlDiff.Append($"<span style='background-color: {lightGreen};'>" + filter(element.ElementFromCollection2.Value.Substring(ii2, section.LengthInCollection2)) + "</span>");

                                    //TODO: Add trailing context 
                                    htmlDiff.Append("<br>");
                                }

                                ii1 += section.LengthInCollection1;
                                ii2 += section.LengthInCollection2;
                            }
                            htmlDiff.Append(endDiv);
                            break;
                    }
                    prevDiffOperation = element.Operation;
                }
                htmlDiff.Append(endDiv);
            }
            else
            {
                string errorText = $"Unable to calculate diff. Previous HTML split length: {previousHtmlArr.Length}. Current HTML split length: {currentHtmlArr.Length}";
                log.LogError($"{ DateTime.Now}: {errorText}");
                htmlDiff.Append("Error: " + errorText);
            }
            return htmlDiff.ToString();
        }

        private static string HashWebData(string data)
        {
            string hashedValue;
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(data);
                hashedValue = ByteArrayToString(md5.ComputeHash(inputBytes));
            }
            return hashedValue;
        }

        private static void ReadSecretsFromKeyVault(ILogger log)
        {
            try
            {
                var clientId = System.Environment.GetEnvironmentVariable("ClientId");
                var clientSecret = System.Environment.GetEnvironmentVariable("ClientSecret");

                // Creating the Key Vault client
                var keyVault = new KeyVaultClient(async (authority, resource, scope) =>
                {
                    var authContext = new AuthenticationContext(authority);
                    var credential = new ClientCredential(clientId, clientSecret);
                    var token = await authContext.AcquireTokenAsync(resource, credential);
                    return token.AccessToken;
                });

                _blobConnectionString = keyVault.GetSecretAsync(System.Environment.GetEnvironmentVariable("KeyVaultUri"), "BlobConnectionString").Result.Value;
                _sendGridApiKey = keyVault.GetSecretAsync(System.Environment.GetEnvironmentVariable("KeyVaultUri"), "SendGridApiKey").Result.Value;
                _emailAddressTo = keyVault.GetSecretAsync(System.Environment.GetEnvironmentVariable("KeyVaultUri"), "EmailTo").Result.Value;
            }
            catch (Exception ex)
            {
                log.LogCritical($"Unable to retrieve secrets from key vault. {ex}");
            }
        }

        private static void WriteToBlob(ILogger log, List<Website> list)
        {
            CloudBlockBlob blob = GetBlobConnection(log);

            RootObject root = new RootObject();
            root.websites = list;
            string websitesElement = JsonConvert.SerializeObject(root);

            blob.UploadTextAsync(websitesElement);
        }

        private static List<Website> ReadFromBlob(ILogger log)
        {
            List<Website> retVal = new List<Website>();
            CloudBlockBlob blob = GetBlobConnection(log);

            string json = blob.DownloadTextAsync().Result;
            JObject parsedJson = JObject.Parse(json);
            IList<JToken> results = parsedJson["websites"].Children().ToList();
            foreach (JToken result in results)
            {
                Website website = result.ToObject<Website>();
                retVal.Add(website);
            }
            return retVal;
        }

        private static CloudBlockBlob GetBlobConnection(ILogger log)
        {
            CloudBlockBlob retval = null;
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_blobConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("webwatcher");
                retval = container.GetBlockBlobReference("websites.json");
            }
            catch (Exception ex)
            {
                log.LogCritical($"Error: {ex}. {ex.Message}");
            }
            return retval;
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            foreach (byte singleByte in bytes)
                result.Append(
                    singleByte.ToString("x2")
                );

            return result.ToString();
        }

        private static void SendEmail(ILogger log, string siteUrl, string messageBody)
        {
            try
            {
                var client = new SendGridClient(_sendGridApiKey);
                var from = new EmailAddress("test@example.com", "WebWatcher Notifications");
                var subject = $"WebWatcher: {siteUrl} has been modified";
                var to = new EmailAddress(_emailAddressTo);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, messageBody, messageBody);
                client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                log.LogError("Unable to send email: " + ex.Message);
            }
        }
    }
}