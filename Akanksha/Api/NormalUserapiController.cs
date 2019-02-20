using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class NormalUserapiController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();

        [HttpGet]
        public IHttpActionResult GetUserById(string id)
        {
          
          var   user = db.AspNetUsers.SingleOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public IHttpActionResult PostUser(AspNetUser user)
        {
            var userInDb = db.AspNetUsers.Single(u => u.Id == user.Id);
            if (userInDb == null)
            {
                return NotFound();
            }
            //  userInDb.Email = user.Email;
            userInDb.PhoneNumber = user.PhoneNumber;
            userInDb.UserName = user.UserName;
            userInDb.Email = user.UserName;
            db.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            var users = db.AspNetUsers.ToList();
            return Ok(users);
        }
    }
}
