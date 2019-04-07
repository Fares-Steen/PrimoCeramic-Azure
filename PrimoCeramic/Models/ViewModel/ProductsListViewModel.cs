using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class ProductsListViewModel
    {
        public List<Products> Products{ get; set; }
        public IEnumerable<ProductTypes> ProductTypes { get; set; }
        public IEnumerable<SpecialTags> SpecialTags { get; set; }
        public IEnumerable<ProductApplication> ProductApplication { get; set; }
        public IEnumerable<ProductCountry> ProductCountry { get; set; }
        public IEnumerable<ProductSurface> ProductSurface { get; set; }


        public PagingInfo PagingInfo { get; set; }
        public int SelectedType { get; set; }
        public int SelectedTag { get; set; }

        public int SelectedApplication { get; set; }

        public int SelectedCountry { get; set; }

        public int SelectedSurface { get; set; }

        public string SearchName { get; set; }

    }
}
