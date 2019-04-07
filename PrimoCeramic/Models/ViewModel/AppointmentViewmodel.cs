using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class AppointmentViewmodel
    {

        public List<Appointments> Appointments { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
