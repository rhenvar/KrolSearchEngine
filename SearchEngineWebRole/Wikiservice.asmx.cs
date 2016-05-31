using KrolCloudLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace SearchEngineWebRole
{
    /// <summary>
    /// Wikiservice ASMX  
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Wikiservice : System.Web.Services.WebService
    {
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
    }
}
