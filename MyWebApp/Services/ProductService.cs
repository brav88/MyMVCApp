using Microsoft.Data.SqlClient;
using MyWebApp.DatabaseHelper;
using MyWebApp.Models;
using System.Data;
using System.Drawing;

namespace MyWebApp.Services
{
    public static class ProductService
    {
        public static List<Product> GetAll()
        {
            var products = new List<Product>();

            DataTable? dataTable = DatabaseSql.ExecuteStoredProcedure("[dbo].[uspGetProducts]", null);

            if (dataTable == null)
                return products;

            foreach (DataRow row in dataTable.Rows)
            {
                products.Add(new Product
                {
                    ProductID = (int)row["ProductID"],
                    ProductNumber = row["ProductNumber"].ToString() ?? string.Empty,
                    ProductName = row["ProductName"].ToString() ?? string.Empty,
                    ListPrice = (decimal)row["ListPrice"],
                    Color = row["Color"].ToString() ?? string.Empty,
                    SubCategory = row["SubCategory"].ToString() ?? string.Empty,
                    Category = row["Category"].ToString() ?? string.Empty,
                    Model = row["Model"].ToString() ?? string.Empty,
                    Size = row["Size"].ToString() ?? string.Empty,
                    Weight = row["Weight"] == DBNull.Value ? null : (decimal)row["Weight"]
                });
            }

            return products;
        }

        public static ProductDetail? GetProductDetail(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new("@id", id)
            };

            DataTable? dataTable = DatabaseSql.ExecuteStoredProcedure(
                "[dbo].[uspGetProductDetail]", parameters);

            if (dataTable == null || dataTable.Rows.Count == 0)
                return null;

            DataRow row = dataTable.Rows[0];

            return new ProductDetail
            {
                ProductId = Convert.ToInt32(row["ProductID"]),
                ProductName = row["ProductName"].ToString() ?? string.Empty,
                ProductNumber = row["ProductNumber"].ToString() ?? string.Empty,
                MakeFlag = Convert.ToBoolean(row["MakeFlag"]),
                FinishedGoodsFlag = Convert.ToBoolean(row["FinishedGoodsFlag"]),
                Color = row["Color"] == DBNull.Value ? null : row["Color"].ToString(),
                SafetyStockLevel = Convert.ToInt16(row["SafetyStockLevel"]),
                ReorderPoint = Convert.ToInt16(row["ReorderPoint"]),
                CurrentStandardCost = Convert.ToDecimal(row["CurrentStandardCost"]),
                CurrentListPrice = Convert.ToDecimal(row["CurrentListPrice"]),
                Size = row["Size"] == DBNull.Value ? null : row["Size"].ToString(),
                SizeUnitMeasureCode = row["SizeUnitMeasureCode"] == DBNull.Value ? null : row["SizeUnitMeasureCode"].ToString(),
                WeightUnitMeasureCode = row["WeightUnitMeasureCode"] == DBNull.Value ? null : row["WeightUnitMeasureCode"].ToString(),
                Weight = row["Weight"] == DBNull.Value ? null : Convert.ToDecimal(row["Weight"]),
                DaysToManufacture = Convert.ToInt32(row["DaysToManufacture"]),
                ProductLine = row["ProductLine"] == DBNull.Value ? null : row["ProductLine"].ToString(),
                Class = row["Class"] == DBNull.Value ? null : row["Class"].ToString(),
                Style = row["Style"] == DBNull.Value ? null : row["Style"].ToString(),
                SellStartDate = Convert.ToDateTime(row["SellStartDate"]),
                SellEndDate = row["SellEndDate"] == DBNull.Value ? null : Convert.ToDateTime(row["SellEndDate"]),
                DiscontinuedDate = row["DiscontinuedDate"] == DBNull.Value ? null : Convert.ToDateTime(row["DiscontinuedDate"]),
                TotalQuantityInStock = Convert.ToInt32(row["TotalQuantityInStock"])
            };
        }

        public static void SaveProductDetail(ProductDetail detail)
        {
            var parameters = new List<SqlParameter>
            {
                new("@ProductId", (object?)detail.ProductId ?? DBNull.Value),
                new("@ProductName", detail.ProductName ?? (object)DBNull.Value),
                new("@ProductNumber", detail.ProductNumber ?? (object)DBNull.Value),
                new("@MakeFlag", detail.MakeFlag),
                new("@FinishedGoodsFlag", detail.FinishedGoodsFlag),
                new("@Color", detail.Color ?? (object)DBNull.Value),
                new("@SafetyStockLevel", detail.SafetyStockLevel),
                new("@ReorderPoint", detail.ReorderPoint),
                new("@CurrentStandardCost", detail.CurrentStandardCost),
                new("@CurrentListPrice", detail.CurrentListPrice),
                new("@Size", detail.Size ?? (object)DBNull.Value),
                new("@SizeUnitMeasureCode", detail.SizeUnitMeasureCode ?? (object)DBNull.Value),
                new("@WeightUnitMeasureCode", detail.WeightUnitMeasureCode ?? (object)DBNull.Value),
                new("@Weight", detail.Weight ?? (object)DBNull.Value),
                new("@DaysToManufacture", detail.DaysToManufacture),
                new("@ProductLine", detail.ProductLine ?? (object)DBNull.Value),
                new("@Class", detail.Class ?? (object)DBNull.Value),
                new("@Style", detail.Style ?? (object)DBNull.Value),
                new("@SellStartDate", detail.SellStartDate),
                new("@SellEndDate", detail.SellEndDate ?? (object)DBNull.Value),
                new("@DiscontinuedDate", detail.DiscontinuedDate ?? (object)DBNull.Value),
                new("@TotalQuantityInStock", detail.TotalQuantityInStock)
            };

            DatabaseSql.ExecuteStoredProcedure("[dbo].[uspUpdateProduct]", parameters);
        }
    }
}
