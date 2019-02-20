








using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class CustomerapiController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();

        [HttpGet]
        public IHttpActionResult GetCustomer(string id)
        {
            Customer customer;

            customer = db.Customers.SingleOrDefault(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpGet]
        public IHttpActionResult UpdateCustomer(string UserId, string ccn)
        {
            Customer customerInDb = db.Customers.SingleOrDefault(c => c.Id == UserId);
            if (customerInDb == null)
            {
                return NotFound();
            }

            customerInDb.CreditCardNumber = ccn;
          
            db.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            db.Customers.Add(customer);
            db.SaveChanges();
            return Ok();
        }
   
    }
}
