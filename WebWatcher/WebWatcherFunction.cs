using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DiffLib;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

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

        public class WebDiff
        {
            public string Collection1 { get; set; } = String.Empty;
            public string Collection2 { get; set; } = String.Empty;
            public DiffOperation DiffOperation { get; set; }

            public WebDiff(DiffOperation diffOperation, string collection1, string collection2)
            {
                DiffOperation = diffOperation;
                Collection1 = collection1;
                Collection2 = collection2;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                WebDiff objresult = (WebDiff)obj;

                return
                    DiffOperation == objresult.DiffOperation &&
                    String.Equals(Collection1, objresult.Collection1) &&
                    String.Equals(Collection2, objresult.Collection2);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private static string _emailAddressTo;
        private static string _sendGridApiKey;
        private static string _blobConnectionString;
        private static string _containerName;

        [FunctionName("WebWatcherFunction")]
        public static void Run([TimerTrigger("0 */30 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string tenantId = System.Environment.GetEnvironmentVariable("TenantId", EnvironmentVariableTarget.Process);
            string clientId = System.Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = System.Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            string keyVaultUri = $"https://{System.Environment.GetEnvironmentVariable("KeyVaultUri", EnvironmentVariableTarget.Process)}.vault.azure.net/";

            var secretClient = new SecretClient(vaultUri: new Uri(keyVaultUri), credential: credential);// new DefaultAzureCredential());


            KeyVaultSecret blobConnectionSecret = secretClient.GetSecret("BlobConnectionString");
            KeyVaultSecret sendGridSecret = secretClient.GetSecret("SendGridApiKey");
            KeyVaultSecret emailSecret = secretClient.GetSecret("EmailTo");
            KeyVaultSecret containerSecret = secretClient.GetSecret("ContainerName");

            _blobConnectionString = blobConnectionSecret.Value;
            _sendGridApiKey = sendGridSecret.Value;
            _emailAddressTo = emailSecret.Value;
            _containerName = containerSecret.Value;

            List<Website> websites = ReadFromBlob(log);

            using (HttpClient client = new HttpClient())
            {
                log.LogInformation($"Checking {websites.Count} websites...");
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
                            List<WebDiff> diff = new List<WebDiff>();
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
                                            log.LogInformation($"{DateTime.Now}: {message}");

                                            //If the html is empty, then the entire page is an addition. 
                                            if (!String.IsNullOrEmpty(site.MostRecentHtmlContent) &&
                                                !String.IsNullOrEmpty(currentHtml))
                                            {
                                                File.WriteAllText(@"C:\Users\valueduser\Desktop\current.html", currentHtml);
                                                File.WriteAllText(@"C:\Users\valueduser\Desktop\previous.html", site.MostRecentHtmlContent);
                                                diff = GetDiff(site.MostRecentHtmlContent,
                                                    currentHtml, log);
                                            }

                                            site.MostRecentHtmlContent = currentHtml;
                                            if (diff.Count > 0)
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

        public static List<WebDiff> GetDiff(string previousHtml, string currentHtml, ILogger log)
        {
            List<WebDiff> diffCollection = new List<WebDiff>();

            try
            {
                string[] previousHtmlArr = previousHtml.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None);
                string[] currentHtmlArr = currentHtml.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None);


                if (previousHtmlArr.Length > 0 && currentHtmlArr.Length > 0)
                {
                    IEnumerable<DiffSection> allSections = Diff.CalculateSections(previousHtmlArr, currentHtmlArr);
                    IEnumerable<DiffElement<string>> elements = Diff.AlignElements(previousHtmlArr, currentHtmlArr, allSections, new StringSimilarityDiffElementAligner());

                    Func<string, string> filter = input =>
                        input.Replace(" ", Constants.nonBreakingSpace)
                            .Replace("&", "&amp;")
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;");

                    foreach (DiffElement<string> element in elements)
                    {
                        if (element.ElementFromCollection1.HasValue && element.ElementFromCollection2.HasValue)
                        {
                            DiffOperation prevDiffOperation = DiffOperation.Match;

                            switch (element.Operation)
                            {
                                case DiffOperation.Insert:
                                    diffCollection.Add(new WebDiff(element.Operation, String.Empty, filter(element.ElementFromCollection2.Value)));
                                    break;
                                case DiffOperation.Delete:
                                    diffCollection.Add(new WebDiff(element.Operation, filter(element.ElementFromCollection1.Value), String.Empty));
                                    break;
                                case DiffOperation.Replace:
                                case DiffOperation.Modify:
                                    int ii1 = 0;
                                    int ii2 = 0;
                                    IEnumerable<DiffSection> sections = Diff.CalculateSections(element.ElementFromCollection1.Value.ToCharArray(), element.ElementFromCollection2.Value.ToCharArray()).ToArray();
                                    foreach (var section in sections)
                                    {
                                        if (!section.IsMatch)
                                        {
                                            diffCollection.Add(new WebDiff(
                                                element.Operation,
                                                filter(element.ElementFromCollection1.Value.Substring(ii1, section.LengthInCollection1)),
                                                filter(element.ElementFromCollection2.Value.Substring(ii2, section.LengthInCollection2))
                                             ));
                                        }

                                        ii1 += section.LengthInCollection1;
                                        ii2 += section.LengthInCollection2;
                                    }

                                    break;
                                case DiffOperation.Match:
                                    if (element.ElementFromCollection1.Value.Equals(element.ElementFromCollection2.Value))
                                        break;
                                    else
                                    {
                                        log.LogError($"FALSE MATCH? '{element.ElementFromCollection1.Value}' at {element.ElementIndexFromCollection1} does not match '{element.ElementFromCollection2.Value}' at {element.ElementIndexFromCollection2}");
                                        break;
                                    }
                                default:
                                    log.LogError($"Something went wrong. {element.Operation}");
                                    break;
                            }
                            prevDiffOperation = element.Operation;
                        }
                    }
                    log.LogInformation("Done calculating diffs...");
                }
                else
                {
                    string errorText = $"Unable to calculate diff. Previous HTML split length: {previousHtmlArr.Length}. Current HTML split length: {currentHtmlArr.Length}";
                    log.LogError($"{ DateTime.Now}: {errorText}");
                }
            }
            catch (SystemException ex)
            {
                log.LogError($"{ DateTime.Now}: {ex.Message}");
            }
            return diffCollection;
        }

        private static string HashWebData(string data)
        {
            string hashedValue;
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(data);
                hashedValue = ByteArrayToString(md5.ComputeHash(inputBytes));
            }
            return hashedValue;
        }

        private static void WriteToBlob(ILogger log, List<Website> list)
        {
            CloudBlockBlob blob = GetBlobConnection(log);

            RootObject root = new RootObject { websites = list };

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
                CloudBlobContainer container = blobClient.GetContainerReference(_containerName);
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