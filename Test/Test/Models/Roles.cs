using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace Test.Models
{

    public class Roles
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static DbContext db = new DbContext();
        static Auth curUser = new Auth();
        static private IEnumerable<string> GetRoles
        {
            get
            {
                try
                {
                    IEnumerable<string> roles = (from r in db.Role select r.Role_Name);
                    return (roles);
                }
                catch(Exception ex)
                {
                    logger.Error(ex.Message);
                    return null;
                }
            }
        }

        public static bool IsInRole(int userId, string str)
        {
            List<int> userRoleId = (from uir in db.UserInRole where uir.User_id == userId select uir.Role_id).ToList<int>();
            List<string> userRoleNames = new List<string>();
            foreach (int el in userRoleId)
            {
                userRoleNames.Add((from r in db.Role where r.id == el select r.Role_Name).First());
            }
            foreach (string el in userRoleNames)
            {
                if (str.Contains(el))
                    return true;
            }
            return false;
        }

    }
}