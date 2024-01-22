using DGRAPIs.Models;
using DGRAPIs.Repositories;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections;

namespace DGRAPIs.BS
{
    public interface MailServiceBS
    {
        Task<List<MailResponse>> SendEmailAsync(MailRequest mailRequest);


    }
    public class MailService
    {
        //private readonly MailSettings _mailSettings;
        //public MailService(IOptions<MailSettings> mailSettings)
        //{
        //    _mailSettings = mailSettings.Value;
        //}
        //public async Task<List<MailResponse>> SendEmailAsync(MailRequest mailRequest)
        //Tanvi's Changes
		public static async Task<List<MailResponse>> SendEmailAsync(MailRequest mailRequest, MailSettings _mailSettings,int withoutSetTime)
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                List<TimeSpan> time = new List<TimeSpan> ();

                time.Add(TimeSpan.Parse(MyConfig.GetValue<string>("Timer:DailyReportTime")));
                time.Add(TimeSpan.Parse(MyConfig.GetValue<string>("Timer:WeeklyReportTime")));
                time.Add(TimeSpan.Parse(MyConfig.GetValue<string>("Timer:WeeklyReportTimeSolar")));
                time.Add(TimeSpan.Parse(MyConfig.GetValue<string>("Timer:firstDgrReminderTime")));
                time.Add(TimeSpan.Parse(MyConfig.GetValue<string>("Timer:secondDgrReminderTime")));

                var timeNow = DateTime.Now.TimeOfDay;
                //daily mail
               
                    List<MailResponse> _MailResponse = new List<MailResponse>();

                    var email = new MimeMessage();
                    //email.Sender = MailboxAddress.Parse(MailSettings.Mail);
                    email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                    foreach (var mail in mailRequest.ToEmail)
                    {
                        email.To.Add(MailboxAddress.Parse(mail));
                    }

                if(mailRequest.CcEmail != null){

                    foreach (var mail in mailRequest.CcEmail)
                    {
                        email.Cc.Add(MailboxAddress.Parse(mail));
                    }
                }
                    // email.Cc.Add(MailboxAddress.Parse(mailRequest.CcEmail));
                    email.Subject = mailRequest.Subject;
                    var builder = new BodyBuilder();
                    if (mailRequest.Attachments != null)
                    {
                        byte[] fileBytes;
                        foreach (var file in mailRequest.Attachments)
                        {
                            //var file = mailRequest.Attachments; 
                            if (file.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    file.CopyTo(ms);
                                    fileBytes = ms.ToArray();
                                }
                                //builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse("application/octet-stream"));
                            }
                        }
                    }
                    builder.HtmlBody = mailRequest.Body;
                    email.Body = builder.ToMessageBody();

                foreach(TimeSpan schduledTime in time)
                {
                    TimeSpan endTime = schduledTime.Add(TimeSpan.FromMinutes(8)) ;
                    //if ((schduledTime >= approxTime) && (schduledTime <= timeNow))
                    if (((timeNow >= schduledTime) && (timeNow <= endTime)) || withoutSetTime == 1)
                    {
                        using var smtp = new MailKit.Net.Smtp.SmtpClient();
                        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                        // smtp.Connect(MailSettings.Host, MailSettings.Port, SecureSocketOptions.StartTls);
                        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                        //smtp.Authenticate(MailSettings.Mail, MailSettings.Password);
                        await smtp.SendAsync(email);
                        smtp.Disconnect(true);

                        _MailResponse.Add(new MailResponse { mail_sent = true, message = "Mail sent successfully" });
                        break;
                    }
                    else
                    {
                        _MailResponse.Add(new MailResponse { mail_sent = false, message = "Mail not sent." });
                    }
                }
                    return _MailResponse;
                
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }
        }
    }
}
