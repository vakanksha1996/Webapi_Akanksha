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
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Http;

namespace Akanksha.Controllers
{
    public class ProductController : Controller
    {
        private AkankshaEntities db;
        
        public ProductController()
        {
            db = new Akanksha.AkankshaEntities();
        }



        // GET: Product
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            IEnumerable<Product> products;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_desc" : "Price";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

          using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Productapi?searchString="+ searchString);
                responseTask.Wait();

                var result = responseTask.Result;
              
                    var readTask = result.Content.ReadAsAsync<IList<Product>>();
                    readTask.Wait();
                    products = readTask.Result;
                                
          }
           
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "Price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:  // Name ascending 
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }



        [HttpPost]
        public JsonResult GetProducts(string keyword)
        {

            List<string> productname;
            IEnumerable<Product> products;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Productapi?searchString=" +keyword);
                responseTask.Wait();

                var result = responseTask.Result;

                var readTask = result.Content.ReadAsAsync<IList<Product>>();
                readTask.Wait();
                 products = readTask.Result;

            }
            productname = products.Where(x => x.Name.ToLower().StartsWith(keyword.ToLower())).Select(y => y.Name).ToList();
            return  Json(productname,JsonRequestBehavior.AllowGet);
             
        }



        public ActionResult New()
        {
            ProductViewModel pvm;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Productapi?productid=");
                responseTask.Wait();
                var result = responseTask.Result;
                var readTask = result.Content.ReadAsAsync<ProductViewModel>();
                 pvm = readTask.Result;
                     
          
            }
            return View(pvm);
                                                                                                                                                                                                                                                                                                  

        }



        public ActionResult Edit(int Id)
        {
           // var product = db.Products.Single(p => p.ProductId == Id);
          //  var parentids = db.Subcategories.Where(p => p.ParentId != null).Select(p => p.ParentId).ToList();
           

            ProductViewModel pvm;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Productapi?productid="+Id);
                responseTask.Wait();
                var result = responseTask.Result;
                var readTask = result.Content.ReadAsAsync<ProductViewModel>();
                pvm = readTask.Result;

                if (pvm.Product == null)
                {
                    throw new Exception();
                }


            }
           
            return View(pvm);
        }



        public ActionResult Save(Product product)
        {
           // var parentids = db.Subcategories.Where(p => p.ParentId != null).Select(p => p.ParentId).ToList();
            ProductViewModel pvm;
            if (product.ProductId == 0)
            {
                if (ModelState.IsValid)
                {
                    product.CreatedDate = DateTime.Now;

                    using(var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Productapi");
                        var responseTask = client.PostAsJsonAsync<Product>("Productapi",product);
                        responseTask.Wait();

                        var result = responseTask.Result;
                       

                    }

                    //db.Products.Add(product);
                    //db.SaveChanges();

                }

                else
                {
                    //var vm = new ProductViewModel
                    //{
                    //    Product=product,
                    //    SubcategoryList = db.Subcategories
                    //         .Where(e => !parentids.Contains(e.SubcategoryId)).ToList()


                    //};
                    using(var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/");
                        var responseTask = client.GetAsync("Productapi?productid=");
                        responseTask.Wait();
                        var result = responseTask.Result;
                        var readTask = result.Content.ReadAsAsync<ProductViewModel>();
                        readTask.Wait();
                        pvm = readTask.Result;
                    }
                   
                    return View("New",pvm);
                }
              
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Productapi");
                        var putTask = client.PutAsJsonAsync("Productapi", product);
                        putTask.Wait();
                        var result = putTask.Result;

                    }
                    //var dbproduct = db.Products.Single(p => p.ProductId == product.ProductId);
                    //dbproduct.Name = product.Name;
                    //dbproduct.ModifiedDate = DateTime.Now;
                    //dbproduct.Pic = product.Pic;
                    //dbproduct.Description = product.Description;
                    //dbproduct.Price = product.Price;
                    //dbproduct.NumberOfStock = product.NumberOfStock;
                    //dbproduct.SubcategoryId = product.SubcategoryId;
                    //db.SaveChanges();

                }
                else
                {
                    //var vm = new ProductViewModel
                    //{
                    //    Product = product,
                    //    SubcategoryList = db.Subcategories
                    //        .Where(e => !parentids.Contains(e.SubcategoryId)).ToList()


                    //};
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/");
                        var responseTask = client.GetAsync("Productapi?productid=");
                        responseTask.Wait();
                        var result = responseTask.Result;
                        var readTask = result.Content.ReadAsAsync<ProductViewModel>();
                        readTask.Wait();
                        pvm = readTask.Result;
                    }

                    return View("Edit", pvm);
                }
               
            }
            return RedirectToAction("Index");
        }



        public JsonResult Delete(int Id)
        {
            //var product = db.Products.Single(p => p.ProductId == Id);
            
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

                var responseTask = client.GetAsync("Productapi?Id=" + Id);
                responseTask.Wait();
                var result = responseTask.Result;

                if (!result.IsSuccessStatusCode)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (var cclient = new HttpClient())
                    {
                        cclient.BaseAddress = new Uri("http://localhost:55437/api/");
                        var Task = client.DeleteAsync("Productapi?Id=" + Id);
                        Task.Wait();
                        var resultt = Task.Result;
                        if (resultt.IsSuccessStatusCode)
                        {
                            return Json(true, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);

                        }

                    }
                }

            }
           
          
        }



        public ActionResult GetProductsByName(string name,string sortOrder, string currentFilter, string searchString, int? page) 
        {
                  
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameDescParm = "name_desc";
            ViewBag.NameAsenParm = "name_asen";
            ViewBag.PriceAsenParm = "Price";
            ViewBag.PriceDescParm = "price_desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            IEnumerable<Product> products;
            Subcategory parentcategory;
            using(var client  = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Subcategoryapi?SearchString");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Subcategory>>();
                    readTask.Wait();
                    var sub = readTask.Result;
                    parentcategory = sub.Single(s => s.Name == name);

                }
                else
                {
                    parentcategory = null;
                }
            }
                //products = (from s in db.Products.Where(p => p.Subcategory.Name == name)
                //            select s);
                using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Productapi?searchString");
                responseTask.Wait();
                var result = responseTask.Result;
                var readTask = result.Content.ReadAsAsync<IList<Product>>();
                products = readTask.Result;

            }

            products = products.Where(p => p.Subcategory.Name == name).ToList();
                if (products.Count() == 0)
                {
                   return RedirectToAction("GetSubcategoriesByCategoryName", "SubCategory", new {CategoryName=name, ParentId =  parentcategory.SubcategoryId});
                }


            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));

            }
            switch (sortOrder)
            {
                case "name_asen":

                    products = products.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:  // Name ascending 
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));

           
        }



        public ActionResult ProductPage(int id)
        {
            IEnumerable<Cart> carts;
            Product product;
            string Id = User.Identity.GetUserId();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Cartapi");
                responseTask.Wait();
                var result = responseTask.Result;
                var readTask = result.Content.ReadAsAsync<IList<Cart>>();
                carts = readTask.Result;
            }
            ViewBag.IsItemaddedtoCart = carts.Count(p => p.ProductId == id && p.Id==Id);

            Session["isitemadded"] = carts.Count(p => p.ProductId == id && p.Id == Id);
            using(var client = new HttpClient())
            {


                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Productapi?Id="+id);
                responseTask.Wait();
                var result = responseTask.Result;
                var readTask = result.Content.ReadAsAsync<Product>();
                readTask.Wait();
                product = readTask.Result;

            }
            //var product = db.Products.Single(p => p.ProductId == id);
            return View(product);
        }


        [Authorize]
        public ActionResult CheckOut(Address address,int productid)
         {
            //string id = User.Identity.GetUserId();

            //var cvm = new BuyViewModel
            //{
            //    Product = db.Products.Single(p => p.ProductId == productid),
            //    States = db.States.ToList(),
            //    PaymentModes = db.Payments.ToList(),
            //    AddressBook = db.Addresses.Where(a => a.Id == id).ToList()
            //};

            //if (address.AddressId != 0)
            //{
            //    if (address.City == null)
            //    {
            //        cvm.address = db.Addresses.Single(a => a.AddressId == address.AddressId);

            //    }
            //    else
            //    {
            //        var addressindb = db.Addresses.Single(a => a.AddressId == address.AddressId);
            //        addressindb.HouseNo = address.HouseNo;
            //        addressindb.City = address.City;
            //        addressindb.Colony_Street = address.Colony_Street;
            //        addressindb.StateId = address.StateId;
            //        addressindb.Pincode = address.Pincode;
            //        db.SaveChanges();
            //        cvm.address = addressindb;
            //    }
            //}

            //if (address.City != null && address.AddressId == 0)
            //{
            //    var user = db.AspNetUsers.Single(u => u.Id == id);
            //    address.Id = user.Id;
            //    db.Addresses.Add(address);
            //    db.SaveChanges();
            //    cvm.address = address;
            //}
            //return View(cvm);

            string id = User.Identity.GetUserId();
            BuyViewModel cvm;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Productapi?productidtobeby=" + productid + "&userid=" +id+"&addressid=");
                responseTask.Wait();
                var result = responseTask.Result;
                var readTask = result.Content.ReadAsAsync<BuyViewModel>();
                readTask.Wait();
                cvm = readTask.Result;
            }

            if (address.AddressId != 0)
            {
                if (address.City == null)
                {
                    cvm.address = cvm.AddressBook.Single(a => a.AddressId == address.AddressId);

                }
                else
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Addressapi");
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
            }

            if (address.City != null && address.AddressId == 0)
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
                        using(var clientt = new HttpClient())
                        {
                            clientt.BaseAddress = new Uri("http://localhost:55437/api/");
                            var responsetask = clientt.GetAsync("Addressapi?userid="+id);
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



        public ActionResult OrderCompletion(int addressId, Order Order,int productId)
        {
                 string id = User.Identity.GetUserId();
            Address address;
            AspNetUser user;
            Product Product;
            Payment payment;


            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("Addressapi?addressid=" + addressId.ToString());
                responsetask.Wait();
                var result = responsetask.Result;
                var read = result.Content.ReadAsAsync<Address>();
                read.Wait();
                address = read.Result;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("Productapi?Id=" +productId.ToString());
                responsetask.Wait();
                var result = responsetask.Result;
                var read = result.Content.ReadAsAsync<Product>();
                read.Wait();
                Product = read.Result;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("NormalUserapi?id="+id);
                responsetask.Wait();
                var result = responsetask.Result;
                var read = result.Content.ReadAsAsync<AspNetUser>();
                read.Wait();
                user = read.Result;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = client.GetAsync("Paymentapi?paymentid="+Order.PaymentId);
                responsetask.Wait();
                var result = responsetask.Result;
                var read = result.Content.ReadAsAsync<Payment>();
                read.Wait();
                payment = read.Result;
            }

           
            
                Order.CreatedDate = DateTime.Now;
               // Order.PaymentId = payment.PaymentId;
                Order.Id = user.Id;
               // Order.Subtotal = Order.ItemQuantity * Product.Price;
            Order.Subtotal = CalculateSubtotal(Order.ItemQuantity, Product.Price);
                Order.ShippingAddress = address.HouseNo + "\n" + address.Colony_Street + "\n" +
                      address.City + "\n " + address.State.Name + "\n " + address.Pincode;
            
          
            //   var  address = add;

            BuyViewModel bvm;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Productapi?productidtobeby="+Product.ProductId.ToString()+"&userid="+id+"&addressid="+addressId.ToString());
                response.Wait();
                var result = response.Result;
                var read = result.Content.ReadAsAsync<BuyViewModel>();
                read.Wait();
                bvm = read.Result;
            }

        //    bvm.address = address;
           bvm.Order = Order;
            //var bvm = new BuyViewModel
            //{
            //    Product = Product,
            //    address = address,
            //    AddressBook = db.Addresses.ToList(),
            //    States = db.States.ToList(),
            //    PaymentModes = db.Payments.ToList(),
            //    Order = Order

            //};
            if (!ModelState.IsValid)
            {
               

                return View("CheckOut", bvm);

            }
            else
            {
                if (Product.NumberOfStock == 0)
                {
                    ModelState.AddModelError("Order.ItemQuantity", "Out Of Stock");
                    return View("CheckOut", bvm);

                }

                if (Product.NumberOfStock < Order.ItemQuantity)
                {
                    
                   ModelState.AddModelError("Order.ItemQuantity", "Only " + Product.NumberOfStock + " stock is left");

                    return View("CheckOut", bvm);
                }
            }

           // db.Orders.Add(Order);

            using(var client = new HttpClient())
            {
                 client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.PostAsJsonAsync<Order>("Orderapi/PostOrder",Order);
                response.Wait();
                var result = response.Result;
                
            }

            Customer existingcustomer = null;
            using(var client = new HttpClient())
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

           // var existingcustomer = db.Customers.SingleOrDefault(c => c.Id == user.Id);
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

                using(var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:55437/api/Customerapi");
                    var response = client.PostAsJsonAsync<Customer>("Customerapi", customer);
                    response.Wait();

                    var result = response.Result;
                }
               // db.Customers.Add(customer);
            }

            
           // db.SaveChanges();
           //using(var client = new HttpClient())
           //  {
           //     client.BaseAddress = new Uri("http://localhost:55437/api/");
           //     var response = client.GetAsync("Productapi");
           //     response.Wait();
           //     var result = response.Result;


           // }

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Orderapi");
                response.Wait();
                var result = response.Result;
                var read = result.Content.ReadAsAsync<IList<Order>>();
                read.Wait();
                IEnumerable<Order> orders = read.Result;
                Order = orders.Where(o => o.Id == id).OrderBy(o=>o.OrderId).Last();
            }
            OrderDetail orderdetail = new OrderDetail
                {

                    ProductId = Product.ProductId,
                    OrderId = Order.OrderId,
                    Quantity = Order.ItemQuantity ,
                    TotalPrice = Product.Price * Order.ItemQuantity

                };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.PostAsJsonAsync<OrderDetail>("Orderapi/PostOrderDetails", orderdetail);
                response.Wait();

                var result = response.Result;
            }

            //  Product.NumberOfStock = Product.NumberOfStock - Order.ItemQuantity;

               DecreaseNumberOfStock(Product, Order.ItemQuantity);

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:55437/api/");
            //    var response = client.GetAsync("Productapi");
            //    response.Wait();
            //    var result = response.Result;


            //}
            Order.Payment = payment;
            Order.AspNetUser = user;

            var email = new CartController();
                email.SendEmail(Order);

            return View(Order);
        }



        //[Authorize]
        //public ActionResult SelectShippingAddress(int Id)
        //{
        //    var product = db.Products.Single(p => p.ProductId == Id);
        //    string id = User.Identity.GetUserId();
        //    var user = db.AspNetUsers.Single(u => u.Id == id);
        //    var svm = new ShippingViewModel
        //    {
        //        User = user,
        //        Product = product,
        //        Addresses = db.Addresses.Where(u=>u.AspNetUser.Id==id).ToList(),
        //        States = db.States.ToList()

        //    };

        //    return View(svm);
        //}


        public ActionResult EditShippingAddress(int id, int productid)
         {
            //var address = db.Addresses.Single(a => a.AddressId == id);
            //var product = db.Products.Single(p => p.ProductId == productid);

            string userid = User.Identity.GetUserId();
            BuyViewModel bvm;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Productapi?productidtobeby="+productid+"&userid="+userid+"&addressid=" + id.ToString());
                response.Wait();
                var result = response.Result;
                var read = result.Content.ReadAsAsync<BuyViewModel>();
                bvm = read.Result;
            }

            //var bvm = new BuyViewModel
            //{
            //    Product = product,
            //    address = address,
            //    AddressBook = db.Addresses.ToList(),
            //    States = db.States.ToList(),
            //    PaymentModes = db.Payments.ToList(),
                

            //};
            return View(bvm);
        }

        
        //public ActionResult SelectPaymentMode(ShippingViewModel svm)
        //{
        //    var ovm =  new OrderViewModel();
        //    if (svm.ShippingAddress == null)
        //    {
        //        svm.NewAddress.Id = svm.User.Id;
        //        db.Addresses.Add(svm.NewAddress);
        //        db.SaveChanges();
        //        ovm.ShippingAddress = svm.NewAddress;

        //    }
        //    else
        //    {
        //      var   oldshippingaddress = db.Addresses.Single(a => a.AddressId == svm.ShippingAddress.AddressId);
        //        oldshippingaddress.City = svm.ShippingAddress.City;
        //        oldshippingaddress.Colony_Street = svm.ShippingAddress.Colony_Street;
        //        oldshippingaddress.HouseNo = svm.ShippingAddress.HouseNo;
        //        oldshippingaddress.Pincode = svm.ShippingAddress.Pincode;
        //        oldshippingaddress.StateId = svm.ShippingAddress.StateId;
        //        db.SaveChanges();
        //        ovm.ShippingAddress = oldshippingaddress;

        //    }
           
        //    var product = db.Products.Single(p => p.ProductId == svm.Product.ProductId);
        //    var user = db.AspNetUsers.Single(u => u.Id == svm.User.Id);
        //    ovm.Product = product;
        //    ovm.User = user;
        //    ovm.PaymentList = db.Payments.ToList();
        //    return View(ovm);
        //}

            
        
        public ActionResult GetProductByNameOrCategory(string searchkey, string name, string sortOrder, string currentFilter, string searchString, int? page)
       
        {
              ViewBag.searchkey = searchkey;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameDescParm = "name_desc";
            ViewBag.NameAsenParm = "name_asen";
            ViewBag.PriceAsenParm = "Price";
            ViewBag.PriceDescParm = "price_desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            IEnumerable<SearchProcedure_Result> products;

            using(var client  = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var response = client.GetAsync("Productapi?searchkey=" + searchkey);
                response.Wait();
                var result = response.Result;
                var readtask = result.Content.ReadAsAsync<IList<SearchProcedure_Result>>();
                readtask.Wait();
                products = readtask.Result;
            }
            
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));

            }
            switch (sortOrder)
            {
                case "name_asen":

                    products = products.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:  // Name ascending 
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));

        }

        public decimal CalculateSubtotal(int quantity,decimal ProductPrice)
        {

            return quantity * ProductPrice;
           
        }

        public void DecreaseNumberOfStock(Product product,int quantity)
        {
            Product producttobebuy = new Product()
            {
                ProductId = product.ProductId,
                Price = product.Price,
                Description = product.Description,
                DiscountRate = product.DiscountRate,
                Pic = product.Pic,
                NumberOfStock = product.NumberOfStock,
                SubcategoryId = product.SubcategoryId,
                Name = product.Name

            };

            producttobebuy.NumberOfStock = producttobebuy.NumberOfStock - quantity;

            using (var c = new HttpClient())
            {
                c.BaseAddress = new Uri("http://localhost:55437/api/");
                var responsetask = c.PutAsJsonAsync("Productapi", producttobebuy);
                responsetask.Wait();
                var r = responsetask.Result;

            }

        }
    }
      
    
}