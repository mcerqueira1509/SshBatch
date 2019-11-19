using System;

namespace SshBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new SshBatchProcessor();
            Console.Write(p.ProcessParams(args));
        }
    }
}
