using System;
using System.Data.Linq.Mapping;
using ConfigurationManager.Attributes;

namespace Models.DbEntities
{
    [Table(Name = "SalesLT.CustomerAddress")]
    public class CustomerAddress
    {
        [Column(Name = "CustomerID", CanBeNull = false)]
        public int CustomerID { get; set; }

        [Column(Name = "AddressID", CanBeNull = false)]
        public int AddressID { get; set; }

        [Column(Name = "AddressType", CanBeNull = false)]
        public string AddressType { get; set; }

        [Column(Name = "rowguid")]
        [ParseIgnore]
        public Guid rowguid { get; set; }

        [Column(Name = "ModifiedDate")]
        [ParseIgnore]
        public DateTime ModifiedDate { get; set; }
    }
}