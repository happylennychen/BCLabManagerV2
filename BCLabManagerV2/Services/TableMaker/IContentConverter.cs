using System.Collections.Generic;

namespace BCLabManager
{
    public interface IContentConverter
    {
        string Type { get; }
        List<string> GetHFileContent(string strStandardH, List<string> strHHeaderComments, List<int> ilstOCVVolt, List<uint> voltList, List<float> listfCurr, List<float> listfTemp, double fCTABase, double fCTASlope);
        List<string> GetCFileContent(string strCFileName, string strHFileName, List<string> strHHeaderComments, List<int> ilstOCVVolt, List<uint> voltList, List<float> listfTemp, List<float> listfCurr, List<List<int>> outYValue, double fCTABase, double fCTASlope);
    }
}