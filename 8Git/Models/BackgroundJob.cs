using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace _8Git
{
    public class BackgroundJob
    {
        public long starttime = 0;
        public int maxEcecutionTime = 5; // seconds
        public CancellationTokenSource cts = null;
        public BackgroundWorker bw = null;
        public CancellationToken token = CancellationToken.None;
    }
}
