using ChatMoa_DataBaseServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMoa
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DB_IO.RunAsync().GetAwaiter().GetResult();
        }
    }
}
