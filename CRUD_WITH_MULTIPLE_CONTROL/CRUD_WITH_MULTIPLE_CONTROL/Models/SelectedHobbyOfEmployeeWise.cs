//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRUD_WITH_MULTIPLE_CONTROL.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SelectedHobbyOfEmployeeWise
    {
        public int SelectedHobbyId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> HobbyId { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Hobby Hobby { get; set; }
    }
}
