//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FasheShop.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contact
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Message { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<System.DateTime> ArrivedTime { get; set; }
    
        public virtual State State { get; set; }
    }
}
