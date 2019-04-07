using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class CustomerShoppingCart
    {
        public int Id { get; set; }

        [Display(Name = "Person")]
        public string PersonID { get; set; }

        [ForeignKey("PersonID")]
        public virtual ApplicationUser Person { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }

        public double Quantity { get; set; }

       
    }
}
