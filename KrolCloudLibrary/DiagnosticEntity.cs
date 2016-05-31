using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KrolCloudLibrary
{
    [XmlRoot("DiagnosticEntity")]
    public class DiagnosticEntity : TableEntity
    {
        [XmlElement(ElementName = "CpuUsage")]
        public string CpuUsage { get; set; }
        [XmlElement(ElementName = "MemLeft")]
        public string MemUsage { get; set; }
        [XmlElement(ElementName = "LastCrawled")]
        public string LastCrawled { get; set; }
        [XmlElement(ElementName = "LastError")]
        public string ErrorUrls { get; set; }
        [XmlElement(ElementName = "TotalCrawled")]
        public int TotalCrawled { get; set; }
        [XmlElement(ElementName = "QueueSize")]
        public int QueueSize { get; set; }
        [XmlElement(ElementName = "IndexSize")]
        public int IndexSize { get; set; }
        [XmlElement(ElementName = "IndexDate")]
        public DateTime IndexDate { get; set; }
        [XmlElement(ElementName = "RoleStats")]
        public string RoleStats { get; set; }
        [XmlElement(ElementName = "TrieSize")]
        public int TrieSize { get; set; }
        [XmlElement(ElementName = "TrieLastTitle")]
        public string TrieLastTitle { get; set; }

        public DiagnosticEntity(float cpuUsage, float memUsage, string[] lastCrawled, string[] errorUrls, int totalCrawled, int queueSize, int indexSize, Dictionary<string, string> roleStats)
        {
            this.PartitionKey = Convert.ToString(DateTime.Now.Ticks);
            this.CpuUsage = Convert.ToString(cpuUsage);
            this.MemUsage = Convert.ToString(memUsage);
            this.LastCrawled = string.Join("|||", lastCrawled);
            this.ErrorUrls = string.Join("|||", errorUrls);
            this.TotalCrawled = totalCrawled;
            this.QueueSize = queueSize;
            this.IndexSize = indexSize;
            this.RoleStats = "";
            foreach (KeyValuePair<string, string> pair in roleStats)
            {
                RoleStats += pair.Key + "|" + pair.Value + "||";
            }
            this.IndexDate = DateTime.Now;
            this.RowKey = DateTime.Now.Ticks.ToString();
        }

        public DiagnosticEntity() { }
    }
}
