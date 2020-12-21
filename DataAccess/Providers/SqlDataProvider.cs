using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using ConfigurationManager.Parsers;
using Models.DbEntities;

namespace DataAccess.Providers
{
    public class SqlDataProvider : IDataProvider, IDisposable, ILogger
    {
        private readonly SqlConnection connection;

        public SqlDataProvider(string connectionString)
        {
            using (var scope = new TransactionScope())
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                Log(DateTime.Now, "Connected to server by LinqDataProvider");
                scope.Complete();
            }
        }

        public IEnumerable<ProductCategory> GetProductCategories()
        {
            try
            {
                Log(DateTime.Now, "GetProductCategories() was invoked");
                var command = new SqlCommand("GetProductCategories", connection);
                command.CommandType = CommandType.StoredProcedure;
                using (var scope = new TransactionScope())
                {
                    var ans = Map<ProductCategory>(command.ExecuteReader());
                    scope.Complete();
                    return ans;
                }
            }
            catch (Exception ex)
            {
                Log(DateTime.Now, $"Loading failed in GetProductCategories() \n{ex.Message}");
                throw ex;
            }
        }

        public IEnumerable<Product> GetProducts()
        {
            try
            {
                Log(DateTime.Now, "GetProducts() was invoked");
                var command = new SqlCommand("GetProducts", connection);
                command.CommandType = CommandType.StoredProcedure;
                using (var scope = new TransactionScope())
                {
                    var ans = Map<Product>(command.ExecuteReader());
                    scope.Complete();
                    return ans;
                }
            }
            catch (Exception ex)
            {
                Log(DateTime.Now, $"Loading failed in GetProducts() \n{ex.Message}");
                throw ex;
            }
        }

        public IEnumerable<Customer> GetCustomers()
        {
            try
            {
                Log(DateTime.Now, "GetCustomers() was invoked");
                var command = new SqlCommand("GetCustomers", connection);
                command.CommandType = CommandType.StoredProcedure;
                using (var scope = new TransactionScope())
                {
                    var ans = Map<Customer>(command.ExecuteReader());
                    scope.Complete();
                    return ans;
                }
            }
            catch (Exception ex)
            {
                Log(DateTime.Now, $"Loading failed in GetCustomers() \n{ex.Message}");
                throw ex;
            }
        }

        public IEnumerable<CustomerAddress> GetCustomerAddresses()
        {
            try
            {
                Log(DateTime.Now, "GetCustomerAddresses() was invoked");
                var command = new SqlCommand("GetCustomerAddresses", connection);
                command.CommandType = CommandType.StoredProcedure;
                using (var scope = new TransactionScope())
                {
                    var ans = Map<CustomerAddress>(command.ExecuteReader());
                    scope.Complete();
                    return ans;
                }
            }
            catch (Exception ex)
            {
                Log(DateTime.Now, $"Loading failed in GetCustomerAddresses() \n{ex.Message}");
                throw ex;
            }
        }

        public IEnumerable<Address> GetAddresses()
        {
            try
            {
                Log(DateTime.Now, "GetAddresses() was invoked");
                var command = new SqlCommand("GetAddresses", connection);
                command.CommandType = CommandType.StoredProcedure;
                using (var scope = new TransactionScope())
                {
                    var ans = Map<Address>(command.ExecuteReader());
                    scope.Complete();
                    return ans;
                }
            }
            catch (Exception ex)
            {
                Log(DateTime.Now, $"Loading failed in GetAddresses() \n{ex.Message}");
                throw ex;
            }
        }

        public IEnumerable<SalesOrderDetail> GetSalesOrderDetails()
        {
            try
            {
                Log(DateTime.Now, "GetSalesOrderDetails() was invoked");
                var command = new SqlCommand("GetSalesOrderDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                using (var scope = new TransactionScope())
                {
                    var ans = Map<SalesOrderDetail>(command.ExecuteReader());
                    scope.Complete();
                    return ans;
                }
            }
            catch (Exception ex)
            {
                Log(DateTime.Now, $"Loading failed in GetSalesOrderDetails() \n{ex.Message}");
                throw ex;
            }
        }

        public IEnumerable<SalesOrderHeader> GetSalesOrderHeaders()
        {
            try
            {
                Log(DateTime.Now, "GetSalesOrderHeaders() was invoked");
                var command = new SqlCommand("GetSalesOrderHeaders", connection);
                command.CommandType = CommandType.StoredProcedure;
                using (var scope = new TransactionScope())
                {
                    var ans = Map<SalesOrderHeader>(command.ExecuteReader());
                    scope.Complete();
                    return ans;
                }
            }
            catch (Exception ex)
            {
                Log(DateTime.Now, $"Loading failed in GetSalesOrderHeaders() \n{ex.Message}");
                throw ex;
            }
        }

        public void Dispose()
        {
            //connection.Close();
        }

        public void Log(DateTime date, string message)
        {
            var command = new SqlCommand("SendLog", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("date", date));
            command.Parameters.Add(new SqlParameter("message", message));

            using (var scope = new TransactionScope())
            {
                command.ExecuteNonQuery();
                scope.Complete();
            }
        }

        private List<Dictionary<string, object>> Parse(SqlDataReader reader)
        {
            var ans = new List<Dictionary<string, object>>();
            while (reader.Read())
            {
                var dict = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    var val = reader.GetValue(i);
                    dict.Add(name, val);
                }

                ans.Add(dict);
            }

            reader.Close();
            return ans;
        }

        private List<T> Map<T>(SqlDataReader reader)
        {
            var parsed = Parse(reader);
            var ans = new List<T>();
            foreach (var dict in parsed) ans.Add(ModelCreator.CreateInstanse<T>(dict));
            return ans;
        }

        ~SqlDataProvider()
        {
            Dispose();
        }
    }
}