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
}
