using System;

namespace BCLabManager.Model
{
    public class DataRow
    {
        public UInt32 uSerailNum { get; set; }
        public float fVoltage { get; set; }
        public float fCurrent { get; set; }
        public float fTemperature { get; set; }
        public float fAccMah { get; set; }
        public DateTime dtRecord { get; set; }
        public float fSoCAdj { get; set; }

        public DataRow(UInt32 uN, float fV, float fC, float fT, float fAcc, string strDt, float fUnit = 1)
        {

            uSerailNum = uN;
            fVoltage = fV * fUnit;
            fCurrent = fC * fUnit;
            fTemperature = fT;
            fAccMah = fAcc * fUnit;
            if (fAccMah < -0.0001)
                fAccMah *= -1.0F;

            ConvertStringToDateTime(strDt);
        }

        private bool ConvertStringToDateTime(string strTime)
        {
            bool bReturn = false;
            int iSlash = 0;     //for date
            int iComm = 0;  //for time
            char[] chr = strTime.ToCharArray();
            string strtmp;
            int iYear = 0, iMonth = 0, iDay = 0, iHour = 0, iMinute = 0, iSecond = 0;

            strtmp = "";
            for (int i = 0; i < chr.Length; i++)
            {
                if (chr[i].Equals('-'))
                {
                    iSlash++;
                    if (iSlash == 1)
                    {
                        //iYear = Convert.ToInt32(strtmp);
                        int.TryParse(strtmp, out iYear);
                        strtmp = "";
                    }
                    else if (iSlash == 2)
                    {
                        //iMonth = Convert.ToInt32(strtmp);
                        int.TryParse(strtmp, out iMonth);
                        strtmp = "";
                    }
                }
                else if (chr[i].Equals(' '))
                {
                    //iDay = Convert.ToInt32(strtmp);
                    int.TryParse(strtmp, out iDay);
                    strtmp = "";
                }
                else if (chr[i].Equals(':'))
                {
                    iComm++;
                    if (iComm == 1)
                    {
                        //iHour = Convert.ToInt32(strtmp);
                        int.TryParse(strtmp, out iHour);
                        if (iHour >= 24) iHour %= 24;
                        strtmp = "";
                    }
                    else if (iComm == 2)
                    {
                        //iMinute = Convert.ToInt32(strtmp);
                        int.TryParse(strtmp, out iMinute);
                        if (iMinute >= 60) iMinute -= 60;
                        strtmp = "";
                    }
                }
                else
                {
                    strtmp += chr[i];
                }
            }
            //iSecond = Convert.ToInt32(strtmp);
            int.TryParse(strtmp, out iSecond);
            if (iSecond >= 60) iSecond -= 60;

            if ((iSlash == 2) && (iComm == 2))
            {
                dtRecord = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);
                bReturn = true;
            }
            else if (iComm == 2)
            {
                DateTime nowTime = DateTime.Now;
                dtRecord = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, iHour, iMinute, iSecond);
                bReturn = true;
            }
            else
            {
                //just for case
                DateTime nowTime = DateTime.Now;
                dtRecord = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, nowTime.Second);
                bReturn = false;
            }
            {
            }
            return bReturn;
        }
    }
}