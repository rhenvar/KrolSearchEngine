using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrolCloudLibrary
{
    public static class CloudCounter
    {
        public static volatile int IndexSize;
        public static volatile int QueueSize;

        public static void IncrementQueue()
        {
            QueueSize++;
        }

        public static void IncrementIndex()
        {
            IndexSize++;
        }
    }
}
