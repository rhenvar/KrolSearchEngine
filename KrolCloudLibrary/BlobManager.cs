using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrolCloudLibrary
{
    public static class BlobManager
    {
        static CloudStorageAccount storageAccount;
        static CloudBlobClient blobClient;
        static CloudBlobContainer container;

        static BlobManager()
        {
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=krolazurestorageaccount;AccountKey=N8cpG7qSHssOUt9swfrjnbo3i7m9ihCzNyWaDKTFJAl+Z/jPkUkahzeTUr8lxNWdpuM+vQqwwC5IKrqlVN6Pbw==");
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("info344");
        }

        public static void DownloadFile(string filename)
        {
            try
            {
                CloudBlockBlob blobTitles = container.GetBlockBlobReference(filename);
                if (File.Exists(System.IO.Path.GetTempPath() + "\\" + filename))
                {
                    File.Delete(System.IO.Path.GetTempPath() + "\\" + filename);
                }

                blobTitles.DownloadToFile(System.IO.Path.GetTempPath() + "\\" + filename, FileMode.CreateNew);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
