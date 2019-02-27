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
      //  private AkankshaEntities db;


        public CartController()
        {
           // db = new AkankshaEntities();
        }


        // GET: Cart
        public ActionResult Index()
        {
          return View();
        }



        public ActionResult GetCartItemList()
        {
            
            string Id = User.Identity.GetUserId();
      
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
            IList<Cart> carts;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Cartapi");
                var result = response.Result;
                var read = result.Content.ReadAsAsync<IList<Cart>>();
                carts = read.Result;
            }

            carts = carts.Where(c => c.Id == id).ToList();

            var isalreadyadded = IsItemAlreadyAdded(carts, productid);

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

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:55437/api/");
                    var response = client.PostAsJsonAsync("Cartapi", cart);
                    response.Wait();
                    var r = response.Result;

                }

                return Json(true, JsonRequestBehavior.AllowGet);

            }



         }



        public JsonResult RemovefromCart(int id)

        {

            using (var client = new HttpClient())
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

            cart.Quantity = qty;

            Cart updatedcart = new Cart()
            {
                CartId = cart.CartId,
                ProductId = cart.ProductId,
                Id = cart.Id,
                Quantity = cart.Quantity,
                Amount = cart.Amount
            };
           // cart.Quantity = qty;
            //using(var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:55437/api/");
            //    var responsetask = client.GetAsync("Cartapi?cartid=" + cart.CartId.ToString() + "&qty=" + qty.ToString());
            //    responsetask.Wait();
            //    var result = responsetask.Result;
            //    var read = result.Content.ReadAsAsync<bool>();
            //    read.Wait();
            //    if (read.Result == true)
            //    {
            //        return Json(true, JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {
            //        return Json(false, JsonRequestBehavior.AllowGet);
            //    }
            //}

            using(var client =new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.PutAsJsonAsync("Cartapi", updatedcart);
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                   return  Json(false, JsonRequestBehavior.AllowGet);
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


            var addresstype = FindTypeOfAddress(address);


            if (addresstype == "Address_From_Addressbook")
            {
                cvm.address = cvm.AddressBook.Single(a => a.AddressId == address.AddressId);
            }


            else if (addresstype == "Address_From_Addressbook_AfterEdit")
            {
                using (var client = new HttpClient())
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
            }


            else if (addresstype == "New Address")
            {
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

            else

                cvm.address = null;

           
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

            Address address;
            AspNetUser user;
            List<Cart> cartitems;
            CheckOutViewModel cvm;
         

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
          
            Order.CreatedDate = DateTime.Now;
            Order.ShippingAddress = address.HouseNo + "\n" + address.Colony_Street + "\n" +
                   address.City + "\n " + address.State.Name + "\n " + address.Pincode;

            
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

          
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.PostAsJsonAsync<Order>("Orderapi/PostOrder", Order);
                response.Wait();
                var result = response.Result;

            }
           
            var flag  = CheckIfNewCustomer(user.Id);
           
            if(flag == true)
            {
                Customer customer = new Customer()
                {
                    Name = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Id,
                    Id = user.Id

                };

              

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:55437/api/Customerapi");
                    var response = client.PostAsJsonAsync<Customer>("Customerapi", customer);
                    response.Wait();

                    var result = response.Result;
                }

            }

           

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

                 
                  var isproductavailable =  checkProductAvailability((int)item.ProductId,item.Quantity);
               
                    if (isproductavailable == false)
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
                        return Content(item.Product.Name + " is out of stock");
                    }
                    else
                    {
                    using (var c = new HttpClient())
                    {
                        c.BaseAddress = new Uri("http://localhost:55437/api/");
                        var responsetask = c.PostAsJsonAsync("orderapi/PostOrderDetails", orderdetail);
                        responsetask.Wait();
                        var r = responsetask.Result;

                    }

                    var productcontroller = new ProductController();

                    productcontroller. DecreaseNumberOfStock(item.Product,item.Quantity);
                   
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
           
        }

        [NonAction]
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

        [NonAction]
        public bool IsItemAlreadyAdded(IList<Cart> CurrentUserCart,int productid)
        {
            
            var count = CurrentUserCart.Count(c => c.ProductId == productid);

            if (count == 0)
                return false;

            return true;
            
         }


        [NonAction] 
        public string FindTypeOfAddress(Address address)
        {
            if(address.AddressId != 0)
            {
                if(address.City == null)
                {
                    return "Address_From_Addressbook";
                }

                else
                {
                    return "Address_From_Addressbook_AfterEdit";
                }
            }

            else
            {
                if(address.City != null)
                {
                    return "New Address";
                }
                else
                {
                    return "Yet_Not_Selected";
                }
            }
        }

        [NonAction]
        public bool CheckIfNewCustomer(string userid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Customerapi?id=" + userid);
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsAsync<Customer>();
                    read.Wait();
                    var existingcustomer = read.Result;
                    if (existingcustomer == null)
                        return true;
                   
                }
            }

            return false;
        }


        [NonAction]
        public bool checkProductAvailability(int productid,int quantity)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Productapi?Id=" +productid);
                response.Wait();
                var result = response.Result;
                var read = result.Content.ReadAsAsync<Product>();
                read.Wait();

                var producttobebuy = read.Result;

                if (producttobebuy.NumberOfStock > quantity)
                    return true;

                return false;


            }

            
        }

      

    }
}