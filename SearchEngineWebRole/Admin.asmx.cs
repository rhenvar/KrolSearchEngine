using KrolCloudLibrary;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace SearchEngineWebRole
{
    /// <summary>
    /// Summary description for Admin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Admin : System.Web.Services.WebService
    {
        private static CloudTable urls = AccountManager.tableClient.GetTableReference("urltable");
        private static CloudTable diagnostics = AccountManager.tableClient.GetTableReference("diagnostictable");
        private static Dictionary<string, Tuple<List<string>, DateTime>> cache;
        public static string SourceFile = "clean_viewcount.txt";
        public static Trie Data;
        public static List<string> UserHistory;

        [WebMethod]
        public string DownloadBlob()
        {
            try
            {
                BlobManager.DownloadFile(SourceFile);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "Successfully Downloaded File";
        }

        [WebMethod]
        public string BuildTree()
        {
            try
            {
                using (Stream stream = new FileStream(System.IO.Path.GetTempPath() + "\\" + SourceFile, FileMode.Open, FileAccess.Read))
                    Data = new Trie(stream);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "built tree!";
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public void Suggestions(string input)
        {
            Dictionary<string, bool> resultsDict = new Dictionary<string, bool>();
            var jsonSerialiser = new System.Web.Script.Serialization.JavaScriptSerializer();

            string[] results = Data.GetSuggestions(input.ToLower());
            if (results != null)
            {
                foreach (string result in results)
                {
                    resultsDict[result] = false;
                }
            }

            if (UserHistory != null)
            {
                foreach (string value in UserHistory)
                {
                    if (value.IndexOf(input, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        resultsDict[value.ToLower()] = true;
                    }
                }
            }
            HttpContext.Current.Response.Write(jsonSerialiser.Serialize(resultsDict));
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public int GetSize()
        {
            return Data.Size;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Search(string input)
        {
            if (UserHistory == null)
            {
                UserHistory = new List<string>();
            }

            bool contains = false;
            foreach (string value in UserHistory)
            {
                if (value == input)
                {
                    contains = true;
                }
            }
            if (!contains)
            {
                UserHistory.Add(input);
            }
        }


        // should only be reading data from azure table here
        // don't invoke worker or insert urls
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void StartCrawling()
        {
            CloudQueue resumeQueue = AccountManager.queueClient.GetQueueReference("resumequeue");
            resumeQueue.AddMessage(new CloudQueueMessage(""));
        }

        [WebMethod]
        public bool ParseRobots()
        {
            using (var client = new WebClient())
            {
                CloudQueue xmlQueue = AccountManager.queueClient.GetQueueReference("xmlqueue");
                xmlQueue.CreateIfNotExists();
                xmlQueue.AddMessage(new CloudQueueMessage("http://bleacherreport.com/sitemap/nba.xml"));

                byte[] cnnData = client.DownloadData("http://www.cnn.com/robots.txt");
                ParseRobots(cnnData);
            }
            return true;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void StopCrawling()
        {
            CloudQueue stopQueue = AccountManager.queueClient.GetQueueReference("stopqueue");
            CloudQueueMessage stopMessage = new CloudQueueMessage("");
            stopQueue.AddMessage(stopMessage);
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void ClearQueue()
        {
            StopCrawling();
            Thread.Sleep(1000);
            CloudQueue queue = AccountManager.queueClient.GetQueueReference("myurls");
            queue.CreateIfNotExists();
            CloudQueueMessage message = queue.GetMessage();
            while (message != null)
            {
                message = queue.GetMessage();
                queue.DeleteMessage(message);
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void ClearIndex()
        {
            StopCrawling();
            Thread.Sleep(1000);
            urls.DeleteAsync();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetPageTitle(string url)
        {
            string key = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url));
            TableQuery<WebPageEntity> query = new TableQuery<WebPageEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key));
            foreach (WebPageEntity page in urls.ExecuteQuery(query))
            {
                return page.Title;
            }
            return "No such page found!";
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public List<string> GetResults(string partial)
        {
            if (cache == null)
            {
                cache = new Dictionary<string, Tuple<List<string>, DateTime>>();
            }
            else
            {
                if (cache.ContainsKey(partial))
                {
                    return cache[partial].Item1;
                }
            }

            string[] partialSplit = partial.ToLower().Split(new char[] { ' ', '_' });
            List<string> results = new List<string>();
            List<TitlePartialEntity> entities = new List<TitlePartialEntity>();
            foreach (string partKey in partialSplit)
            {
                string cleanKey = new string(partKey.Where(x => Char.IsLetter(x) || Char.IsWhiteSpace(x) || Char.IsNumber(x)).ToArray()).ToLower();
                TableQuery<TitlePartialEntity> query = new TableQuery<TitlePartialEntity>().Where(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, cleanKey));

                foreach (TitlePartialEntity result in urls.ExecuteQuery(query))
                {
                    entities.Add(result);
                }
            }

            // a
            var filtered = entities.Select(x => new Tuple<string, string>(x.Title, x.Url)).OrderByDescending(x => InstancesOfPartial(partial, x.Item1)).Distinct().Distinct().Take(20).ToList();
            List<string> filteredList = filtered.Select(x => x.Item1 + "||" + x.Item2).ToList();
            cache.Add(partial, Tuple.Create<List<string>, DateTime>(filteredList, DateTime.Now.Date));
            return filteredList;
        }

        private int InstancesOfPartial(string partial, string title)
        {
            int total = 0;
            foreach (string word in partial.Split(new char[] { ' ', '_' }))
            {
                if (title.Contains(word))
                {
                    total++;
                }
            }
            return total;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public DiagnosticEntity GetData()
        {
            DiagnosticEntity returnEntity = new DiagnosticEntity("", 0, 0, new string[0], new string[0], 0, 0, 0, new Dictionary<string, string>());
            if (Data != null)
            {
                returnEntity.TrieLastTitle = Data.LastTitle;
                returnEntity.TrieSize = Data.Size;
            }
            TableQuery<DiagnosticEntity> query = new TableQuery<DiagnosticEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, "0"));
            var entities = diagnostics.ExecuteQuery(query);
            foreach (DiagnosticEntity dE in entities)
            {
                returnEntity.CpuUsage = dE.CpuUsage;
                returnEntity.MemUsage = dE.MemUsage;
                returnEntity.ErrorUrls = dE.ErrorUrls;
                returnEntity.LastCrawled = dE.LastCrawled;
                returnEntity.RoleStats = dE.RoleStats;
                returnEntity.IndexSize += dE.IndexSize;
                returnEntity.QueueSize += dE.QueueSize;
                returnEntity.TotalCrawled += dE.TotalCrawled;
            }
            return returnEntity;
        }

        private void ParseRobots(byte[] data)
        {
            // NEED TO PARSE DISALLOW FIRST SO CRAWLING DOESN'T START WITHOUT KNOWING 
            // WHAT IS BLACKLISTED
            CloudQueue forbiddenQueue = AccountManager.queueClient.GetQueueReference("forbiddenqueue");
            forbiddenQueue.CreateIfNotExists();

            using (StreamReader reader = new StreamReader(new MemoryStream(data)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains("Disallow"))
                    {
                        string[] testLine = line.Split(' ');

                        string disallowExtension = testLine[1];
                        forbiddenQueue.AddMessage(new CloudQueueMessage("cnn.com" + disallowExtension));
                    }
                }
            }

            CloudQueue xmlQueue = AccountManager.queueClient.GetQueueReference("xmlqueue");
            xmlQueue.CreateIfNotExists();
            using (StreamReader reader = new StreamReader(new MemoryStream(data)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains("Sitemap"))
                    {
                        string[] testLine = line.Split(' ');

                        if (testLine[1].Contains(".xml"))
                        {
                            xmlQueue.AddMessage(new CloudQueueMessage(testLine[1]));
                        }
                    }
                }
            }
        }
    }
}
