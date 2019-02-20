using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class AdminapiController : ApiController
    {

        AkankshaEntities db = new AkankshaEntities();

        [HttpGet]
        public IHttpActionResult GetUsers(string SearchString)
        {

            var users = from s in db.AspNetUsers
                        select s;
            if (!String.IsNullOrEmpty(SearchString))
            {
                users = users.Where(s => s.UserName.Contains(SearchString)
                                       || s.Email.Contains(SearchString));
            }
            if (users.Count() == 0)
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpGet]
        public IHttpActionResult GetAdmin(string id)
        {
            AspNetUser Admin;
            Admin = db.AspNetUsers.SingleOrDefault(u => u.Id == id);
            if (Admin == null)
            {
                return NotFound();
            }

            return Ok(Admin);
        }


        [HttpPost]
        public IHttpActionResult PostAdmin(AspNetUser admin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");

            }

            var userInDb = db.AspNetUsers.Single(u => u.Id == admin.Id);
            userInDb.Email = admin.UserName;
            userInDb.UserName = admin.UserName;
            userInDb.PhoneNumber = admin.PhoneNumber;
            db.SaveChanges();


            return Ok();

        }
    }

}
