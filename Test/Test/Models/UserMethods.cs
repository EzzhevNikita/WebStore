using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace Test.Models
{
    public class UserMethods
    {
        static DbContext db = new DbContext();
        static Auth userdata = new Auth();
        static private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IEnumerable<Product> GetItems()
        {
            try
            {
                var products = (from p in db.Product select p);
                return products;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }
        public void AddToCart(int productID)
        {
            try
            {
                Basket cart = (from b in db.Basket
                               where (b.User_id == userdata.curUser && b.Product_id == productID)
                               select b).First();
                cart.Number++;
                db.SubmitChanges();

            }
            catch(Exception ex)
            {
                Basket newcart = new Basket() { User_id = userdata.curUser, Product_id = productID, Number = 1 };
                db.Basket.InsertOnSubmit(newcart);
                db.SubmitChanges();
            }

        }

        public List<BusketViewModel> ViewBasket()
        {
            try
            {
                IEnumerable<Basket> pid = (from b in db.Basket where b.User_id == userdata.curUser select b);
                List<BusketViewModel> busket = new List<BusketViewModel>();
                foreach (Basket el in pid)
                {
                    BusketViewModel b = new BusketViewModel();
                    b.id = el.Product_id;
                    b.Product_Name = (from p in db.Product where p.id == el.Product_id select p.Name).First();
                    b.Number = el.Number;
                    b.Price = b.Number * (from p in db.Product where p.id == el.Product_id select p.Price).First();
                    busket.Add(b);
                }
                return busket;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

        public void MakeAnOder()
        {
            try
            {
                IQueryable<Basket> busket = (from b in db.Basket where b.User_id == userdata.curUser select b);
                List<OrderItem> oi = new List<OrderItem>();
                float sum = 0;
                foreach (var el in busket)
                {
                    float price = (from p in db.Product where p.id == el.Product_id select p.Price).First();
                    OrderItem item = new OrderItem() { Product_id = el.Product_id, Number = el.Number, Price = price };
                    sum += (price * el.Number);
                    oi.Add(item);
                }
                Order order = new Order() { User_id = userdata.curUser, DateOfOrder = DateTime.Now, PaymentDate = DateTime.MaxValue, Status_id = 1, Sum = sum };
                db.Order.InsertOnSubmit(order);
                db.SubmitChanges();

                foreach (var el in oi)
                {
                    el.Order_id = order.id;
                }
                db.Orderitem.InsertAllOnSubmit(oi);
                db.SubmitChanges();

                IEnumerable<Basket> cart = (from b in db.Basket where b.User_id == userdata.curUser select b);
                db.Basket.DeleteAllOnSubmit(cart);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public IEnumerable<HistoryViewModel> ViewHistory()
        {
            try
            {
                IEnumerable<User_history> uh = (from u in db.UserHistory where u.User_id == userdata.curUser select u);
                List<Product> product = new List<Product>();
                List<HistoryViewModel> history = new List<HistoryViewModel>();
                foreach (User_history u in uh)
                {
                    Product pr = ((from p in db.Product where p.id == u.Product_id select p).First());
                    HistoryViewModel h = new HistoryViewModel() { Name = pr.Name, Descrption = pr.Description, Price = pr.Price, Date_of_buy = u.Date_of_buy };
                    history.Add(h);
                }
                return history.AsEnumerable<HistoryViewModel>();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

        public OrdersViewModel ViewOrder()
        {
            try
            {
                IEnumerable<Order> order = (from o in db.Order where userdata.curUser == o.User_id select o);
                OrdersViewModel orders = new OrdersViewModel();
                orders.order = new List<OrderView>();
                orders.orderItem = new List<OrderItemView>();
                foreach (Order o in order)
                {
                    OrderView ov = new OrderView()
                    {
                        id = o.id,
                        User_id = o.User_id,
                        DateOfOrder = o.DateOfOrder,
                        Status = (from s in db.Orderstatus where o.Status_id == s.id select s.Name).First(),
                        StatusList = (from s in db.Orderstatus select s.Name).ToList(),
                        PaymentDate = o.PaymentDate,
                        Sum = o.Sum
                    };
                    orders.order.Add(ov);
                    IEnumerable<OrderItem> orderitem = (from oi in db.Orderitem where ov.id == oi.Order_id select oi);
                    foreach (OrderItem oi in orderitem)
                    {
                        OrderItemView oiv = new OrderItemView()
                        {
                            id = oi.id,
                            Order_id = oi.Order_id,
                            Product_Name = (from p in db.Product where p.id == oi.Product_id select p.Name).First(),
                            Price = oi.Price,
                            Number = oi.Number
                        };
                        orders.orderItem.Add(oiv);
                    }
                }
                return orders;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

        public void ChangeUserData(User user)
        {
            try
            {
                User userToChange = (from u in db.User where user.id == u.id select u).First();
                userToChange.First_Name = user.First_Name;
                userToChange.Last_Name = user.Last_Name;
                userToChange.Last_Date = DateTime.Now;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public void DeleteProduct(int id)
        {
            try
            {
                Basket cart = (from c in db.Basket where c.User_id == userdata.curUser && c.Product_id == id select c).First();
                db.Basket.DeleteOnSubmit(cart);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public void RestoreAccount(RestoreViewModel user)
        {
            try
            {
                User userToChange = (from u in db.User where u.Email == user.email select u).First();
                userToChange.Password = user.password;
                userToChange.Status = 0;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}