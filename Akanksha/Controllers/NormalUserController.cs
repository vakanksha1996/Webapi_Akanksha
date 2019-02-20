using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Akanksha.ViewModel;
using System.Web.Http;
using System.Net.Http;
using System.Collections;

namespace Akanksha.Controllers
{
    [System.Web.Mvc.Authorize]
    public class NormalUserController : Controller
    {
        private AkankshaEntities db;

        public NormalUserController()
        {
            db = new AkankshaEntities();
        }



        // GET: NormalUser
        public ActionResult Index()
        {
            return View();
        }


        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Account()
        {

            AspNetUser user;
            string id = User.Identity.GetUserId();
            using (var client =  new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("NormalUserapi?id="+id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<AspNetUser>();
                    readTask.Wait();

                    user = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    user = null;

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

            }
            //var user = db.AspNetUsers.SingleOrDefault(c => c.Id == id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                AspNetUser new_user = new AspNetUser();
                return View(new_user);
            }



        }

        
        public ActionResult AddPayment()
        {
            string id = User.Identity.GetUserId();
            Customer customer;

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

                var reponseTask = client.GetAsync("Customerapi?id=" + id);
                reponseTask.Wait();

                var result = reponseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Customer>();
                    readTask.Wait();
                    customer = readTask.Result;
                }
                else
                {
                    customer = null;
                    ModelState.AddModelError(string.Empty, "Server error.Please contact administrator.");
                }
            }
            return View(customer);
        }

        public ActionResult AddAddress()
        {
           // AddressViewModel avm;
            List<State> states;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

                var reponseTask = client.GetAsync("Addressapi");
                reponseTask.Wait();

                var result = reponseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<State>>();
                    readTask.Wait();
                   states    = readTask.Result;
                }
                else
                {
                    states = null;
                    ModelState.AddModelError(string.Empty, "Server error.Please contact administrator.");
                }
            }


            var avm = new AddressViewModel()
            {
                 states = states
        };
            
            return View(avm);
        }


        public ActionResult EditAddress(int id)
        {
            
             AddressViewModel avm;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

                var reponseTask = client.GetAsync("Addressapi?id="+id.ToString());
                reponseTask.Wait();

                var result = reponseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<AddressViewModel>();
                    readTask.Wait();
                    avm = readTask.Result;
                }
                else
                {
                    avm = null;
                    ModelState.AddModelError(string.Empty, "Server error.Please contact administrator.");
                }
            }
            
            
            return View(avm);
        }

        public ActionResult SaveAddress(Address address)
        {
           
            if (address.AddressId == 0)
            {
                if (ModelState.IsValid)
                {
                    address.Id = User.Identity.GetUserId();
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Addressapi");

                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<Address>("Addressapi", address);
                        postTask.Wait();

                        var result = postTask.Result;
                      
                            return RedirectToAction("YourAddresses");
                       
                    }

                    // db.Addresses.Add(address);
                }
                else
                {
                    List<State> states;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/");

                        var reponseTask = client.GetAsync("Addressapi");
                        reponseTask.Wait();

                        var result = reponseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<List<State>>();
                            readTask.Wait();
                            states = readTask.Result;
                        }
                        else
                        {
                            states = null;
                            ModelState.AddModelError(string.Empty, "Server error.Please contact administrator.");
                        }
                    }

                    var avm = new AddressViewModel()
                    {
                        address = address,
                        states = states
                    };

                    return View("AddAddress", avm);

                }


            }
            else
            {

                //var addressInDb = db.Addresses.Single(a => a.AddressId == address.AddressId);
                //addressInDb.City = address.City;
                //addressInDb.HouseNo = address.HouseNo;
                //addressInDb.Colony_Street = address.Colony_Street;
                //addressInDb.StateId = address.StateId;
                //addressInDb.Pincode = address.Pincode;
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Addressapi");

                        //HTTP POST
                        var putTask = client.PutAsJsonAsync<Address>("Addressapi", address);
                        putTask.Wait();

                        var result = putTask.Result;


                        return RedirectToAction("YourAddresses");

                    }
                }
                else
                {
                    List<State> states;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/");

                        var reponseTask = client.GetAsync("Addressapi");
                        reponseTask.Wait();

                        var result = reponseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<List<State>>();
                            readTask.Wait();
                            states = readTask.Result;
                        }
                        else
                        {
                            states = null;
                            ModelState.AddModelError(string.Empty, "Server error.Please contact administrator.");
                        }
                    }

                    var avm = new AddressViewModel()
                    {
                        address = address,
                        states = states
                    };

                    return View("EditAddress", avm);

                }
                

            }
            //    db.SaveChanges();
            //    return RedirectToAction("YourAddresses");
        }


        public ActionResult YourAddresses()
        {
            IEnumerable<Address> addressbook;
            string id = User.Identity.GetUserId();
            //var addressbook = db.Addresses.Where(a => a.Id == id).ToList();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Addressapi?userid="+id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Address>>();
                    readTask.Wait();

                    addressbook = readTask.Result;
                }
                else
                {
                    addressbook =  Enumerable.Empty<Address>();

                 //   ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

                return View(addressbook);
        }




        public ActionResult EditProfile()
        {
            string id = User.Identity.GetUserId();
         //   var user = db.AspNetUsers.Single(u => u.Id == id);
         AspNetUser user;
            using (var client =  new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("NormalUserapi?id="+id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<AspNetUser>();
                    readTask.Wait();

                    user = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    user = null;

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

            }

            return View(user);
        }



        public ActionResult SaveProfile(AspNetUser user)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:55437/api/NormalUserapi");

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<AspNetUser>("NormalUserapi", user);
                    postTask.Wait();

                    var result = postTask.Result;

                    return View("Account", user);
                    
                }
                //var userInDb = db.AspNetUsers.Single(u => u.Id == user.Id);
                ////  userInDb.Email = user.Email;
                //userInDb.PhoneNumber = user.PhoneNumber;
                //userInDb.UserName = user.UserName;
                //userInDb.Email = user.UserName;
                //db.SaveChanges();

            }

            return View("EditProfile", user);
        }

        public JsonResult DeleteAddress(int Id)
        {
            Address address;
            // var address= db.Addresses.Single(p => p.AddressId == Id);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Addressapi?addressid="+Id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Address>();
                    readTask.Wait();

                    address = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    address = null;

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

            }
        

            if (address == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            //db.Addresses.Remove(address);
            //db.SaveChanges();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync("Addressapi?id="+address.AddressId.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
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
        
        [System.Web.Http.HttpPost]
        public JsonResult  SavePaymentCard(string UserId,string ccn)
        {

            if (UserId == null || ccn == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            //var  CustomerinDb = db.Customers.Single(c => c.Id == UserId);

            //CustomerinDb.CreditCardNumber = ccn;

            // db.SaveChanges();

            using (var client  = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

                var responseTask = client.GetAsync("Customerapi?UserId=" + UserId + "&ccn=" + ccn);
                responseTask.Wait();
                var result = responseTask.Result;
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
    }
}