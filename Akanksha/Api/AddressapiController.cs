using Akanksha.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Akanksha.Api
{
    public class AddressapiController : ApiController
    {
        AkankshaEntities db = new AkankshaEntities();

        [HttpGet]
        public IHttpActionResult GetStates()
        {
            IEnumerable<State> states;
            states = db.States.ToList();

            if (states.Count() == 0)
            {
                return NotFound();
            }

            return Ok(states);
        }


        [HttpGet]
        public IHttpActionResult GetAddressesByUserId(string userid)
        {
             IEnumerable<Address> addressbook = db.Addresses.Where(a => a.Id == userid).ToList();

            if (addressbook.Count() == 0)
            {
                return NotFound();
            }

            return Ok(addressbook);
        

        }

        [HttpGet]
        public IHttpActionResult GetAddress(int addressid)
        {
            var address= db.Addresses.SingleOrDefault(a => a.AddressId == addressid);
            if (address == null)
            {
                return NotFound();
            }

             return Ok(address);
        }

        [HttpGet]
        public IHttpActionResult GetAddressViewModel(int id)
        {
          
             Address  address = db.Addresses.SingleOrDefault(a => a.AddressId == id);

            if (address == null)
            {
                return NotFound();
            }
            var avm = new AddressViewModel()
            {
                address = address,
                states = db.States.ToList()
            };

            return Ok(avm);


           
        }

        [HttpPost]
        public IHttpActionResult PostAddress(Address address)
        {



            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }




            db.Addresses.Add(address);
            db.SaveChanges();
             

            return Ok();
        }

        [HttpPut]
        public IHttpActionResult PutAddress(Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");

            }

            var addressInDb = db.Addresses.Single(a => a.AddressId == address.AddressId);
            addressInDb.City = address.City;
            addressInDb.HouseNo = address.HouseNo;
            addressInDb.Colony_Street = address.Colony_Street;
            addressInDb.StateId = address.StateId;
            addressInDb.Pincode = address.Pincode;

            db.SaveChanges();
            return Ok();
        }

        [HttpDelete] 
        public IHttpActionResult DeleteAddress(int id)
        {
            var address = db.Addresses.SingleOrDefault(a => a.AddressId == id);

            if (address == null)
            {
                return NotFound();
            }

            db.Addresses.Remove(address);
            db.SaveChanges();
            return  Ok();
        }
    }
}
