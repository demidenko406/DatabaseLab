using System.Collections.Generic;
using Models.DbEntities;

namespace DataAccess.Providers
{
    public enum ProviderType
    {
        LinqProvider,
        SqlProvider
    }

    public interface IDataProvider
    {
        IEnumerable<ProductCategory> GetProductCategories();
        IEnumerable<Product> GetProducts();
        IEnumerable<Customer> GetCustomers();
        IEnumerable<CustomerAddress> GetCustomerAddresses();
        IEnumerable<Address> GetAddresses();
        IEnumerable<SalesOrderDetail> GetSalesOrderDetails();
        IEnumerable<SalesOrderHeader> GetSalesOrderHeaders();
    }
}