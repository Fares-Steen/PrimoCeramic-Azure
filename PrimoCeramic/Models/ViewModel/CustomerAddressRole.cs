using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class CustomerAddressRole
    {
        

        public IEnumerable<CustomerAddress> CustomerAddress { get; set; }

        public int SelectedRole { get; set; }
    }
}
