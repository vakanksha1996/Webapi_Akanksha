using Akanksha.Models;
using Akanksha.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class ProductapiController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();

        [HttpGet]
        public IHttpActionResult GetProductCount(string CategoryName)
        {
           var  productcount = db.Products.Count(p => p.Subcategory.Name == CategoryName);

            if (productcount == 0)
            {
                return NotFound();
            }

            return Ok(productcount);
        }


        [HttpGet]
        public IHttpActionResult GetProductById(int Id)
        {
            var product = db.Products.SingleOrDefault(p => p.ProductId == Id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet]
        public IHttpActionResult GetProducts(string searchString)
            {
               
       
            var products = db.Products.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.ToLower().StartsWith(searchString.ToLower())).ToList();

            }


            return Ok(products);
        }


        [HttpGet]
        public IHttpActionResult GetProductsbyNameAndCategory(string searchkey)
        {
            var products = from s in db.SearchProcedure(searchkey).ToList()
                           select s;

            return Ok(products);
        }

      [HttpGet]
      public IHttpActionResult GetProductViewModel(int? productid)
        {
            var parentids = db.Subcategories.Where(p => p.ParentId != null).Select(p => p.ParentId).ToList();

            var pvm = new ProductViewModel
            {
                Product = db.Products.SingleOrDefault(p => p.ProductId == productid),
                SubcategoryList = db.Subcategories
                              .Where(e => !parentids.Contains(e.SubcategoryId)).ToList()


            };

            return Ok(pvm);
        }


        [HttpGet]
        public IHttpActionResult GetBuyViewModel(int productidtobeby,string userid,int? addressid)
        {
            var cvm = new BuyViewModel
            {
                
                Product = db.Products.Single(p => p.ProductId == productidtobeby),
                States = db.States.ToList(),
                PaymentModes = db.Payments.ToList(),
                AddressBook = db.Addresses.Where(a => a.Id == userid).ToList()
            };
            if (addressid != null)
            {
                cvm.address = db.Addresses.Single(a => a.AddressId == addressid);
            }
            
            return Ok(cvm);
        }

        [HttpPost]
        public IHttpActionResult PostProduct(Product product)
        {
            if(!ModelState.IsValid){
                return BadRequest("Invalid Data");
            }

            db.Products.Add(product);
            db.SaveChanges();
            return Ok();

        }

        [HttpPut]
        public IHttpActionResult PutProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");

            }

            var dbproduct = db.Products.SingleOrDefault(p => p.ProductId == product.ProductId);
            if (dbproduct == null)
            {
                return NotFound();
            }
            
            dbproduct.Name = product.Name;
            dbproduct.ModifiedDate = DateTime.Now;
            dbproduct.Pic = product.Pic;
            dbproduct.DiscountRate = product.DiscountRate;
            dbproduct.Description = product.Description;
            dbproduct.Price = product.Price;
            dbproduct.NumberOfStock = product.NumberOfStock;
            dbproduct.SubcategoryId = product.SubcategoryId;
            db.SaveChanges();

            return Ok();

        }

        [HttpDelete]
        public IHttpActionResult DeleteProduct(int Id)
        {
            var product = db.Products.SingleOrDefault(p => p.ProductId == Id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                db.Products.Remove(product);
                db.SaveChanges();
                return Ok();
            }
        }

        [HttpGet]
        public IHttpActionResult Save()
        {

        
            db.SaveChanges();

            return Ok();
        }
    }


}
