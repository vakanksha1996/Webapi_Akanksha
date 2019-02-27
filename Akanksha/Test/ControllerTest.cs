using Akanksha.Api;
using Akanksha.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Akanksha.Test
{
    [TestClass]
    public class ControllerTest
    {
        [TestMethod]
       public void IsProductAlreadyAdded_ShouldReturnTrue()
        {
            

            //Arrange
            var cartcontroller = new CartController();



            //Act

            var result = cartcontroller.IsItemAlreadyAdded(GetTestCarts(),27);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsProductAlreadyAdded_ShouldReturnFalse()
        {


            //Arrange
            var cartcontroller = new CartController();



            //Act

            var result = cartcontroller.IsItemAlreadyAdded(GetTestCarts(), 10);

            //Assert
            Assert.IsFalse(result);
        }

       


        [TestMethod]
        public void FindTypeOfAddress_ShouldReturnAdressFromAddressbook()
        {
            //Arrange

            var cartcontroller = new CartController();
            var address = new Address()
            {
                AddressId = 1,
                //HouseNo = "Gl-25",
                //Colony_Street = "Takshshila colony",
                //StateId = 14,
                //City = "Indore",
                //Id = "71ee2849-c1e8-4e26-a45e-f72b445e6d2d",
                //Pincode = 452012
            };
            string expected = "Address_From_Addressbook";

            //Act
            var result = cartcontroller.FindTypeOfAddress(address);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FindTypeOfAddress_ShouldReturnUpdatedAddressFromAddressbook()
        {
            //Arrange

            var cartcontroller = new CartController();
            var address = new Address()
            {
                AddressId = 1,
                HouseNo = "Gl-25",
                Colony_Street = "Takshshila colony",
                StateId = 15,
                City = "Indore",
                Id = "71ee2849-c1e8-4e26-a45e-f72b445e6d2d",
                Pincode = "452012"
            };
            string expected = "Address_From_Addressbook_AfterEdit";

            //Act
            var result = cartcontroller.FindTypeOfAddress(address);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void FindTypeOfAddress_ShouldReturnNewAddress()
        {
            //Arrange

            var cartcontroller = new CartController();
            var address = new Address()
            {
                AddressId = 0,
                HouseNo = "112",
                Colony_Street = "Tilak Nagar",
                StateId = 15,
                City = "Indore",
                Id = "71ee2849-c1e8-4e26-a45e-f72b445e6d2d",
                Pincode = "452024"
            };
            string expected = "New Address";

            //Act
            var result = cartcontroller.FindTypeOfAddress(address);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CalculateSubTotal()
        {
            //Arrange
            var productcontroller = new ProductController();
            decimal  expected = 501;

            //Act
            var result = productcontroller.CalculateSubtotal(2, (decimal)250.50);

            //Assert
            Assert.AreEqual(expected, result);
           
        }

        private List<Cart> GetTestCarts()
        {
            var testcarts = new List<Cart>();
            testcarts.Add(new Cart { CartId = 67, Id = "1d42d5d2-7c67-44cc-b475-951796fae710", ProductId = 47 });
            testcarts.Add(new Cart { CartId = 68, Id = "1d42d5d2-7c67-44cc-b475-951796fae710", ProductId = 27 });
            testcarts.Add(new Cart { CartId = 73, Id = "1d42d5d2-7c67-44cc-b475-951796fae710", ProductId = 31 });

            return testcarts;
        }
    }
}