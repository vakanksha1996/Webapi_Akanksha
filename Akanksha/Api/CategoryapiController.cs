using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class CategoryapiController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();

        [HttpGet]
        public IHttpActionResult GetCategories()
        {
            IEnumerable<Category> categories;

            categories = db.Categories.ToList();

            if (categories.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(categories);
            }
        }

        [HttpGet]
        public IHttpActionResult GetCategory(int id)
        {
            Category category;
            category = db.Categories.SingleOrDefault(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(category);
            }
        }


        [HttpGet]
        public IHttpActionResult Getcategories(string SearchString)
        {

            IEnumerable<string> categoriesname;
          
                categoriesname = db.Categories.Where(x => x.Name.ToLower().StartsWith(SearchString.ToLower())).Select(y => y.Name).ToList();


            

            if (categoriesname.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(categoriesname);
            }
        }

        [HttpPost]
        public IHttpActionResult PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data..");
            }

            category.CreatedDate = DateTime.Now;
            db.Categories.Add(category);
            db.SaveChanges();

            return Created("http://localhost:55437/api/Categoryapi", category);
        }

        [HttpPut]
        public IHttpActionResult PutCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data...");

            }

            var CategoryInDb = db.Categories.Single(c => c.CategoryId == category.CategoryId);
            CategoryInDb.ModifiedDate = DateTime.Now;
            CategoryInDb.Name = category.Name;
            CategoryInDb.Pic = category.Pic;
            CategoryInDb.Description = category.Description;

            db.SaveChanges();

            return Ok("Successfully Updated..");
        }

        [HttpDelete]
        public IHttpActionResult DeleteCategory(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid student id");

            using (var ctx = new AkankshaEntities())
            {
                var category = ctx.Categories
                    .Where(s => s.CategoryId == id)
                    .FirstOrDefault();

                ctx.Entry(category).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok(true);

        }
    }
}
