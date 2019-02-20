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
    
    public partial class Product
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Picture { get; set; }
        public string Description { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<System.DateTime> ReceivedTime { get; set; }
        public Nullable<int> Sale { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<int> FeaturedProduct { get; set; }
        public Nullable<int> SaleTimeProduct { get; set; }
        public Nullable<System.DateTime> ExpiredDate { get; set; }
        public string language { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }
}
