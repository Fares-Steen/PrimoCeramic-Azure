using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class Emails
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Map { get; set; }

        public int Port { get; set; }

        public string EmailType { get; set; }
    }
}
