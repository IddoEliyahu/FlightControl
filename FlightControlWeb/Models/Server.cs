﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControl
{
    public class Server
    {

            [Required]

            public string ServerID { get; set; }

            [Required]

            public string ServerURL { get; set; }

     
    }
}
