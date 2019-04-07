using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class ProductsSelectedForOrder
    {


        public int Id { get; set; }
        

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Orders Orders { get; set; }



        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }

        public double Quantity { get; set; }

        public double Prise { get; set; }

        public double Vat { get; set; }


    }
}
