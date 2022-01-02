using Riddhasoft.OfficeSetup.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTech.Demo.Models
{
    public class DropdownViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameNp { get; set; }
        public bool IsManager { get; set; }
        public int? ParentId { get; set; }
        public int UnitType { get; set; }
        public string UnitTypeName { get { return ((UnitType)this.UnitType).ToString(); } }
        public bool Checked{ get; set; }

        public List<DropdownViewModel> Children { get; set; }

    }
}