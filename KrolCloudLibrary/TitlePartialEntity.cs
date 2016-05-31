using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KrolCloudLibrary
{
    public class TitlePartialEntity : TableEntity
    {
        // Obama says YES! => URLA

        // Obama => URLA
        // says => URLA
        // YES! => URLA

        public string TitlePartial { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime SiteDate { get; set; }

        public TitlePartialEntity(string titlePartial, string url, string title, DateTime date)
        {
            string key = SanitizeTitlePartial(titlePartial);
            string titleKey = SanitizeTitlePartial(title);

            this.PartitionKey = key;
            this.RowKey = titleKey;
            this.Url = url;
            this.Title = title;
            this.SiteDate = date;
        }

        private string SanitizeTitlePartial(string url)
        {
            return new string(url.Where(x => Char.IsLetter(x) || Char.IsWhiteSpace(x) || Char.IsNumber(x)).ToArray()).ToLower();
        }

        public TitlePartialEntity() { }
    }
}
