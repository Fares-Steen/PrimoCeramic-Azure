using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class CustomerAddressWithMessage
    {
        public List<CustomerAddress> CustomerAdresses { get; set; }
        public string Message { get; set; }
    }
}
