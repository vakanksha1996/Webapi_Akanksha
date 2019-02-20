using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
   
    public class OrderapiController : ApiController
    {

        AkankshaEntities db = new AkankshaEntities();

      
      
      
        [HttpPost]
        public IHttpActionResult PostOrder(Order order)
        {
             if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data..");

            }

            db.Orders.Add(order);
            db.SaveChanges();
            return Ok();
        }
        
     
       [HttpPost]
        public IHttpActionResult PostOrderDetails(OrderDetail orderdetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");

            }

            db.OrderDetails.Add(orderdetail);
            db.SaveChanges();
            return Ok();
        }


        [HttpGet]
        public IHttpActionResult GetOrders()
        {
            IEnumerable<Order> orders = db.Orders.ToList();
            return Ok(orders);
        }


        [HttpPut]
        public IHttpActionResult PutOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();

            }

            var orderindb = db.Orders.Single(o => o.OrderId == order.OrderId);
            orderindb.ItemQuantity = order.ItemQuantity;
            orderindb.Subtotal = order.Subtotal;
            db.SaveChanges();

            return Ok();
        }

    }
}
