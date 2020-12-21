namespace Models
{
    public class DetailedSummary
    {
        public int SalesOrderID { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSize { get; set; }
        public int OwnerId { get; set; }
        public string OwnerTitle { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerMiddleName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerSuffix { get; set; }
        public string OwnerCompanyName { get; set; }
        public string OwnerSalesPerson { get; set; }
        public string OwnerEmailAddress { get; set; }
        public string OwnerPhone { get; set; }
        public short Status { get; set; }
        public string ShipMethod { get; set; }
        public string ShipAddressCity { get; set; }
        public string ShipAddressLine { get; set; }
    }
}