//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Akanksha
{
    using System;
    using System.Collections.Generic;
    
    public partial class Address
    {
        public int AddressId { get; set; }
        public string HouseNo { get; set; }
        public string Colony_Street { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string Id { get; set; }
        public int StateId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual State State { get; set; }
    }
}
