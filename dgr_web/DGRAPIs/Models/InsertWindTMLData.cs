﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class InsertWindTMLData
    {
        public int uploading_file_TMR_Data_id { get; set; }
        public string onm_wtg { get; set; }
        public string WTGs { get; set; }
        public int wtg_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public dynamic timestamp { get; set; }
        public double avg_active_power { get; set; }
        public double avg_wind_speed { get; set; }
        public double calculated_ws { get; set; }
        public int restructive_WTG { get; set; }
        public string date { get; set; }
        public dynamic from_time { get; set; }
        public dynamic to_time { get; set; }
        public string status { get; set; }
        public int status_code { get; set; }
        public double recon_wind_speed { get; set; }
        public double exp_power_kw { get; set; }
        public double deviation_kw { get; set; }
        public double loss_kw { get; set; }
        public string manual_bd { get; set; }
        public string all_bd { get; set; }
        public string file_name { get; set; }
        public dynamic variable { get; set; }
        public string PLC_min { get; set; }
        public string PLC_max { get; set; }
        public int PC_validity { get; set; }
        public string plc_state_code { get; set; }
        public int operation_mode { get; set; }
        public int low_wind_period { get; set; }
        public int service { get; set; }
        public int visit { get; set; }
        public int error { get; set; }
        public int operation { get; set; }
        public int power_production { get; set; }
        public dynamic created_on { get; set; }
    }
    public class WindSpeedData
    {
        public int windspeed_tmd_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public dynamic tmd_date { get; set; }
        public dynamic tmd_time { get; set; }
        public double windspeed { get; set; }
        public double averageWindSpeed { get; set; }
    }
    public class dailyBasisFetch
    {
        public int site_id { get; set; }
        public string site { get; set; }
        public dynamic data_date { get; set; }
        public int wtg_id { get; set; }
        public string wtg { get; set; }
        public int tml_count { get; set; }
        public double actual_wind_speed { get; set; }
        public double actual_active_power { get; set; }
        public double expected_power { get; set; }
        public double usmh_loss { get; set; }
        public double smh_loss { get; set; }
        public double others_loss { get; set; }
        public double igbd_loss { get; set; }
        public double egbd_loss { get; set; }
        public double loadshedding_loss { get; set; }
        public double pcd_loss { get; set; }
        public double lull_loss { get; set; }
        public double nc_loss { get; set; }
        public double healthcheck_loss { get; set; }
        public double setup_loss { get; set; }
        public double initialization_loss { get; set; }
        public double startup_loss { get; set; }
        public double controller_kwh { get; set; }
        public double lineloss_mu { get; set; }
        public double jmr_kwh { get; set; }
        public double target_kwh { get; set; }
        public double adjusted_expected { get; set; }
        public double difference_expected { get; set; }
        public double ma { get; set; }
        public double iga{ get; set; }
        public double ega_a { get; set; }
        public double ega_b { get; set; }
        public double ega_c { get; set; }

    }
    public class TargetSubQry
    {
        public int site_id { get; set; }
        public string site { get; set; }
        public dynamic date { get; set; }
        public double kwh { get; set; }
    }
    public class CheckUpdateManualBd
    {
        public int site_id { get; set; }
        public string site { get; set; }
        public string date { get; set; }
        public int isTmlData { get; set; }
        public int dataUpdated { get; set; }
        public int import_batch_id { get; set; }
    }
    public class TMLCountComparision
    {
        public Int64 realCount { get; set; }
        public Int64 expected_TML { get; set; }
        public Int64 actual_TML { get; set; }
        public Int64 wtg_count { get; set; }
        public int upload_status_id { get; set; }
    }
    public class GetWindTMLGraphData
    {
        public string all_bd { get; set; }
        public int month_no { get; set; }
        public int site_id { get; set; }
        public double loss_kw { get; set; }
        public double expected_kw_sum { get; set; }
        public double active_power_sum { get; set; }
        public double target_sum { get; set; }
        public double line_loss_per { get; set; }
        public double gen_actual_active_power { get; set; }

        public double lineloss_final { get; set; }
        public double expected_final { get; set; }
        public double target_final { get; set; }
        public double actual_final { get; set; }
        public double lossIGBD_final { get; set; }
        public double lossEGBD_final { get; set; }
        public double lossLULL_final { get; set; }
        public double lossNC_final { get; set; }
        public double lossPCD_final { get; set; }
        public double lossSMH_final { get; set; }
        public double lossUSMH_final { get; set; }
        public double loadShedding { get; set; }
        public int monthlyData { get; set; }
        public double target_wind { get; set; }
    }
	public class ExpectedVsActualDaily
    {
        public string all_bd { get; set; }
        public int site_id { get; set; }
        public dynamic data_date { get; set; }
        public Int64 wtg_id { get; set; }
        public string wtg { get; set; }
        public Int64 wtg_number { get; set; }
        public Int64 tml_count { get; set; }
        public double actual_wind_speed { get; set; }
        public double actual_active_power { get; set; }
        public double recon_wind_speed { get; set; }
        public double expected_power { get; set; }
        public double usmh_loss { get; set; }
        public double smh_loss { get; set; }
        public double others_loss { get; set; }
        public double igbd_loss { get; set; }
        public double egbd_loss { get; set; }
        public double loadshedding_loss { get; set; }
        public double pcd_loss { get; set; }
        public double lull_loss { get; set; }
        public double nc_loss { get; set; }
        public double healthcheck_loss { get; set; }
        public double setup_loss { get; set; }
        public double initialization_loss { get; set; }
        public double startup_loss { get; set; }
        public double controller_kwh { get; set; }
        public double lineloss_mu { get; set; }
        public double jmr_kwh { get; set; }
        public double target_kwh { get; set; }
        public double adjusted_expected { get; set; }
        public double difference_expected { get; set; }
        public double ma { get; set; }
        public double iga { get; set; }
        public double ega_a { get; set; }
        public double ega_b { get; set; }
        public double ega_c { get; set; }
        public double wind_speed { get; set; }
    }
    public class ExpectedResult
    {
        public int site_id { get; set; }
        public string status { get;set; }
        public string date { get; set; }
    }
}
