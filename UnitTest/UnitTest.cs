using BCLabManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class UnitTest
    {
        [Theory]
        [InlineData(
            @"D:\Issues\Open\BC_Lab\SW Design\BCLabManagerV2\BCLabManagerV2\bin\Debug\Local Data\M26\O2Sim1\Table Maker Product\20210823151713\LG_M26_2568mAhr_RC_stage2.txt",
            @"D:\Issues\Open\BC_Lab\SW Design\BCLabManagerV2\BCLabManagerV2\bin\Debug\Local Data\",
            4)]
        public void GetRelativeLevelShouldWork(String path, String str, int expected)
        {
            int actual = FileTransferHelper.GetRelativeLevel(path, str);
            Assert.Equal(expected, actual);
        }
    }
}
