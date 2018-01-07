using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace Test.Models
{
    public class SalesPerson
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static DbContext db = new DbContext();
        private static List<Product> products = new List<Product>();
        public void AddItem(AddItemViewModel newProduct)
        {
            Product pr = new Product();
            pr.Name = newProduct.Name;
            pr.Description = newProduct.Description;
            pr.Price = newProduct.Price;
            try
            {
                db.Product.InsertOnSubmit(pr);
                db.SubmitChanges();
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }

            Product_in_Category pic = new Product_in_Category()
            {
                Product_id = pr.id,
                Category_id = (from c in db.Category
                               where c.Name == newProduct.Category
                               select c.id).First()
            };
            try
            {
                db.ProductInCategory.InsertOnSubmit(pic);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public void SaveChanges(SellerPanelViewModel product)
        {
            try
            {
                Product productToChange = (from p in db.Product where product.id == p.id select p).First();
                productToChange.Name = product.Name;
                productToChange.Description = product.Description;
                productToChange.Price = product.Price;
                db.SubmitChanges();

                Product_in_Category changecat = (from c in db.ProductInCategory
                                                 where product.id == c.Product_id
                                                 select c).First();
                changecat.Category_id = (from cat in db.Category
                                         where product.Category == cat.Name
                                         select cat.id).First();
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public IEnumerable<SellerPanelViewModel> GetItems()
        {
            try
            {
                List<SellerPanelViewModel> products = new List<SellerPanelViewModel>();
                IEnumerable<Product> pr = (from p in db.Product select p);
                foreach (Product p in pr)
                {
                    SellerPanelViewModel sp = new SellerPanelViewModel()
                    {
                        id = p.id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Category = (from c in db.Category
                                    where (from pic in db.ProductInCategory
                                           where pic.Product_id == p.id
                                           select pic.Category_id).First() == c.id
                                    select c.Name).First()
                    };
                    products.Add(sp);
                }
                return products.AsEnumerable();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

        public SellerPanelViewModel GetItemById(int id)
        {
            try
            {
                Product product = (from p in db.Product where p.id == id select p).First();
                SellerPanelViewModel pr = new SellerPanelViewModel()
                {
                    id = product.id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = (from c in db.Category
                                where (from pin in db.ProductInCategory
                                       where pin.Product_id == product.id
                                       select pin.Category_id).First() == c.id
                                select c.Name).First()
                };
                return pr;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
            
        }

        public void AddCat(string Name)
        {
            Category cat = new Category() { Name = Name };
            try
            {
                db.Category.InsertOnSubmit(cat);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public IEnumerable<Category> ViewCategories()
        {
            try
            {
                IEnumerable<Category> categories = (from c in db.Category select c);
                return categories;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

        public OrdersViewModel GetOrders()
        {
            try
            {
                IEnumerable<Order> order = (from o in db.Order select o);
                IEnumerable<OrderItem> orderItem = (from oi in db.Orderitem select oi);
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
                    ov.StatusList.Remove(ov.Status);
                    ov.StatusList.Insert(0, ov.Status);
                    orders.order.Add(ov);
                }
                foreach (OrderItem oi in orderItem)
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
                return orders;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }


        public void ChangeStatus(int saveid, string Status)
        {
            try
            {
                Order order = (from o in db.Order where o.id == saveid select o).First();
                if (Status == "Paid")
                {
                    order.Status_id = (from s in db.Orderstatus where s.Name == Status select s.id).First();
                    order.PaymentDate = DateTime.Now;
                    db.SubmitChanges();
                }
                else
                {
                    order.Status_id = (from s in db.Orderstatus where s.Name == Status select s.id).First();
                    db.SubmitChanges();
                }
                

                if (Status == "Issued to the client")
                {
                    List<User_history> uh = new List<User_history>();
                    IEnumerable<OrderItem> orderitems = (from oi in db.Orderitem where order.id == oi.id select oi);
                    foreach (OrderItem oi in orderitems)
                    {
                        User_history h = new User_history()
                        {
                            User_id = order.User_id,
                            Product_id = oi.Product_id,
                            Date_of_buy = order.PaymentDate
                        };
                        uh.Add(h);
                    }
                    db.UserHistory.InsertAllOnSubmit(uh);
                    db.SubmitChanges();
                }


                if (Status == "Decline")
                {
                    IEnumerable<OrderItem> orderitem = (from oi in db.Orderitem where order.id == oi.id select oi);
                    db.Orderitem.DeleteAllOnSubmit(orderitem);
                    db.SubmitChanges();
                    db.Order.DeleteOnSubmit(order);
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