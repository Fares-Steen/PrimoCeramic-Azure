using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class Orders
    {

        public int Id { get; set; }

        [Display(Name = "Person")]
        public string PersonID { get; set; }

        [ForeignKey("PersonID")]
        public virtual ApplicationUser Person { get; set; }

        //[Display(Name = "Address")]
        //public int AddressId { get; set; }

        //[ForeignKey("AddressId")]
        //public virtual CustomerAddress CustomerAddress { get; set; }

       
        public string Country { get; set; }


        
        public string City { get; set; }

        
        public string Address { get; set; }

        
        public string Phone { get; set; }

        public DateTime OrderDate { get; set; }

        [NotMapped]
        public DateTime OrderTime { get; set; }

        public double Vat { get; set; }

        public string OrderStatus { get; set; }

        public bool IsSent { get; set; }

        public bool IsPaid { get; set; }


    }
}
