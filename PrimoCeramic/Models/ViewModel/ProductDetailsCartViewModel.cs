using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class ProductDetailsCartViewModel
    {
        public Products Products { get; set; }

        public double Quantity { get; set; }

        public List<ProductImages> ProductImages { get; set; }
    }
}
