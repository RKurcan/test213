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
    using System.ComponentModel.DataAnnotations;
    
    public partial class userlist
    {
        [Key]
        public int userlist1 { get; set; }
        public int user { get; set; }
        public string name { get; set; }
        public string userpwd { get; set; }
        public string usercard { get; set; }
        public Nullable<int> userpri { get; set; }
        public string SN { get; set; }
    
        public virtual iclock iclock { get; set; }
        public virtual userinfo userinfo { get; set; }
    }
}