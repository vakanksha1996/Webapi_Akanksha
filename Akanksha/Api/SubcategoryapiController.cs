using Akanksha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class SubcategoryapiController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();


        [HttpGet]
        public IQueryable<Subcategory> GetSubCategoryByCategoryName(string CategoryName, int? ParentId)
        {
            IQueryable<Subcategory> subcategories;
            // var categoryid = db.Categories.Single(c => c.Name == CategoryName);

            if (ParentId == null)
            {
                subcategories = db.Subcategories.Where(s => s.Category.Name == CategoryName && s.ParentId == null);

            }
            else
            {
                subcategories = db.Subcategories.Where(s => s.ParentId == ParentId);


            }
            return subcategories;

        }


        [HttpGet]

        public IHttpActionResult GetSubcategories(string SearchString)
        {
            IEnumerable<Subcategory> subcategories;
            subcategories = from s in db.Subcategories
                            select s;
            if (!String.IsNullOrEmpty(SearchString))
            {
                subcategories = subcategories.Where(s => s.Name.ToLower().StartsWith(SearchString.ToLower()));


            }

            if (subcategories.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(subcategories);
            }
        }

        [HttpPost]
        public IHttpActionResult PostSubcategory(Subcategory subcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data...");
            }

            db.Subcategories.Add(subcategory);
            db.SaveChanges();

            return Created("http://localhost:55437/api/ABC", subcategory);
        }

        [HttpPut]
        public IHttpActionResult PutSubcategory(Subcategory subcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data..");
            }

            var subcategoryInDb = db.Subcategories.Single(s => s.SubcategoryId == subcategory.SubcategoryId);
            subcategoryInDb.Name = subcategory.Name;
            subcategoryInDb.Pic = subcategory.Pic;
            subcategoryInDb.Description = subcategory.Description;
            subcategoryInDb.ModifiedBy = User.Identity.Name;
            subcategoryInDb.ModifiedDate = DateTime.Now;
            subcategoryInDb.ParentId = subcategory.ParentId;
            subcategoryInDb.CategoryId = subcategory.CategoryId;

            db.SaveChanges();

            return Ok("Successfully updated");
        }

        [HttpDelete]
        public IHttpActionResult  DeleteSubcategory(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid student id");

            using (var ctx = new AkankshaEntities())
            {
                var student = ctx.Subcategories
                    .Where(s => s.SubcategoryId == id)
                    .FirstOrDefault();

                ctx.Entry(student).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok();

        }


        [HttpGet]
        public int  GetSubcategorycount(int? id,int? subcategoryid)
        {
            if (subcategoryid == null)
            {
                var categorycount = db.Subcategories.Count(s => s.CategoryId == id);
                return categorycount;

            }
            else
            {
                var subcategoriescount = db.Subcategories.Count(s => s.ParentId == subcategoryid);
                return subcategoriescount;
            }


        }



        [HttpGet] 
        public IHttpActionResult GetSubcategoryViewModel(int? subcategoryid)
        {
            Subcategory subcategory=null;
            if (subcategoryid != null)
            {
                subcategory = db.Subcategories.SingleOrDefault(s => s.SubcategoryId == subcategoryid);
            }
            var svm = new SubcategoryViewModel()
            {
                SubCategory = subcategory,
                DepartmentList = db.Categories.ToList(),
                CategoryList = db.Subcategories.ToList()
            };

            return Ok(svm);




        }

        [HttpGet]
        public IHttpActionResult GetSubcategory(int categoryid)
        {
            var category = db.Subcategories.Single(c => c.SubcategoryId == categoryid);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }


        [HttpGet]
        public IHttpActionResult GetParentId()
        {
            var parentids = db.Subcategories.Where(p => p.ParentId != null).Select(p => p.ParentId).ToList();

            return Ok(parentids);
        }




    }
}
