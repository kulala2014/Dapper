using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServerDapper.BusinessObjects;

namespace SqlServerDapper
{
    [TestClass]
    public class AdventureWorksTests
    {
        [TestMethod]
        public void RunTests()
        {
            QuerySQL1();
            QuerySQL2();
            QueryMultipleSQL1();
            QueryMultipleSQL2();
            ToDataTable();
            ToEnumerable();
            ToDataSet();
            ExecuteScalarSQL();
            ExecuteSQL();
            ToProperties1();
            ToProperties2();
            ToProperties3();
            ToPropertiesToDataTable1();
            ToPropertiesToDataTable2();
            ToPropertiesToDataTable3();
        }

        private void QuerySQL1()
        {
            var products = SqlHelper.QuerySQL<Product>("select * from Production.Product where ProductID = @ProductID", new { ProductID = 1 });

            Console.WriteLine("Query 1");
            Console.WriteLine();
            foreach (var product in products)
                Console.WriteLine("{0,-2} {1}", product.ProductID, product.Name);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Count() == 1);
        }

        private void QuerySQL2()
        {
            var products = SqlHelper.QuerySQL<Product>("select top 4 * from Production.Product order by ProductID");

            Console.WriteLine("Query 2");
            Console.WriteLine();
            foreach (var product in products)
                Console.WriteLine("{0,-2} {1}", product.ProductID, product.Name);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Count() == 4);
        }

        private void QueryMultipleSQL1()
        {
            var results = SqlHelper.QueryMultipleSQL<Product, Person>(@"
                select top 4 * from Production.Product order by ProductID;
                select top 4 * from Person.Person order by BusinessEntityID;
            ");

            var products = results.Item1;
            var people = results.Item2;

            Console.WriteLine("Query Multiple 1 (Different Types of Result Sets)");
            Console.WriteLine();
            foreach (var product in products)
                Console.WriteLine("{0,-2} {1}", product.ProductID, product.Name);
            Console.WriteLine();
            foreach (var person in people)
                Console.WriteLine("{0,-2} {1} {2}", person.BusinessEntityID, person.FirstName, person.LastName);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsNotNull(people);
            Assert.IsTrue(products.Count() == 4);
            Assert.IsTrue(people.Count() == 4);
        }

        private void QueryMultipleSQL2()
        {
            var results = SqlHelper.QueryMultipleSQL<Product, Product, Product, Product, Product, Product, Product, Product, Product, Product, Product, Product, Product, Product>(@"
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
                select top 1 * from Production.Product order by ProductID;
            ");

            var productLists = new IEnumerable<Product>[] {
                results.Item1,
                results.Item2,
                results.Item3,
                results.Item4,
                results.Item5,
                results.Item6,
                results.Item7,
                results.Rest.Item1,
                results.Rest.Item2,
                results.Rest.Item3,
                results.Rest.Item4,
                results.Rest.Item5,
                results.Rest.Item6,
                results.Rest.Item7,
            };

            Console.WriteLine("Query Multiple 2 (14 Result Sets)");
            Console.WriteLine();

            int index = 0;
            foreach (var products in productLists)
            {
                var product = products.First();
                Console.WriteLine("Result Set {0,2}: {1,-2} {2}", (++index), product.ProductID, product.Name);
            }
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            foreach (var products in productLists)
            {
                Assert.IsNotNull(products);
                Assert.IsTrue(products.Count() == 1);
            }
        }

        private void ToDataTable()
        {
            var products = SqlHelper.QuerySQL<Product>("select top 4 * from Production.Product order by ProductID").ToDataTable();

            Console.WriteLine("To DataTable");
            Console.WriteLine();
            foreach (DataRow product in products.Rows)
                Console.WriteLine("{0,-2} {1}", product["ProductID"], product["Name"]);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Rows.Count == 4);
        }

        private void ToEnumerable()
        {
            var products = SqlHelper.QuerySQL<Product>("select top 4 * from Production.Product order by ProductID").ToDataTable().Cast<Product>();

            Console.WriteLine("To Enumerable");
            Console.WriteLine();
            foreach (var product in products)
                Console.WriteLine("{0,-2} {1}", product.ProductID, product.Name);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Count() == 4);
        }

        private void ToDataSet()
        {
            var results = SqlHelper.QueryMultipleSQL<Product, Person>(@"
                select top 4 * from Production.Product order by ProductID;
                select top 4 * from Person.Person order by BusinessEntityID;
            ").ToDataSet();

            var products = results.Tables[0];
            var people = results.Tables[1];

            Console.WriteLine("To DataSet");
            Console.WriteLine();
            foreach (DataRow product in products.Rows)
                Console.WriteLine("{0,-2} {1}", product["ProductID"], product["Name"]);
            Console.WriteLine();
            foreach (DataRow person in people.Rows)
                Console.WriteLine("{0,-2} {1} {2}", person["BusinessEntityID"], person["FirstName"], person["LastName"]);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsNotNull(people);
            Assert.IsTrue(products.Rows.Count == 4);
            Assert.IsTrue(people.Rows.Count == 4);
        }

        private void ExecuteScalarSQL()
        {
            int minProductID = SqlHelper.ExecuteScalarSQL<int>("select min(ProductID) from Production.Product");
            int maxProductID = SqlHelper.ExecuteScalarSQL<int>("select max(ProductID) from Production.Product");

            Console.WriteLine("Execute Scalar");
            Console.WriteLine();
            Console.WriteLine("Min ProductID: " + minProductID);
            Console.WriteLine("Max ProductID: " + maxProductID);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsTrue(minProductID > 0);
            Assert.IsTrue(maxProductID > 0);
        }

        private void ExecuteSQL()
        {
            var outParam = new DynamicParameters("MinProductID", sqlDbType: SqlDbType.Int, direction: ParameterDirection.Output);
            outParam.Add("MaxProductID", sqlDbType: SqlDbType.Int, direction: ParameterDirection.Output);

            int rowsAffected = SqlHelper.ExecuteSQL(@"
                select @MinProductID = min(ProductID) from Production.Product;
                select @MaxProductID = max(ProductID) from Production.Product;
            ", outParam: outParam);

            int minProductID = (int)outParam.Get()["MinProductID"];
            int maxProductID = outParam.Get<int>()["MaxProductID"];

            Console.WriteLine("Execute");
            Console.WriteLine();
            Console.WriteLine("Min ProductID: " + minProductID);
            Console.WriteLine("Max ProductID: " + maxProductID);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsTrue(minProductID > 0);
            Assert.IsTrue(maxProductID > 0);
        }

        private void ToProperties1()
        {
            string sql = @"if @Option = 1
begin
    select StringColumn = 'A', IntColumn = 1 union all
    select StringColumn = 'B', IntColumn = 2
end
else if @Option = 2
begin
    select 
        DateColumn = getdate(), 
        DecimalColumn = 1.1, 
        NullColumn = null
    union all
    select 
        DateColumn = getdate() + 1, 
        DecimalColumn = 2.2, 
        NullColumn = null
end";

            IEnumerable<dynamic> dynamicResults0 = SqlHelper.QuerySQL(sql, new { Option = 0 });
            IEnumerable<dynamic> dynamicResults1 = SqlHelper.QuerySQL(sql, new { Option = 1 });
            IEnumerable<dynamic> dynamicResults2 = SqlHelper.QuerySQL(sql, new { Option = 2 });

            IEnumerable<IDictionary<string, object>> results0 = dynamicResults0.ToProperties();
            IEnumerable<IDictionary<string, object>> results1 = dynamicResults1.ToProperties();
            IEnumerable<IDictionary<string, object>> results2 = dynamicResults2.ToProperties();

            Console.WriteLine("ToProperties from IEnumerable<dynamic>");
            Console.WriteLine();

            Console.WriteLine(sql);
            Console.WriteLine();

            Console.WriteLine("Option = 0:");
            PropertiesPrint(results0);

            Console.WriteLine("Option = 1:");
            PropertiesPrint(results1);

            Console.WriteLine("Option = 2:");
            PropertiesPrint(results2);

            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsTrue(results0.Count() == 0);
            Assert.IsTrue(results1.Count() == 2);
            Assert.IsTrue(results2.Count() == 2);
        }

        private void ToProperties2()
        {
            IEnumerable<IDictionary<string, object>> products = SqlHelper.QuerySQL<Product>("select top 4 * from Production.Product order by ProductID").ToProperties();

            Console.WriteLine("ToProperties from IEnumerable<Product>");
            Console.WriteLine();
            PropertiesPrint(products);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsTrue(products.Count() == 4);
        }

        private void ToProperties3()
        {
            IEnumerable<Product> products = SqlHelper.QuerySQL<Product>("select top 4 * from Production.Product order by ProductID");
            IEnumerable<IDictionary<string, object>> productsProps = products.ToProperties();
            IEnumerable<IDictionary<string, object>> productsPropsFiltered = productsProps.ToProperties("Name");

            Console.WriteLine("ToProperties from IEnumerable<Product> and filtered by column name");
            Console.WriteLine();
            PropertiesPrint(productsPropsFiltered);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsTrue(productsProps.Count() == 4);
            Assert.IsTrue(productsPropsFiltered.Count() == 4);
        }

        private void PropertiesPrint(IEnumerable<IDictionary<string, object>> results)
        {
            if (results.Count() == 0)
            {
                Console.WriteLine("Empty Result Set");
            }
            else
            {
                foreach (var key in results.First().Keys)
                    Console.Write(string.Format("{0,-22}", key));
                Console.WriteLine();

                foreach (var key in results.First().Keys)
                    Console.Write(string.Format("{0,-22}", "---------------"));
                Console.WriteLine();

                foreach (var row in results)
                {
                    foreach (var pair in row)
                        Console.Write(string.Format("{0,-22}", pair.Value));
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }

        private void ToPropertiesToDataTable1()
        {
            DataTable products = SqlHelper.QuerySQL("select top 4 ProductID, Name, ProductNumber from Production.Product order by ProductID").ToDataTable();

            Console.WriteLine("from IEnumerable<dynamic> to DataTable");
            Console.WriteLine();
            foreach (DataRow row in products.Rows)
                Console.WriteLine("{0,-2} {1,-22} {2}", row["ProductID"], row["Name"], row["ProductNumber"]);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Rows.Count == 4);
            Assert.IsTrue(products.Columns.Count == 3);
            Assert.IsTrue(products.Columns[0].ColumnName == "ProductID");
            Assert.IsTrue(products.Columns[1].ColumnName == "Name");
            Assert.IsTrue(products.Columns[2].ColumnName == "ProductNumber");
        }

        private void ToPropertiesToDataTable2()
        {
            IEnumerable<IDictionary<string, object>> products1 = SqlHelper.QuerySQL("select top 4 ProductID, Name from Production.Product order by ProductID").ToProperties();
            IEnumerable<IDictionary<string, object>> products2 = SqlHelper.QuerySQL("select top 4 ProductID, ProductNumber from Production.Product order by ProductID").ToProperties();

            List<IDictionary<string, object>> productsProps = new List<IDictionary<string, object>>();
            productsProps.AddRange(products1);
            productsProps.AddRange(products2);
            DataTable products = productsProps.ToDataTable();

            Console.WriteLine("two different properties mashed together");
            Console.WriteLine();
            foreach (DataRow row in products.Rows)
                Console.WriteLine("{0,-2} {1,-22} {2}", row["ProductID"], row["Name"], row["ProductNumber"]);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Rows.Count == 2 * 4);
            Assert.IsTrue(products.Columns.Count == 3);
            Assert.IsTrue(products.Columns[0].ColumnName == "ProductID");
            Assert.IsTrue(products.Columns[1].ColumnName == "Name");
            Assert.IsTrue(products.Columns[2].ColumnName == "ProductNumber");
        }

        private void ToPropertiesToDataTable3()
        {
            DataTable products = SqlHelper.QuerySQL("select top 4 ProductID, Name, ProductNumber from Production.Product order by ProductID").ToDataTable(toEmptyDataTable: true);

            Console.WriteLine("from IEnumerable<dynamic> to Empty DataTable");
            Console.WriteLine();
            Console.WriteLine("Number of rows: " + products.Rows.Count);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Rows.Count == 0);
            Assert.IsTrue(products.Columns.Count == 3);
            Assert.IsTrue(products.Columns[0].ColumnName == "ProductID");
            Assert.IsTrue(products.Columns[1].ColumnName == "Name");
            Assert.IsTrue(products.Columns[2].ColumnName == "ProductNumber");
        }
    }
}
