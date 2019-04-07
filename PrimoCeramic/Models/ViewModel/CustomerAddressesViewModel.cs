using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class CustomerAddressesViewModel
    {

        

        public ApplicationUser ApplicationUser{get;set;}

        public List<CustomerAddress> CustomerAdresses { get; set; }

    }
}
