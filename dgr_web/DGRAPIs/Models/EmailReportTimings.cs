using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class EmailReportTimings
    {
        public string dailyTime { get; set; }
        public string windWeeklyTimw { get; set; }
        public string solarWeeklyTime { get; set; }
        public string windWeekDay { get; set; }
        public string solarWeekDay { get; set; }

    }
    public class EmailReportTimingsLog
    {
        public int email_report_timings_log_id { get; set; }
        public string daily_report { get; set; }
        public string wind_weekly { get; set; }
        public string solar_weekly { get; set; }
        public string first_dgr_reminder { get; set; }
        public string second_dgr_reminder { get; set; }
        public string wind_weekly_day { get; set; }
        public string solar_weekly_day { get; set; }
        public string updated_by_name { get; set; }
        public int updated_by_id { get; set; }
        public string updated_by_role { get; set; }
        public dynamic created_on { get; set; }
    }
}
