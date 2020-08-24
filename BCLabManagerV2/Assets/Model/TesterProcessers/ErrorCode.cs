using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ErrorCode
    {
        public const UInt32 NORMAL = 0x00000000;
        private const UInt32 SECTION_CHROMA17200 = 0x00010000;
        public const UInt32 CHARGE_START_VOL = SECTION_CHROMA17200 + 0x01;
        public const UInt32 CHARGE_START_CUR = SECTION_CHROMA17200 + 0x02;
        public const UInt32 CHARGE_START_TEMP = SECTION_CHROMA17200 + 0x03;
        public const UInt32 CHARGE_MID_VOL = SECTION_CHROMA17200 + 0x04;
        public const UInt32 CHARGE_MID_CUR = SECTION_CHROMA17200 + 0x05;
        public const UInt32 CHARGE_MID_TEMP = SECTION_CHROMA17200 + 0x06;
        public const UInt32 CHARGE_END_VOL = SECTION_CHROMA17200 + 0x07;
        public const UInt32 CHARGE_END_CUR = SECTION_CHROMA17200 + 0x028;
        public const UInt32 CHARGE_END_TEMP = SECTION_CHROMA17200 + 0x09;
        public const UInt32 STEP_JUMP = SECTION_CHROMA17200 + 0x0a;
        public const UInt32 UNDEFINDED = 0xffffffff;
    }
}
