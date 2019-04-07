using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class OrderViewModelWithId
    {
        public List<OrderViewModel> OrderViewModel { get; set; }

        public int Id { get; set; }
    }
}
