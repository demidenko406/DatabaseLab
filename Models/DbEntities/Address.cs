using System;
using System.Data.Linq.Mapping;
using ConfigurationManager.Attributes;

namespace Models.DbEntities
{
    [Table(Name = "SalesLT.Address")]
    public class Address
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]

        public int AddressID { get; set; }

        [Column(Name = "AddressLine1")] public string AddressLine1 { get; set; }

        [Column(Name = "AddressLine2")] public string AddressLine2 { get; set; }

        [Column(Name = "City")] public string City { get; set; }

        [Column(Name = "StateProvince")] public string StateProvince { get; set; }

        [Column(Name = "CountryRegion")] public string CountryRegion { get; set; }

        [Column(Name = "PostalCode")] public string PostalCode { get; set; }

        [Column(Name = "rowguid")]
        [ParseIgnore]
        public Guid rowguid { get; set; }

        [Column(Name = "ModifiedDate")]
        [ParseIgnore]
        public DateTime ModifiedDate { get; set; }
    }
}