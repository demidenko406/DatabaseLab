using System;
using System.Data.Linq.Mapping;
using ConfigurationManager.Attributes;

namespace Models.DbEntities
{
    [Table(Name = "SalesLT.Product")]
    public class Product
    {
        [Column(Name = "ProductID", IsPrimaryKey = true, CanBeNull = false)]
        public int ProductID { get; set; }

        [Column(Name = "Name")] public string Name { get; set; }

        [Column(Name = "ProductNumber")] public string ProductNumber { get; set; }

        [Column(Name = "Color")] public string Color { get; set; }

        [Column(Name = "Size")] public string Size { get; set; }

        [Column(Name = "ProductCategoryID")] public int? ProductCategoryID { get; set; }

        [Column(Name = "SellStartDate")] public DateTime SellStartDate { get; set; }

        [Column(Name = "rowguid", CanBeNull = false)]
        [ParseIgnore]
        public Guid rowguid { get; set; }

        [Column(Name = "ModifiedDate")]
        [ParseIgnore]
        public DateTime ModifiedDate { get; set; }
    }
}