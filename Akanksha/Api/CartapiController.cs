using Akanksha.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class CartapiController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();


        [HttpGet]
        public IHttpActionResult GetCarts()
        {
            var carts = db.Carts.ToList();
            return Ok(carts);
        }

        [HttpGet]
        public IHttpActionResult GetCarts(string userid)
        {
            var carts = db.Carts.Where(c => c.Id == userid).ToList();
            return Ok(carts);
        }


        [HttpGet]
        public IHttpActionResult GetCheckoutViewModel(string userid)
        {
            var cvm = new CheckOutViewModel
            {
                Cartitems = db.Carts.Where(c => c.Id == userid && c.Product.NumberOfStock != 0).ToList(),
                States = db.States.ToList(),
                PaymentModes = db.Payments.ToList(),
                AddressBook = db.Addresses.Where(a => a.Id == userid).ToList()
            };

            return Ok(cvm);


        }

        [HttpGet]
        public IHttpActionResult AddtoCart(string userid, int productid)
        {

            var product = db.Products.Single(p => p.ProductId == productid);
            var isalreadyadded = db.Carts.Count(c => c.ProductId == product.ProductId && c.Id == userid);
            if (isalreadyadded == 0)
            {

                return Ok(false);
            }

            else
            {
                return Ok(true);
            }
                        
        }


        [HttpGet]
        public IHttpActionResult GetCartById(int cartid)
        {
            var cart = db.Carts.SingleOrDefault(c => c.CartId == cartid);
            if(cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [HttpPost]
        public IHttpActionResult PostCartItem(Cart cartitem)
        {

            var product = db.Products.Single(p => p.ProductId == cartitem.ProductId);
            cartitem.Amount = product.Price;
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");

            }

            db.Carts.Add(cartitem);
            db.SaveChanges();

            return Ok();
        }


       [HttpGet]
        public IHttpActionResult UpdateQuantityOfCartItem(int cartid,int qty)
        {
            var cart = db.Carts.SingleOrDefault(c => c.CartId == cartid);
            if (cart == null)
            {
                return Ok(false);
            }

            else
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid Data");
                }
                else
                {
                    cart.Quantity = qty;
                    db.SaveChanges();
                    return Ok(true);

                }
               
            }
        }


        [HttpDelete]
        public IHttpActionResult Deletecartitem(int cartid)
        {
            var cartitem = db.Carts.SingleOrDefault(c => c.CartId == cartid);
            if(cartitem == null)
            {
                return NotFound();

            }

            db.Carts.Remove(cartitem);
            db.SaveChanges();
            return Ok();
        }




    }
}