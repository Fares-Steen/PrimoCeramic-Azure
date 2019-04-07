using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class ProductsViewModel
    {
        public Products Products { get;set; }

        public IEnumerable<ProductTypes> ProductTypes { get; set; }
        public IEnumerable<SpecialTags> SpecialTags { get; set; }
        public IEnumerable<ProductApplication> ProductApplication { get; set; }
        public IEnumerable<ProductCountry> ProductCountry { get; set; }
        public IEnumerable<ProductSurface> ProductSurface { get; set; }
        public List<ProductImages> ProductImages { get; set; }
        public int SelectedRole { get; set; }

        

       

    }
}
