using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    public static class ErrorCode
    {
        public const UInt32 NORMAL = 0x00000000;
        private const UInt32 DATA_PREPROCESSOR_OFFSET = 0x00010000;
        public const UInt32 DP_CURRENT_OUT_OF_RANGE = DATA_PREPROCESSOR_OFFSET + 0x00000001;
        public const UInt32 DP_TEMPERATURE_OUT_OF_RANGE = DATA_PREPROCESSOR_OFFSET + 0x00000002;
        public const UInt32 DP_VOLTAGE_OUT_OF_RANGE = DATA_PREPROCESSOR_OFFSET + 0x00000003;
        public const UInt32 DP_TIME_OUT_OF_RANGE = DATA_PREPROCESSOR_OFFSET + 0x00000004;
        public const UInt32 DP_ABNORMAL_STEP_CUTOFF = DATA_PREPROCESSOR_OFFSET + 0x00000005;
        public const UInt32 DP_CHECKSUM = DATA_PREPROCESSOR_OFFSET + 0x00000006;
        public const UInt32 DP_UNDEFINED = DATA_PREPROCESSOR_OFFSET + 0x00000007;
        public const UInt32 DP_STEP_MISSING = DATA_PREPROCESSOR_OFFSET + 0x00000008;
        public const UInt32 DP_NO_NEXT_STEP = DATA_PREPROCESSOR_OFFSET + 0x00000009;
        public const UInt32 DP_ACTION_MODE_MISMATCH = DATA_PREPROCESSOR_OFFSET + 0x0000000a;
        //public const UInt32 DP_CHECKSUM_ABNORMAL_STEP_JUMP = DATA_PREPROCESSOR_OFFSET + 0x0000000b;
        public const UInt32 DP_POWER_OUT_OF_RANGE = DATA_PREPROCESSOR_OFFSET + 0x0000000c;
        public const UInt32 DP_STEP_NO_JUMP = DATA_PREPROCESSOR_OFFSET + 0x0000000d;
        public const UInt32 DP_STEP_JUMP = DATA_PREPROCESSOR_OFFSET + 0x0000000e;
        public const UInt32 DP_TIME_JUMP = DATA_PREPROCESSOR_OFFSET + 0x0000000f;
        public const UInt32 DP_CYCLE_JUMP = DATA_PREPROCESSOR_OFFSET + 0x00000010;
        public const UInt32 DP_LOOP_JUMP = DATA_PREPROCESSOR_OFFSET + 0x00000011;
        public const UInt32 DP_MODE_JUMP = DATA_PREPROCESSOR_OFFSET + 0x00000012;
        public const UInt32 DP_DISCHARGE_VOLTAGE_JUMP_IN_RC_OCV = DATA_PREPROCESSOR_OFFSET + 0x00000013;
        public const UInt32 UNDEFINED = 0xffffffff;
        public static string GetDescription(UInt32 errorcode, params object[] args)
        {
            switch(errorcode)
            {
                case ErrorCode.DP_CURRENT_OUT_OF_RANGE:
                    return "Current Out Of Range.";
                case ErrorCode.DP_POWER_OUT_OF_RANGE:
                    return "Power Out Of Range.";
                case ErrorCode.DP_TEMPERATURE_OUT_OF_RANGE:
                    return "Temperature Out Of Range.";
                case ErrorCode.DP_VOLTAGE_OUT_OF_RANGE:
                    return "Voltage Out Of Range";
                case ErrorCode.DP_TIME_OUT_OF_RANGE:
                    return "Time Out Of Range.";
                case ErrorCode.DP_ABNORMAL_STEP_CUTOFF:
                    return "Abnormal step cut off.";
                case ErrorCode.DP_CHECKSUM:
                    return "Checksum error.";
                case ErrorCode.DP_UNDEFINED:
                    return "Undefined Data Processing Error.";
                case ErrorCode.DP_STEP_MISSING:
                    return $"Step {args[0]} is missing";
                case ErrorCode.DP_NO_NEXT_STEP:
                    return "Cannot Get Next Step.";
                case ErrorCode.DP_ACTION_MODE_MISMATCH:
                    return "Action mode does not match.";
                case ErrorCode.DP_STEP_NO_JUMP:
                    return "Step No Jump.";
                case ErrorCode.DP_STEP_JUMP:
                    return "Step Jump.";
                case ErrorCode.DP_TIME_JUMP:
                    return "Time Jump.";
                case ErrorCode.DP_CYCLE_JUMP:
                    return "Cycle Jump.";
                case ErrorCode.DP_LOOP_JUMP:
                    return "Loop Jump.";
                case ErrorCode.DP_MODE_JUMP:
                    return "Mode Jump.";
                case ErrorCode.DP_DISCHARGE_VOLTAGE_JUMP_IN_RC_OCV:
                    return "Discharge volage jump in RC OCV test.";
                default:
                    return "Undefined Error Code";
            }
        }
    }
}
