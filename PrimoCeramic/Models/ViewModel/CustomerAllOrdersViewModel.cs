using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class CustomerAllOrdersViewModel
    {
        public List<OrderViewModelWithId> OrderViewModelWithId { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public bool IsSent { get; set; }

        public bool IsPaid { get; set; }

        public bool Filter { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Date { get; set; }

       
    }
}
