using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Akanksha.Controllers
{
    public class HomeController : Controller
    {
        private AkankshaEntities db;
        public HomeController()
        {
        }
        public ActionResult Index()
        {
            IEnumerable<Subcategory> topcategories;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Subcategoryapi?SearchString=");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Subcategory>>();
                    readTask.Wait();

                    topcategories = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    topcategories = Enumerable.Empty<Subcategory>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

            }

            topcategories = topcategories.Where(s => s.ParentId == null).Take(20);

                return View(topcategories);
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}