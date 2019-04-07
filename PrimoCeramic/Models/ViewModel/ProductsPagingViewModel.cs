using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class ProductsPagingViewModel
    {
        public List<Products> Products { get; set; }
        public IEnumerable<SpecialTags> SpecialTags { get; set; }
        public int SelectedTag { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public double Quantity { get; set; }
        public int Id { get; set; }
       
    }
}
