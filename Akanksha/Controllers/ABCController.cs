using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Controllers
{
    public class ABCController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();

     
        [HttpGet]
        public IQueryable<Subcategory> GetSubCategoryByCategoryName(string CategoryName,int? ParentId)
        {
            IQueryable<Subcategory> subcategories;
           // var categoryid = db.Categories.Single(c => c.Name == CategoryName);

            if (ParentId == null)
            {
                subcategories = db.Subcategories.Where(s => s.Category.Name==CategoryName && s.ParentId == null);

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
                return  Ok(subcategories);
            }
        }

        [HttpPost]
        public IHttpActionResult PostSubcategory(Subcategory subcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data...");
            }
            subcategory.CreatedDate = DateTime.Now;
            db.Subcategories.Add(subcategory);
            db.SaveChanges();

            return Created("http://localhost:55437/api/ABC", subcategory);
        }


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



    }
}
