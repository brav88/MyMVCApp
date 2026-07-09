using Microsoft.Data.SqlClient;
using System.Data;

namespace MyWebApp.DatabaseHelper
{
    public static class DatabaseSql
    {
        private static readonly string ConnectionString =
            "Data Source=SAMUEL\\SQLEXPRESS;Database=AdventureWorks2025;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"MyApp\";Command Timeout=0";

        public static DataTable ExecuteStoredProcedure(string storedProcedure, List<SqlParameter>? parameters = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters is not null)
                command.Parameters.AddRange(parameters.ToArray());

            connection.Open();

            using var adapter = new SqlDataAdapter(command);

            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }
    }
}
