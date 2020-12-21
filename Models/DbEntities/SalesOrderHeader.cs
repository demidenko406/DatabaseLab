using System;
using System.Data.Linq.Mapping;
using ConfigurationManager.Attributes;

namespace Models.DbEntities
{
    [Table(Name = "SalesLT.SalesOrderHeader")]
    public class SalesOrderHeader
    {
        [Column(Name = "SalesOrderID", IsPrimaryKey = true, CanBeNull = false)]
        public int SalesOrderID { get; set; }

        [Column(Name = "CustomerID")] public int CustomerID { get; set; }

        [Column(Name = "ShipToAddressID")] public int ShipToAddressID { get; set; }

        [Column(Name = "BillToAddressID")] public int BillToAddressID { get; set; }

        [Column(Name = "RevisionNumber")] public byte RevisionNumber { get; set; }

        [Column(Name = "OrderDate")]
        [ParseIgnore]
        public DateTime OrderDate { get; set; }

        [Column(Name = "DueDate")]
        [ParseIgnore]
        public DateTime DueDate { get; set; }

        [Column(Name = "Status")] public byte Status { get; set; }

        [Column(Name = "OnlineOrderFlag")] public bool OnlineOrderFlag { get; set; }

        [Column(Name = "SalesOrderNumber", IsDbGenerated = true)]
        public string SalesOrderNumber { get; set; }

        [Column(Name = "ShipMethod")] public string ShipMethod { get; set; }

        [Column(Name = "Comment")] public string Comment { get; set; }

        [Column(Name = "rowguid", CanBeNull = false)]
        [ParseIgnore]
        public Guid rowguid { get; set; }

        [Column(Name = "ModifiedDate", CanBeNull = false)]
        [ParseIgnore]
        public DateTime ModifiedDate { get; set; }
    }
}