//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RTech.Demo.WdmsData
{
    using System;
    using System.Collections.Generic;
    
    public partial class iclock_syncq
    {
        public int id { get; set; }
        public string obj { get; set; }
        public int op { get; set; }
        public string cnt { get; set; }
        public System.DateTime time { get; set; }
        public string upk_id { get; set; }
        public int company_id { get; set; }
    
        public virtual company company { get; set; }
        public virtual iclock_client iclock_client { get; set; }
    }
}
