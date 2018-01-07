using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Timers;
using NLog;

namespace Test.Models
{

    public class MailSender
    {
        DbContext db = new DbContext();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public void Start()
        {
            Timer timer = new Timer(86400000D);
            //Timer timer = new Timer(20);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int port = 587;
            string emailFrom = "testwebstore000@gmail.com";
            string password = "webstorepassword";
            string Subject = "WebStore notification";
            string Body = "You don't enter your account for a month. Enter account or it will be deleted";
            string smtpAddress = "smtp.gmail.com";
            try
            {
                IEnumerable<User> users = (from u in db.User where (DateTime.Now - u.Last_Date).TotalDays >= Convert.ToDouble(30) select u);
                foreach (User user in users)
                {
                    MailMessage ms = new MailMessage();
                    ms.From = new MailAddress(emailFrom);
                    ms.To.Add(user.Email);
                    ms.Subject = Subject;
                    ms.Body = Body;
                    ms.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, port))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = true;
                        try
                        {
                            smtp.Send(ms);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}