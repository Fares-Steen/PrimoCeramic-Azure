using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class OrderDetailsViewModel
    {
        public Orders Orders { get; set; }

        public List<ProductsSelectedForOrder> ProductsSelectedForOrder { get; set; }
    }
}
