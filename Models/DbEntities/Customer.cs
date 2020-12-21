using System;
using System.Data.Linq.Mapping;
using ConfigurationManager.Attributes;

namespace Models.DbEntities
{
    [Table(Name = "SalesLT.Customer")]
    public class Customer
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
        public int CustomerID { get; set; }

        [Column(Name = "NameStyle", CanBeNull = false)]
        public bool NameStyle { get; set; }

        [Column(Name = "Title")] public string Title { get; set; }

        [Column(Name = "FirstName")] public string FirstName { get; set; }

        [Column(Name = "MiddleName")] public string MiddleName { get; set; }

        [Column(Name = "LastName")] public string LastName { get; set; }

        [Column(Name = "Suffix")] public string Suffix { get; set; }

        [Column(Name = "CompanyName")] public string CompanyName { get; set; }

        [Column(Name = "SalesPerson")] public string SalesPerson { get; set; }

        [Column(Name = "EmailAddress")] public string EmailAddress { get; set; }

        [Column(Name = "Phone")] public string Phone { get; set; }

        [Column(Name = "PasswordHash")] public string PasswordHash { get; set; }

        [Column(Name = "PasswordSalt")] public string PasswordSalt { get; set; }

        [Column(Name = "rowguid")]
        [ParseIgnore]
        public Guid rowguid { get; set; }

        [Column(Name = "ModifiedDate")]
        [ParseIgnore]
        public DateTime ModifiedDate { get; set; }
    }
}