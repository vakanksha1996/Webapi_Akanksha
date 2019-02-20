using Akanksha.Models;
using Akanksha.ViewModel;
using PagedList;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Net.Http;

namespace Akanksha.Controllers
{
    public class CartController : Controller
    {
        private AkankshaEntities db;


        public CartController()
        {
            db = new AkankshaEntities();
        }


        // GET: Cart
        public ActionResult Index()
        {
            
            return View();
        }



        public ActionResult GetCartItemList()
        {
            string Id = User.Identity.GetUserId();
            //var cart_items = db.Carts.Where(c => c.Id == Id);
            IEnumerable<Cart> cartitems;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Cartapi/Getcarts?userid=" + Id);
                response.Wait();
                var result = response.Result;
                var read = result.Content.ReadAsAsync<IList<Cart>>();
                read.Wait();
                cartitems = read.Result;
            }

            return View(cartitems);
        }



       [Authorize]
        public JsonResult AddtoCart(int productid)
        {

            string id = User.Identity.GetUserId();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("Cartapi?userid=" + id + "&productid=" + productid);
                responsetask.Wait();
                var result = responsetask.Result;
                var readtask = result.Content.ReadAsAsync<bool>();
                readtask.Wait();
                var isalreadyadded = readtask.Result;
                if (isalreadyadded == true)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Cart cart = new Cart
                    {
                        Id = User.Identity.GetUserId(),
                        ProductId = productid,
                        Quantity = 1,
                        //Amount = product.Price

                    };

                    using(var c = new HttpClient())
                    {
                        c.BaseAddress = new Uri("http://localhost:55437/api/");
                        var response = client.PostAsJsonAsync("Cartapi", cart);
                        response.Wait();
                        var r = response.Result;
                       
                    }

                    return Json(true, JsonRequestBehavior.AllowGet);

                }

            }

        //    string id = User.Identity.GetUserId();
        //    var product = db.Products.Single(p => p.ProductId == productid);
        //    var isalreadyadded =    db.Carts.Count(c => c.ProductId == product.ProductId && c.Id == id);
        //    if (isalreadyadded == 0)
        //    {
        //        Cart c = new Cart
        //        {
        //            Id = User.Identity.GetUserId(),
        //            ProductId = productid,
        //            Quantity = 1,
        //            Amount = product.Price

        //        };
        //        db.Carts.Add(c);
        //        db.SaveChanges();
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
           

            
           //// return RedirectToAction("GetProductsByName","Product", new { name = product.Subcategory.Name });
        }




        public JsonResult RemovefromCart(int id)
            
        {

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.DeleteAsync("Cartapi?cartid=" + id.ToString());
                responsetask.Wait();
                var result = responsetask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        //    var cart = db.Carts.SingleOrDefault(c => c.CartId == id);
        //    if (cart != null)
        //    {
        //        db.Carts.Remove(cart);
        //        db.SaveChanges();
        //        string Id = User.Identity.GetUserId();
        //        var cart_items = db.Carts.Where(c => c.Id == Id);
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(false, JsonRequestBehavior.AllowGet);

        }





        public JsonResult UpdateQuantity(int cartid,int qty)
        {
            Cart cart;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("Cartapi?cartid=" + cartid);
                responsetask.Wait();
                var result = responsetask.Result;
                var read = result.Content.ReadAsAsync<Cart>();
                read.Wait();
                cart = read.Result;
            }

           // cart.Quantity = qty;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("Cartapi?cartid=" + cart.CartId.ToString() + "&qty=" + qty.ToString());
                responsetask.Wait();
                var result = responsetask.Result;
                var read = result.Content.ReadAsAsync<bool>();
                read.Wait();
                if (read.Result == true)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            //var cart = db.Carts.Single(c => c.CartId == cartid);
            //if (cart != null)
            //{
            //    cart.Quantity = qty;
            //    db.SaveChanges();
            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}
            //return Json(false, JsonRequestBehavior.AllowGet);
        }




        public ActionResult CheckOut(Address address)
        {
            string id = User.Identity.GetUserId();
            CheckOutViewModel cvm;
            //var cvm = new CheckOutViewModel
            //{
            //    Cartitems = db.Carts.Where(c => c.Id == id && c.Product.NumberOfStock!=0).ToList(),
            //    States = db.States.ToList(),
            //    PaymentModes = db.Payments.ToList(),
            //    AddressBook = db.Addresses.Where(a=>a.Id == id).ToList()
            //};

            using(var client  =  new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Cartapi/GetCheckoutViewModel?userid=" +id);
                response.Wait();
                var result = response.Result;
                var read = result.Content.ReadAsAsync<CheckOutViewModel>();
                read.Wait();
                cvm = read.Result;
            }

            if (address.AddressId != 0)
            {
                if (address.City == null)
                {
                    //  cvm.address = db.Addresses.Single(a => a.AddressId == address.AddressId);
                    cvm.address = cvm.AddressBook.Single(a => a.AddressId == address.AddressId);

                }
                else
                {

                    using(var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/");
                        var response = client.PutAsJsonAsync("Addressapi", address);
                        response.Wait();
                        var result = response.Result;

                    }

                    using (var client = new HttpClient())
                    {

                        client.BaseAddress = new Uri("http://localhost:55437/api/");
                        var responsetask = client.GetAsync("Addressapi?addressid=" + address.AddressId);
                        responsetask.Wait();
                        var r = responsetask.Result;
                        var read = r.Content.ReadAsAsync<Address>();
                        read.Wait();
                        cvm.address = read.Result;
                    }
                    // var addressindb = db.Addresses.Single(a => a.AddressId == address.AddressId);
                    //var addressindb = cvm.AddressBook.Single(a => a.AddressId == address.AddressId);
                    //addressindb.HouseNo = address.HouseNo;
                    //addressindb.City = address.City;
                    //addressindb.Colony_Street = address.Colony_Street;
                    //addressindb.StateId = address.StateId;
                    //addressindb.Pincode = address.Pincode;
                    //db.SaveChanges();
                    //cvm.address = addressindb;
                }
            }

            if (address.City != null && address.AddressId==0)
            {
                //address.Id = User.Identity.GetUserId();
                //
                //db.Addresses.Add(address);
                //db.SaveChanges();
                //cvm.address = address;


                address.Id = id;



                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:55437/api/Addressapi");
                    var response = client.PostAsJsonAsync<Address>("Addressapi", address);
                    response.Wait();
                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        using (var clientt = new HttpClient())
                        {
                            clientt.BaseAddress = new Uri("http://localhost:55437/api/");
                            var responsetask = clientt.GetAsync("Addressapi?userid=" + id);
                            responsetask.Wait();
                            var resultt = responsetask.Result;
                            var read = resultt.Content.ReadAsAsync<IList<Address>>();
                            read.Wait();
                            var addresses = read.Result;
                            cvm.address = addresses.Last();
                        }
                    }

                    else
                    {
                        cvm.address = null;
                    }
                }

            }
            return View(cvm);
        }



        public ActionResult EditShippingAddress(int id)
        {

            AddressViewModel avm;

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Addressapi?id=" + id.ToString());
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsAsync<AddressViewModel>();
                    read.Wait();
                    avm = read.Result;
                }

                else
                {
                    avm = null;
                }
            }
           
            //var address = db.Addresses.Single(a => a.AddressId == id);
            //var avm = new AddressViewModel
            //{
            //    address = address,
            //    states = db.States.ToList()
            //};
            return View(avm);
        }



        public ActionResult OrderCompletion(int AddressId, Order Order) 
        {
            string id = User.Identity.GetUserId();

          //  var address = db.Addresses.Single(a => a.AddressId == AddressId);
            //var user = db.AspNetUsers.Single(a => a.Id == id);
          //  var payment = db.Payments.Single(p => p.PaymentId == Order.PaymentId);
           // var cartitems = db.Carts.Where(c => c.Id == id && c.Product.NumberOfStock!=0).ToList();
            Address address;
            AspNetUser user;
            List<Cart> cartitems;
            CheckOutViewModel cvm;
            //  Payment payment;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("Addressapi?addressid=" + AddressId.ToString());
                responsetask.Wait();
                var result = responsetask.Result;
                var read = result.Content.ReadAsAsync<Address>();
                read.Wait();
                address = read.Result;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("NormalUserapi?id=" + id);
                responsetask.Wait();
                var result = responsetask.Result;
                var read = result.Content.ReadAsAsync<AspNetUser>();
                read.Wait();
                user = read.Result;
            }
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:55437/api/");
            //    var responsetask = client.GetAsync("Paymentapi?paymentid=" + Order.PaymentId);
            //    responsetask.Wait();
            //    var result = responsetask.Result;
            //    var read = result.Content.ReadAsAsync<Payment>();
            //    read.Wait();
            //    payment = read.Result;
            //}


            Order.CreatedDate = DateTime.Now;
            // Order.PaymentId = payment.PaymentId;
            Order.Id = id;
            Order.ShippingAddress = address.HouseNo + "\n" + address.Colony_Street + "\n" +
                   address.City + "\n " + address.State.Name + "\n " + address.Pincode;

            //var cm = new CheckOutViewModel
            //{
            //    Cartitems=cartitems,
            //    address = address,
            //    AddressBook = db.Addresses.ToList(),
            //    States = db.States.ToList(),
            //    PaymentModes = db.Payments.ToList(),
            //    Order = Order

            //};

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Cartapi/GetCheckoutViewModel?userid=" + id);
                response.Wait();
                var result = response.Result;
                var read = result.Content.ReadAsAsync<CheckOutViewModel>();
                read.Wait();
                cvm = read.Result;   
            }

            cartitems = cvm.Cartitems;
            cvm.address = address;
            cvm.Order = Order;




                if (!ModelState.IsValid)
                {
                    return View("CheckOut", cvm);
                }

            //db.Orders.Add(Order);


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.PostAsJsonAsync<Order>("Orderapi/PostOrder", Order);
                response.Wait();
                var result = response.Result;

            }
            //  var existingcustomer = db.Customers.SingleOrDefault(c => c.Id == id);

            Customer existingcustomer = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Customerapi?id=" + user.Id);
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsAsync<Customer>();
                    read.Wait();
                    existingcustomer = read.Result;
                }
            }
            if (existingcustomer == null)
            {
                Customer customer = new Customer()
                {
                    Name = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Id,
                    Id = user.Id

                };

                // db.Customers.Add(customer);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:55437/api/Customerapi");
                    var response = client.PostAsJsonAsync<Customer>("Customerapi", customer);
                    response.Wait();

                    var result = response.Result;
                }

            }

            // db.SaveChanges();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Orderapi");
                response.Wait();
                var result = response.Result;
                var read = result.Content.ReadAsAsync<IList<Order>>();
                read.Wait();
                IEnumerable<Order> orders = read.Result;
                Order = orders.Where(o => o.Id == id).OrderBy(o => o.OrderId).Last();
            }


            foreach (var item in cartitems)
            {
                item.PaymentDate = DateTime.Now;
               var orderdetail = new OrderDetail
                {

                    ProductId = item.ProductId,
                    OrderId = Order.OrderId,
                    Quantity = item.Quantity,
                    TotalPrice = item.Product.Price*item.Quantity,
                    
                };
               
                using(var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:55437/api/");
                    var response = client.GetAsync("Productapi?Id=" + item.ProductId);
                    response.Wait();
                    var result = response.Result;
                    var read = result.Content.ReadAsAsync<Product>();
                    read.Wait();
                    var   producttobebuy = read.Result;
                    if (producttobebuy.NumberOfStock < item.Quantity)
                    {
                        Order.ItemQuantity = Order.ItemQuantity - item.Quantity;
                        Order.Subtotal = Order.Subtotal - item.Quantity * item.Product.Price;
                        Order.AspNetUser = null;
                        Order.Payment = null;
                        using(var c = new HttpClient())
                        {
                            c.BaseAddress = new Uri("http://localhost:55437/api/");
                            var responsetask = c.PutAsJsonAsync("Orderapi/Putorder", Order);
                            responsetask.Wait();
                            var r = responsetask.Result;

                        }
                        return Content(producttobebuy.Name + " is out of stock");
                    }
                    else
                    {
                        producttobebuy.NumberOfStock = producttobebuy.NumberOfStock - item.Quantity;
                        using (var c = new HttpClient())
                        {
                            c.BaseAddress = new Uri("http://localhost:55437/api/");
                            var responsetask = c.PostAsJsonAsync("orderapi/PostOrderDetails", orderdetail);
                            responsetask.Wait();
                            var r = responsetask.Result;

                        }
                    }
                }
               
              
            }

            SendEmail(Order);                 
            return View(Order);
        }


        public void SendEmail(Order order)
        {
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("0d641e048c0b8d", "8a0f3c27bd6ab2"),
                EnableSsl = true
            };

            MailMessage mail = new MailMessage();
              mail.To.Add(order.AspNetUser.Email);
            mail.From = new MailAddress("admin@shop.com");
            mail.Subject = "Order Successfully Placed!!";
            string Body = "Hii..."+order.AspNetUser.Email + ",</br></br>Thank you for Shopping.<br> Your Order Id:" + order.OrderId + ".</br> Order Date: " + order.CreatedDate;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            client.Send(mail);
            Console.WriteLine("Sent");
            //Email obj = new Email
            //{
            //    Body = "Hi " + order.AspNetUser.Email + ",</br></br>Thank you for Shopping.<br> Your Order Id:" + order.OrderId + ".</br> Order Date: " + order.CreatedDate,
            //    From = "admin@BookMyTicket.com",
            //    Subject = "Booking Confirmation mail",
            //    To = b.Email
            //};
            //  Session["moviename"] = null;


            //if (ModelState.IsValid)
            //{
            //    MailMessage mail = new MailMessage();
            //    mail.To.Add(obj.To);
            //    mail.From = new MailAddress(obj.From);
            //    mail.Subject = obj.Subject;
            //    string Body = obj.Body;
            //    mail.Body = Body;
            //    mail.IsBodyHtml = true;
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = "	smtp.mailtrap.io";
            //    smtp.Port = 25;
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = new System.Net.NetworkCredential("0d641e048c0b8d", "8a0f3c27bd6ab2"); // Enter seders User name and password  
            //    smtp.EnableSsl = true;
            //    smtp.Send(mail);
            //    //  return View("Index", obj);
            //}
        }


        public void Save()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Productapi");
                response.Wait();
                var result = response.Result;
            }
        }

    }
}