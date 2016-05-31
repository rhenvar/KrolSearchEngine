using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrolCloudLibrary
{
    public static class AccountManager
    {
        public static CloudStorageAccount storageAccount { get; private set; }
        public static CloudQueueClient queueClient { get; private set; }
        private static string connString = "DefaultEndpointsProtocol=https;AccountName=krolazurestorageaccount;AccountKey=N8cpG7qSHssOUt9swfrjnbo3i7m9ihCzNyWaDKTFJAl+Z/jPkUkahzeTUr8lxNWdpuM+vQqwwC5IKrqlVN6Pbw==";
        public static CloudTableClient tableClient;

        static AccountManager()
        {
            try
            {
                storageAccount = CloudStorageAccount.Parse(connString);
                queueClient = storageAccount.CreateCloudQueueClient();
                tableClient = storageAccount.CreateCloudTableClient();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
