﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTech.Demo.Models
{
    public class CheckBoxModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }

        public List<CheckBoxModel> Children { get; set; }
    }
}