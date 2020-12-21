using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Models.DbEntities;

namespace DataAccess.Providers
{
    public class LinqDataProvider : IDataProvider, IDisposable, ILogger
    {
        private readonly DataContext readenDatabase;

        public LinqDataProvider(string connectionString)
        {
            readenDatabase = new DataContext(connectionString);
            Log(DateTime.Now, "Connected to server by LinqDataProvider");
        }

        public IEnumerable<Address> GetAddresses()
        {
            if (readenDatabase.DatabaseExists())
            {
                try
                {
                    Log(DateTime.Now, "GetAddresses() was invoked");
                    var res = readenDatabase.GetTable<Address>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    Log(DateTime.Now, $"Loading failed in GetAddresses() \n{ex.Message}");
                    throw new Exception($"Loading failed in GetAddresses() \n{ex.Message}");
                }
            }

            Log(DateTime.Now, "DB connection failed");
            throw new Exception("DB connection failed");
        }

        public IEnumerable<Customer> GetCustomers()
        {
            if (readenDatabase.DatabaseExists())
            {
                try
                {
                    Log(DateTime.Now, "GetCustomers() was invoked");
                    var res = readenDatabase.GetTable<Customer>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    Log(DateTime.Now, $"Loading failed in GetCustomers() \n{ex.Message}");
                    throw new Exception($"Loading failed in GetCustomers() \n{ex.Message}");
                }
            }

            Log(DateTime.Now, "DB connection failed");
            throw new Exception("DB connection failed");
        }

        public IEnumerable<CustomerAddress> GetCustomerAddresses()
        {
            if (readenDatabase.DatabaseExists())
            {
                try
                {
                    Log(DateTime.Now, "GetCustomerAddresses() was invoked");
                    var res = readenDatabase.GetTable<CustomerAddress>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    Log(DateTime.Now, $"Loading failed in GetCustomerAddresses() \n{ex.Message}");
                    throw new Exception($"Loading failed in GetCustomerAddresses() \n{ex.Message}");
                }
            }

            Log(DateTime.Now, "DB connection failed");
            throw new Exception("DB connection failed");
        }

        public IEnumerable<Product> GetProducts()
        {
            if (readenDatabase.DatabaseExists())
            {
                try
                {
                    Log(DateTime.Now, "GetProducts() was invoked");
                    var res = readenDatabase.GetTable<Product>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    Log(DateTime.Now, $"Loading failed in GetProducts() \n{ex.Message}");
                    throw new Exception($"Loading failed in GetProducts() \n{ex.Message}");
                }
            }

            Log(DateTime.Now, "DB connection failed");
            throw new Exception("DB connection failed");
        }

        public IEnumerable<ProductCategory> GetProductCategories()
        {
            if (readenDatabase.DatabaseExists())
                try
                {
                    Log(DateTime.Now, "GetProductCategories() was invoked");
                    var res = readenDatabase.GetTable<ProductCategory>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    Log(DateTime.Now, $"Loading failed in GetSalesOrderHeaders() \n{ex.Message}");
                    throw new Exception($"Loading failed in GetProductCategories() \n{ex.Message}");
                }

            throw new Exception("DB connection failed");
        }

        public IEnumerable<SalesOrderHeader> GetSalesOrderHeaders()
        {
            if (readenDatabase.DatabaseExists())
            {
                try
                {
                    Log(DateTime.Now, "GetSalesOrderHeaders() was invoked");
                    var res = readenDatabase.GetTable<SalesOrderHeader>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    Log(DateTime.Now, $"Loading failed in GetSalesOrderHeaders() \n{ex.Message}");
                    throw new Exception($"Loading failed in GetSalesOrderHeaders() \n{ex.Message}");
                }
            }

            Log(DateTime.Now, "DB connection failed");
            throw new Exception("DB connection failed");
        }

        public IEnumerable<SalesOrderDetail> GetSalesOrderDetails()
        {
            if (readenDatabase.DatabaseExists())
            {
                try
                {
                    Log(DateTime.Now, "GetSalesOrderDetails() was invoked");
                    var res = readenDatabase.GetTable<SalesOrderDetail>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    Log(DateTime.Now, $"Loading failed in GetSalesOrderDetails() \n{ex.Message}");
                    throw new Exception($"Loading failed in GetSalesOrderDetails() \n{ex.Message}");
                }
            }

            Log(DateTime.Now, "DB connection failed");
            throw new Exception("DB connection failed");
        }

        public void Dispose()
        {
            Log(DateTime.Now, "Disconnected from server as LinqDataProvider");
            readenDatabase.Dispose();
        }

        public void Log(DateTime date, string message)
        {
            var command = new SqlCommand("SendLog", readenDatabase.Connection as SqlConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("date", date));
            command.Parameters.Add(new SqlParameter("message", message));

            using (var scope = new TransactionScope())
            {
                command.ExecuteNonQuery();
                scope.Complete();
            }
        }

        ~LinqDataProvider()
        {
            Dispose();
        }
    }
}