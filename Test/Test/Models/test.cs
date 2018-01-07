using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Web;

namespace Test.Models
{
    public class DbContext : DataContext
    {
        public DbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Akvelon_test.mdf;Integrated Security=True")
        { }

        public Table<Basket> Basket;
        public Table<Category> Category;
        public Table<Order> Order;
        public Table<Order_Status> Orderstatus;
        public Table<OrderItem> Orderitem;
        public Table<Product> Product;
        public Table<Product_in_Category> ProductInCategory;
        public Table<User_history> UserHistory;
        public Table<Role> Role;
        public Table<User> User;
        public Table<User_in_Role> UserInRole;
    }

    [Table()]
    public class User
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }

        [Column(Name = "First_Name", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string First_Name { get; set; }

        [Column(Name = "Last_Name", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string Last_Name { get; set; }

        [Column(Name = "Email", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string Email { get; set; }

        [Column(Name = "Password", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string Password { get; set; }

        [Column(Name = "Reg_Date", DbType = "datetime", CanBeNull = false)]
        public DateTime Reg_Date { get; set; }

        [Column(Name = "Last_Date", DbType = "datetime", CanBeNull = true)]
        public DateTime Last_Date { get; set; }

        [Column(Name = "Delete_Date", DbType = "datetime", CanBeNull = true)]
        public DateTime Delete_Date { get; set; }

        [Column(Name = "Status", DbType = "int", CanBeNull = false)]
        public int Status { get; set; }
    }

    [Table()]
    public class Role
    {
        [Column(IsPrimaryKey = true,  IsDbGenerated = true)]
        public int id { get; set; }
        [Column(Name = "Role_Name", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string Role_Name { get; set; }
    }

    [Table()]
    public class User_in_Role
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }
        [Column(Name = "User_id", DbType = "int", CanBeNull = false)]
        public int User_id { get; set; }
        [Column(Name = "Role_id", DbType = "int", CanBeNull = false)]
        public int Role_id { get; set; }
    }

    [Table()]
    public class Product
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }
        [Column(Name = "Name", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string Name { get; set; }
        [Column(Name = "Description", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string Description { get; set; }
        [Column(Name = "Price", DbType = "float", CanBeNull = false)]
        public float Price { get; set; }
    }

    [Table()]
    public class Product_in_Category
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }
        [Column(Name = "Product_id", DbType = "int", CanBeNull = false)]
        public int Product_id { get; set; }
        [Column(Name = "Category_id", DbType = "int", CanBeNull = false)]
        public int Category_id { get; set; }
    }

    [Table()]
    public class Category
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }
        [Column(Name = "Name", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string Name { get; set; }
    }

    [Table()]
    public class Basket
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }
        [Column(Name = "User_id", DbType = "int", CanBeNull = false)]
        public int User_id { get; set; }
        [Column(Name = "Product_id", DbType = "int", CanBeNull = false)]
        public int Product_id { get; set; }
        [Column(Name = "Number", DbType = "int", CanBeNull = false)]
        public int Number { get; set; }
    }

    [Table()]
    public class User_history
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }
        [Column(Name = "User_id", DbType = "int", CanBeNull = false)]
        public int User_id { get; set; }
        [Column(Name = "Product_id", DbType = "int", CanBeNull = false)]
        public int Product_id { get; set; }
        [Column(Name = "Date_of_buy", DbType = "datetime", CanBeNull = false)]
        public DateTime Date_of_buy { get; set; }
    }

    [Table()]
    public class Order
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }

        [Column(Name = "User_id", DbType = "int", CanBeNull = false)]
        public int User_id { get; set; }

        [Column(Name = "DateOfOrder", DbType = "datetime", CanBeNull = false)]
        public DateTime DateOfOrder { get; set; }

        [Column(Name = "Status_id", DbType = "int", CanBeNull = false)]
        public int Status_id { get; set; }

        [Column(Name = "PaymentDate", DbType = "datetime", CanBeNull = false)]
        public DateTime PaymentDate { get; set; }

        [Column(Name = "Sum", DbType = "float", CanBeNull = false)]
        public float Sum { get; set; }
    }

    [Table()]
    public class Order_Status
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }

        [Column(Name = "Name", DbType = "nvarchar(MAX)", CanBeNull = false)]
        public string Name { get; set; }
    }

    [Table()]
    public class OrderItem
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }

        [Column(Name = "Order_id", DbType = "int", CanBeNull = false)]
        public int  Order_id { get; set; }

        [Column(Name = "Product_id", DbType = "int", CanBeNull = false)]
        public int Product_id { get; set; }

        [Column(Name = "Price", DbType = "float", CanBeNull = false)]
        public float Price { get; set; }

        [Column(Name = "Number", DbType = "int", CanBeNull = false)]
        public int Number { get; set; }
    }

    
}