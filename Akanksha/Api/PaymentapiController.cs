using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class PaymentapiController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();

        [HttpGet]
        public IHttpActionResult GetPaymentMode(int paymentid)
        {
            var paymentmode = db.Payments.SingleOrDefault(p => p.PaymentId == paymentid);
            if(paymentmode == null)
            {
                return BadRequest();
            }

            return Ok(paymentmode);
        }
    }
}
