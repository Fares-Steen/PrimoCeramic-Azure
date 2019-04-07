﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class PagingInfo
    {
        public int TotalItem { get; set; }

        public int ItemPerPage { get; set; }

        public int CurrentPage { get; set; }


        public int TotalPage => (int)Math.Ceiling((decimal)TotalItem / ItemPerPage);



        public string urlParam { get; set; }
    }
}
