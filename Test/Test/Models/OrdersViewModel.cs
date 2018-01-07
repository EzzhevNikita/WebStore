using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{

    public class OrderView
    {
        public int id { get; set; }
        public int User_id { get; set; }
        public DateTime DateOfOrder { get; set; }
        public string Status { get; set; }
        public List<String> StatusList { get; set; }
        public DateTime PaymentDate { get; set; }
        public float Sum { get; set; }
    }

    public class OrderItemView
    {
        public int id { get; set; }
        public int Order_id { get; set; }
        public string Product_Name { get; set; }
        public float Price { get; set; }
        public int Number { get; set; }
    }

    public class OrdersViewModel
    {
        public List<OrderView> order { get; set; }
        public List<OrderItemView> orderItem { get; set; }
    }
}