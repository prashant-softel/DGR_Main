using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class EmailReportTimings
    {
        public string dailyTime { get; set; }
        public string windWeeklyTimw { get; set; }
        public string solarWeeklyTime { get; set; }
        public string windweekday { get; set; }
        public string solarweekday { get; set; }

        public List<EmailReportTimings> time_list = new List<EmailReportTimings>();
        public List<EmailReportTimings> GetTimeList()
        {
            return time_list;
        }
    }

    public class EmailReportTimingsLog
    {
        public int email_report_timings_log_id { get; set; }
        public string daily_report { get; set; }
        public string wind_weekly { get; set; }
        public string solar_weekly { get; set; }
        public string wind_weekly_day { get; set; }
        public string solar_weekly_day { get; set; }
        public string updated_by_name { get; set; }
        public int updated_by_id { get;set; }
        public string updated_by_role { get; set; }
        public dynamic created_on { get; set; }
        public List<EmailReportTimingsLog> time_list = new List<EmailReportTimingsLog>();
        public List<EmailReportTimingsLog> GetTimeList()
        {
            return time_list;
        }
    }
    }
