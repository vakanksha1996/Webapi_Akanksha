using Akanksha.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Akanksha.Controllers
{
    public class SubCategoryController : Controller
    {
       // private AkankshaEntities db;

        public SubCategoryController()
        {
           // db = new AkankshaEntities();
        }


        // GET: SubCategory
        [HttpGet]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            IEnumerable<Subcategory> subcategories;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
          //  ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

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

                var responseTask = client.GetAsync("Subcategoryapi?SearchString="+searchString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Subcategory>>();
                    readTask.Wait();

                    subcategories = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    subcategories = Enumerable.Empty<Subcategory>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            switch (sortOrder)
            {
                case "name_desc":
                    subcategories = subcategories.OrderByDescending(s => s.Name);
                    break;
               
                default:  // Name ascending 
                    subcategories = subcategories.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(subcategories.ToPagedList(pageNumber, pageSize));
        }



        public JsonResult Getsubcategories(string keyword)
        {
           IEnumerable<Subcategory> subcategories;
            List<string> Name;


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET

                var responseTask = client.GetAsync("Subcategoryapi?SearchString="+keyword);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Subcategory>>();
                    readTask.Wait();

                    subcategories = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    subcategories = Enumerable.Empty<Subcategory>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

            }
            Name = subcategories.Select(s => s.Name).ToList();



            return Json(Name, JsonRequestBehavior.AllowGet); 
        }




        public ActionResult New()

        { 
            SubcategoryViewModel svm;
            //var vm = new SubcategoryViewModel
            //{
            //    DepartmentList = db.Categories.ToList(),
            //    CategoryList = db.Subcategories.ToList()
                             

            //};

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Subcategoryapi?subcategoryid=");
                responseTask.Wait();
                var result = responseTask.Result;
             
                 var readTask = result.Content.ReadAsAsync<SubcategoryViewModel>();
                  readTask.Wait();
                 svm = readTask.Result;
                          
           }
            return View(svm);
        }



        [HttpPost]
        public ActionResult Save(Subcategory subcategory)
        {
           
            if (subcategory.SubcategoryId == 0)
            {
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Subcategoryapi");

                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<Subcategory>("Subcategoryapi", subcategory);
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
                    //var vm = new SubcategoryViewModel
                    //{
                    //    SubCategory = subcategory,
                    //    DepartmentList = db.Categories.ToList(),
                    //    CategoryList = db.Subcategories.ToList()


                    //};
                    SubcategoryViewModel svm;
                    using(var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/");
                        var responseTask = client.GetAsync("Subcategoryapi?subcategoryid=" + subcategory.SubcategoryId);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        var readTask = result.Content.ReadAsAsync<SubcategoryViewModel>();
                        svm = readTask.Result;
                    }
                    return View("New",svm);
                }
               


            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/Subcategoryapi");

                        //HTTP POST
                        var putTask = client.PutAsJsonAsync<Subcategory>("Subcategoryapi", subcategory);
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
                    SubcategoryViewModel svm;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:55437/api/");
                        var responseTask = client.GetAsync("Subcategoryapi?subcategoryid=" + subcategory.SubcategoryId);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        var readTask = result.Content.ReadAsAsync<SubcategoryViewModel>();
                        svm = readTask.Result;
                    }
                    return View("Edit", svm);
                }
               

            }
            return RedirectToAction("Index", "SubCategory");
            
        }



        public ActionResult Edit(int id)
        {
            TempData.Remove("ProductAlert");
            TempData.Remove("SubcategoryAlert");
            SubcategoryViewModel svm;
            //var subcategory = db.Subcategories.Single(s => s.SubcategoryId == id);
            //var vm = new SubcategoryViewModel
            //{
            //    SubCategory=subcategory,
            //    DepartmentList = db.Categories.ToList(),
            //    CategoryList = db.Subcategories.
            //                 ToList()

            //};

        
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

              var  responseTask = client.GetAsync("Subcategoryapi?subcategoryid=" + id.ToString());
                responseTask.Wait();
                var result = responseTask.Result;
                var readTask = result.Content.ReadAsAsync<SubcategoryViewModel>();
                svm = readTask.Result;
            }
            return View(svm);
        }



        public JsonResult Delete(int Id)
        {
            //var category = db.Subcategories.Single(c => c.SubcategoryId == Id);
            //var productcount = db.Products.Count(p => p.Subcategory.Name == category.Name);
            //var subcategoriescount = db.Subcategories.Count(s => s.ParentId == category.SubcategoryId);
            Subcategory category=null;
            int productcount;
            int subcategoriescount;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Subcategoryapi?categoryid=" + Id.ToString());
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Subcategory>();
                    readTask.Wait();
                    category = readTask.Result;

                }
            }
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Productapi?CategoryName=" + category.Name);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    productcount = readTask.Result;
                }
                else
                {
                    productcount = 0;
                }

            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                var responseTask = client.GetAsync("Subcategoryapi?id=null&subcategoryid="+category.SubcategoryId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    subcategoriescount = readTask.Result;
                }
                else
                {
                    subcategoriescount = 0;
                }

            }
            if (category == null)
            {

                throw new Exception();
            }
            if (productcount != 0) {



                return Json("ProductAlert", JsonRequestBehavior.AllowGet);
            }
            if(subcategoriescount != 0)
            {

                return Json("SubcategoryAlert", JsonRequestBehavior.AllowGet);
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync("Subcategoryapi/" + Id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
              
                    return Json("success", JsonRequestBehavior.AllowGet);
              
            }

            
        }
        //    var categories = db.Subcategories.ToList();
        


        //public ActionResult  GetSubcategoriesByCategoryName(string CategoryName,int? ParentId)

        //{
        //    IEnumerable<Subcategory> subcategories;
        //    if (ParentId == null)
        //    {
        //         subcategories = db.Subcategories.Where(s => s.Category.Name == CategoryName && s.ParentId==null).ToList();

        //    }
        //    else
        //    {
        //        subcategories = db.Subcategories.Where(s => s.ParentId == ParentId).ToList();
                   
                
        //    }
        //    return View(subcategories);
        //}


        public ActionResult GetSubcategoriesByCategoryName(string CategoryName, int? ParentId)

        { 
           IEnumerable<Subcategory> subcategories;


        //    if (ParentId == null)
        //    {
        //        subcategories = db.Subcategories.Where(s => s.Category.Name == CategoryName && s.ParentId == null).ToList();

        //    }
        //    else
        //    {
        //        subcategories = db.Subcategories.Where(s => s.ParentId == ParentId).ToList();


        //    }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55437/api/");
                //HTTP GET
              
                var responseTask = client.GetAsync("Subcategoryapi?CategoryName="+CategoryName+"&ParentId="+ParentId);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Subcategory>>();
                    readTask.Wait();

                    subcategories = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    subcategories = Enumerable.Empty<Subcategory>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(subcategories);
        }



    }
}