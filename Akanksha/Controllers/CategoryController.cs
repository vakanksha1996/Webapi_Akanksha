using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Akanksha.Controllers
{
    public class CategoryController : Controller
    {
        private AkankshaEntities db;
        public CategoryController()
        {
            db = new AkankshaEntities();
        }


        // GET: Category
        [HttpGet]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            IEnumerable<Category> categories;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Categoryapi");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Category>>();
                    readTask.Wait();

                    categories = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    categories = Enumerable.Empty<Category>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(s => s.Name.ToLower().StartsWith(searchString.ToLower()));

            }
            switch (sortOrder)
            {
                case "name_desc":
                    categories = categories.OrderByDescending(s => s.Name);
                    break;

                default:  // Name ascending 
                    categories = categories.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(categories.ToPagedList(pageNumber, pageSize));
        }



        public JsonResult GetCategories(string keyword)
        {
            IEnumerable<string> categories;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Categoryapi?SearchString="+keyword);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<string>>();
                    readTask.Wait();

                    categories = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    categories = Enumerable.Empty<string>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return Json(categories, JsonRequestBehavior.AllowGet);
        }



        public ActionResult New()
        {
            
            return View();
        }



        [HttpGet]
        public ActionResult GetCategories()
        {
            
            IEnumerable<Category> categories = new List<Category>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Categoryapi");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Category>>();
                    readTask.Wait();

                    categories = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    categories = Enumerable.Empty<Category>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(categories);
        }



        public ActionResult Save(Category category)
        {
           
            
                if (category.CategoryId == 0)
                {
                    if (ModelState.IsValid)
                    {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Categoryapi");

                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<Category>("Categoryapi", category);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

                }
                    else
                    {
                        return View("New");
                    }


                }
                else
                {
                    if (ModelState.IsValid)
                    {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Categoryapi");

                        //HTTP POST
                        var putTask = client.PutAsJsonAsync<Category>("Categoryapi", category);
                        putTask.Wait();

                        var result = putTask.Result;
                        if (result.IsSuccessStatusCode)
                        {

                            return RedirectToAction("Index");
                        }
                    }

                }
                    else
                    {
                        return View("Edit", category);
                    }

                }
            return RedirectToAction("Index");

      }
          
                
        public ActionResult Edit(int Id)
        {
            Category category;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Categoryapi?id="+Id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Category>();
                    readTask.Wait();

                    category = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    category = null;

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(category);
        }



        public JsonResult Delete(int Id)
        {
            Category department;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Categoryapi?id=" + Id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Category>();
                    readTask.Wait();

                    department= readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    department = null;

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            int  categorycount;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Subcategoryapi?id="+department.CategoryId);
                responseTask.Wait();

                var result = responseTask.Result;
               
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();

                    categorycount = readTask.Result;
               
            }
            if (department == null)
            {

                throw new Exception();
            }

            if(categorycount != 0)
            {
                return Json("alert", JsonRequestBehavior.AllowGet);
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync("Categoryapi/" + Id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
              
                    return Json("success", JsonRequestBehavior.AllowGet);

            

            }
        }
    }
}