using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class AdminPanelViewModel
    {
        public int id { get; set; }

        public string First_Name { get; set; }

        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Last_Date { get; set; }
        public DateTime Delete_Date { get; set; }
        public int Status { get; set; }
    }
}