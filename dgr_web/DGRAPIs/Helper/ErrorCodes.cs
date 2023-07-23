using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Helper
{
    public enum ErrorCode
    {
        CalDailySolarKPICompFail = 1101,
        CDSKDateTempCorrected,
        CDSKFailUpdateDataPyr15,
        CDSKFailUpdateDataEstHourly,

    }

    public static class ErrorCodes
    {
        public static Dictionary<ErrorCode, string> ErrorDescriptionMap = new Dictionary<ErrorCode, string>
        {
            { ErrorCode.CalDailySolarKPICompFail, "CalculateDailySolarKPI function Complete failure." },
            {ErrorCode.CDSKDateTempCorrected, "Date conversion error in TemperatureCorrectedPr function." },
            {ErrorCode.CDSKFailUpdateDataPyr15, "Exception while updating data into pyranometer 15 min in TemperatureCorrectedPr function." },
            {ErrorCode.CDSKFailUpdateDataEstHourly, "Exception while updating data into estimated_hourly_loss table in TemperatureCorrectedPr function." },
        };
    }
}
