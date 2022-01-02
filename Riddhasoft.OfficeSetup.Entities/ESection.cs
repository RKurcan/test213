using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddhasoft.OfficeSetup.Entities
{
    public class ESection
    {
        [Key]
        public int Id { get; set; }
        [StringLength(10), Required]
        public string Code { get; set; }
        [StringLength(150), Required]
        public string Name { get; set; }
        [StringLength(300)]
        public string NameNp { get; set; }
        public int DepartmentId { get; set; }
        public int? BranchId { get; set; }
        public int? ParentId { get; set; }
        public string UnitCode { get; set; }
        public UnitType UnitType { get; set; }

        public virtual EBranch Branch { get; set; }
        public virtual EDepartment Department { get; set; }


    }

    public enum UnitType
    {
        Department = 1,
        Directorate = 2,
        Section = 3,
        Unit = 4
    }
}
