using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using NLog;


namespace Test.Models
{
    public class AccountDisable
    {

        Auth user = new Auth();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        DbContext db = new DbContext();
        public void Start()
        {
            Timer timer = new Timer(86400000D);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                IEnumerable<User> users = (from u in db.User where (DateTime.Now - u.Last_Date).TotalDays >= Convert.ToDouble(44) select u);
                foreach (User u in users)
                {
                    u.Status = 1;
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}