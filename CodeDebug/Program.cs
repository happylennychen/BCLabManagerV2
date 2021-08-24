using BCLabManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            int actual = FileTransferHelper.GetRelativeLevel(@"D:\Issues\Open\BC_Lab\SW Design\BCLabManagerV2\BCLabManagerV2\bin\Debug\Local Data\M26\O2Sim1\Table Maker Product\20210823151713\LG_M26_2568mAhr_RC_stage2.txt", @"D:\Issues\Open\BC_Lab\SW Design\BCLabManagerV2\BCLabManagerV2\bin\Debug\Local Data\");
            Console.WriteLine(actual.ToString());
        }
    }
}
