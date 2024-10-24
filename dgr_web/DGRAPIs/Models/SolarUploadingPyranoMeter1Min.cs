﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarUploadingPyranoMeter1Min
    {
        public dynamic date_time { get; set; }
        public dynamic stringdatetime { get; set; }
        public dynamic date { get; set; }
        public int site_id { get; set; }
        public dynamic ghi_1 { get; set; }
        public dynamic ghi_2 { get; set; }
        public dynamic poa_1 { get; set; }
        public dynamic poa_2 { get; set; }
        public dynamic poa_3 { get; set; }
        public dynamic poa_4 { get; set; }
        public dynamic poa_5 { get; set; }
        public dynamic poa_6 { get; set; }
        public dynamic poa_7 { get; set; }
        public double avg_ghi { get; set; }
        public double avg_poa { get; set; }
        public dynamic amb_temp { get; set; }
        public dynamic mod_temp { get; set; }
        public double P_exp { get; set; }
        public double P_exp_degraded { get; set; }
        public double mod_tXavg_poa { get; set; }
        public double actModWtTemp { get; set; }
        
    }
    public class estimated1Hour
    {
        public double mod_tXavg_poa { get; set; }
        public double glob_inc { get; set; }
        public double t_array { get; set; }
        public double estModTemp { get; set; }
        public double mod_temp { get; set; }
        

    }
    public class SolarUploadingPyranoMeter1Min_1
    {
        public dynamic date_time { get; set; }
        public int site_id { get; set; }
        public string site { get; set; }
        public dynamic ghi_1 { get; set; }
        public dynamic ghi_2 { get; set; }
        public dynamic poa_1 { get; set; }
        public dynamic poa_2 { get; set; }
        public dynamic poa_3 { get; set; }
        public dynamic poa_4 { get; set; }
        public dynamic poa_5 { get; set; }
        public dynamic poa_6 { get; set; }
        public dynamic poa_7 { get; set; }
        public double avg_ghi { get; set; }
        public double avg_poa { get; set; }
        public dynamic amb_temp { get; set; }
        public dynamic mod_temp { get; set; }
        public string weather_type { get; set; }
    }
    public class SolarUploadingPyranoMeter1Min_1a
    {
      	public  int import_batch_id  { get; set; }
    	public int site_id 	{ get; set; }

        public string site { get; set; }

        public dynamic date_time { get; set; }
		public double ghi_1 { get; set; }
		public double ghi_2 { get; set; }
		public double poa_1 { get; set; }
		public double poa_2 { get; set; }
		public double poa_3 { get; set; }
        public double poa_4 { get; set; }
        public double poa_5 { get; set; }
        public double poa_6 { get; set; }
        public double poa_7 { get; set; }
        public double avg_ghi { get; set; }
        public double avg_poa { get; set; }
        public double amb_temp { get; set; }
        public double mod_temp { get; set; }
    }
}
