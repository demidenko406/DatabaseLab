using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Providers;
using Models;
using Models.DbEntities;

namespace ServiceLayer.API
{
    public class Database : IRepository, IDisposable
    {
        private readonly IDataProvider provider;

        public Database(string connectionString, ProviderType type)
        {
            provider = type == ProviderType.LinqProvider
                ? new LinqDataProvider(connectionString)
                : (IDataProvider) new SqlDataProvider(connectionString);
            Update();
        }

        public void Dispose()
        {
            if (provider is IDisposable)
                ((IDisposable) provider).Dispose();
        }

        public void Update()
        {
            try
            {
                Customers = provider.GetCustomers();
                CustomerAddresses = provider.GetCustomerAddresses();
                Addresses = provider.GetAddresses();
                Products = provider.GetProducts();
                ProductCategories = provider.GetProductCategories();
                SalesOrderDetails = provider.GetSalesOrderDetails();
                SalesOrderHeaders = provider.GetSalesOrderHeaders();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occured in Database.Update()\n{ex.Message}");
            }
        }

        public IEnumerable<Customer> Customers { get; private set; }
        public IEnumerable<CustomerAddress> CustomerAddresses { get; private set; }
        public IEnumerable<Address> Addresses { get; private set; }
        public IEnumerable<Product> Products { get; private set; }
        public IEnumerable<ProductCategory> ProductCategories { get; private set; }
        public IEnumerable<SalesOrderDetail> SalesOrderDetails { get; private set; }
        public IEnumerable<SalesOrderHeader> SalesOrderHeaders { get; private set; }
        public IEnumerable<DetailedSummary> DetailedSummaries { get; private set; }

        public IEnumerable<Product> GetProducts(int categoryId)
        {
            var responce = Products.Where(e => e.ProductCategoryID == categoryId);
            return responce.Count() == 0 ? null : responce;
        }

        public void CalculateSummary()
        {
            var list = new List<DetailedSummary>();
            foreach (var order in SalesOrderDetails)
            {
                var temp = new DetailedSummary();
                temp.SalesOrderID = order.SalesOrderID;
                temp.ProductId = order.ProductID;
                var product = GetProduct(temp.ProductId);
                temp.ProductName = product.Name;
                temp.ProductNumber = product.ProductNumber;
                temp.ProductSize = product.Size;
                temp.ProductCategory = product.ProductCategoryID.HasValue
                    ? GetProductCategory(product.ProductCategoryID.Value).Name
                    : null;
                var header = GetSalesOrderHeader(order.SalesOrderID);
                temp.Status = header.Status;
                temp.ShipMethod = header.ShipMethod;
                temp.OwnerId = header.CustomerID;
                var shipAddress = GetAddress(header.ShipToAddressID);
                temp.ShipAddressCity = shipAddress.City;
                temp.ShipAddressLine = shipAddress.AddressLine1;
                var owner = GetCustomer(header.CustomerID);
                temp.OwnerTitle = owner.Title;
                temp.OwnerFirstName = owner.FirstName;
                temp.OwnerLastName = owner.LastName;
                temp.OwnerMiddleName = owner.MiddleName;
                temp.OwnerSuffix = owner.Suffix;
                temp.OwnerSalesPerson = owner.SalesPerson;
                temp.OwnerCompanyName = owner.CompanyName;
                temp.OwnerPhone = owner.Phone;
                temp.OwnerCompanyName = owner.CompanyName;
                temp.OwnerEmailAddress = owner.EmailAddress;
                list.Add(temp);
            }

            DetailedSummaries = list;
        }

        public DetailedSummary GetDetailedSummary(int id)
        {
            foreach (var summary in DetailedSummaries)
                if (summary.SalesOrderID == id)
                    return summary;
            return null;
        }

        public Customer GetCustomer(int id)
        {
            foreach (var el in Customers)
                if (el.CustomerID == id)
                    return el;
            return null;
        }

        public Address GetCustomerAddress(int customerId)
        {
            foreach (var el in CustomerAddresses)
                if (el.CustomerID == customerId)
                    return GetAddress(el.AddressID);
            return null;
        }

        public Address GetAddress(int id)
        {
            foreach (var el in Addresses)
                if (el.AddressID == id)
                    return el;
            return null;
        }

        public Product GetProduct(int id)
        {
            foreach (var el in Products)
                if (el.ProductID == id)
                    return el;
            return null;
        }

        public ProductCategory GetProductCategory(int id)
        {
            foreach (var el in ProductCategories)
                if (el.ProductCategoryID == id)
                    return el;
            return null;
        }

        public SalesOrderHeader GetSalesOrderHeader(int id)
        {
            foreach (var el in SalesOrderHeaders)
                if (el.SalesOrderID == id)
                    return el;
            return null;
        }

        ~Database()
        {
            Dispose();
        }
    }
}