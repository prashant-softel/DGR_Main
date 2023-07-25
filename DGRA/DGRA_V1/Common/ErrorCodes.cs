using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Common
{
    public enum ErrorCode
    {
        SuccessCode = 0,
        CalDailySolarKPICompFail = 1101,
        CDSKDateTempCorrected,
        CDSKFailUpdateDataPyr15,
        CDSKFailUpdateDataEstHourly,
        CDSKFailTempCorrPr,
        CDSKFailFetchPyrano15TempCorr,
        CDSKFailFetchHourlyLossTempCorr,
        CDSKFailFetchDailyGenTempCorr,
        CDSKFailFetchPvsystTempCorr,
        CDSKFailDelTempCorr = 11010,
        CDSKFailInsertTempCorr,
        CDSKFailFetchSiteFormulas,
        CDSKFailExecuteLoopFormulas,
        CDSKFailExceptionAddingInHash,
        CDSKFailFetchLocationMaster,
        CDSKFailFetchPyrano1min,
        CDSKFailExeLoopCalAvgGhiPoa,
        CDSKFailFetchTrackerLoss,
        CDSKFailExecutingTrackerLoss,
        CDSKFailExecutingPowerExp,
        CDSKFailFetchGenSolar,
        CDSKFailCalLoopLocMast,
        CDSKFailCalLoopLocation,
        CDSKFailUpdatingGenSolar,
        CDSKFailUpdatingInvPlantPR,
        CDSKFailCalculateDailySolarKPI,

    }
    public class ErrorCodes
    {
        public static Dictionary<ErrorCode, string> ErrorDescriptionMap = new Dictionary<ErrorCode, string>
        {
            {ErrorCode.SuccessCode, "API Successful" },
            { ErrorCode.CalDailySolarKPICompFail, "CalculateDailySolarKPI function Complete failure." },
            {ErrorCode.CDSKDateTempCorrected, "Date conversion error in TemperatureCorrectedPr function." },
            {ErrorCode.CDSKFailUpdateDataPyr15, "Exception while updating data into pyranometer 15 min in TemperatureCorrectedPr function." },
            {ErrorCode.CDSKFailUpdateDataEstHourly, "Exception while updating data into estimated_hourly_loss table in TemperatureCorrectedPr function." },
            {ErrorCode.CDSKFailTempCorrPr, "getTemperatureCorrectedPR function complete failure." },
            {ErrorCode.CDSKFailFetchPyrano15TempCorr, "Exception while fetching records from uploading_pyranometer_15_min table in getTemperatureCorrectedPR function." },
            {ErrorCode.CDSKFailFetchHourlyLossTempCorr, "Exception while fetching records from uploading_file_estimated_hourly_loss in getTemperatureCorrectedPR function." },
            {ErrorCode.CDSKFailFetchDailyGenTempCorr, "Exception while fetching records from daily_gen_summary_solar table in getTemperatureCorrectedPR function." },
            {ErrorCode.CDSKFailFetchPvsystTempCorr, "Exception occured while fetching records from uploading_file_pvsyst_loss table in getTemperatureCorrectedPR function." },
            {ErrorCode.CDSKFailDelTempCorr, "Exception while deleting records from temperature _corrected_pr table from database in getTemperatureCorrected function." },
            {ErrorCode.CDSKFailInsertTempCorr, "Exception while inserting data into temprtature_corrected_pr table in getTemperatureCorrectedPR function." },
            {ErrorCode.CDSKFailFetchSiteFormulas, "Exception while getting site formulas from wind_site_formulas table in CalculateDailySolarKPI function." },
            {ErrorCode.CDSKFailExecuteLoopFormulas, "Exception while executing for loop for getting formulas in CalculateDailySolarKPI function" },
            {ErrorCode.CDSKFailExceptionAddingInHash, "Exception while adding inverter AC and Capacity hash table in CalculateDailySolarKPI function." },
            {ErrorCode.CDSKFailFetchLocationMaster, "Exception while getting data from location_master_solar table in CalculateDailySolarKPI function." },
            {ErrorCode.CDSKFailFetchPyrano1min, "Exception while fetching data from uploading_pyranometer_1_min_solar table in CalculateDailySolarKPI function" },
            {ErrorCode.CDSKFailExeLoopCalAvgGhiPoa, "Exception while executing loop of calculating average GHI POA values in CalculateDailySolarKPI function" },
            {ErrorCode.CDSKFailFetchTrackerLoss, "Exception while fetching records from tracker loss table in CalculateTrackerLosses function" },
            {ErrorCode.CDSKFailExecutingTrackerLoss, "Exception while executing CalculateTrackerLosses() function." },
            {ErrorCode.CDSKFailExecutingPowerExp, "Exception while executing PowerExpected() function" },
            {ErrorCode.CDSKFailFetchGenSolar, "Exception while fetching records from uploading_file_generation_solar table in CalculateSolarKPI function" },
            {ErrorCode.CDSKFailCalLoopLocMast, "Exception while calculation in loop of location master in calculateDailySolarKPI function" },
            {ErrorCode.CDSKFailCalLoopLocation, "Exception while calculation in loop of location master in calculateDailySolarKPI function" },
            {ErrorCode.CDSKFailUpdatingGenSolar, "Exception during updating data in table uploading_file_generation_solar in CalculateDailySolarKPI function" },
            {ErrorCode.CDSKFailUpdatingInvPlantPR, "Exception while updating inv_pr and plant_pr to null in uploading_file_generation_solar table in CalculateDailySolarKPI function." },
            {ErrorCode.CDSKFailCalculateDailySolarKPI, "Exception in CalculateDailySolarKPI function" },

        };
    }
}
