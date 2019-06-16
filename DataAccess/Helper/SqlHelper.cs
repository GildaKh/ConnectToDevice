using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Utility
{
    public class SqlHelper : IDisposable
    {
        public static object GetsafeValue(object obj)
        {
            return obj ?? DBNull.Value;
        }
        private readonly string _connectionString;
        private SqlConnection _sqlConnection;

        public SqlHelper(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public DataRow FetchDataRow(string query, List<SqlParameter> parameters, CommandType commandType)
        {
            DataTable dt = FetchDataTable(query, parameters, commandType);
            if (dt == null) return null;
            if (dt.Rows.Count == 0) return null;
            return dt.Rows[0];
        }

        public DataTable FetchDataTable(string query, List<SqlParameter> parameters, CommandType commandType)
        {
            DataSet ds = FetchDataSet(query, parameters, commandType);
            if (ds == null) return null;
            if (ds.Tables.Count == 0) return null;
            return ds.Tables[0];
        }

        public DataSet FetchDataSet(string query, List<SqlParameter> parameters, CommandType commandType)
        {
            SqlDataAdapter da = GetSqlDataAdapter(query, parameters, commandType);
            DataSet ds = new DataSet();

            da.Fill(ds);
            return ds;
        }

        public DataRow FetchDataRowByStoredProcedure(string spName, List<SqlParameter> parameters)
        {
            return FetchDataRow(spName, parameters, CommandType.StoredProcedure);
        }

        public DataTable FetchDataTableByStoredProcedure(string spName, List<SqlParameter> parameters)
        {
            return FetchDataTable(spName, parameters, CommandType.StoredProcedure);
        }

        public DataSet FetchDataSetByStoredProcedure(string spName, List<SqlParameter> parameters)
        {
            return FetchDataSet(spName, parameters, CommandType.StoredProcedure);
        }

        public List<SqlParameter> ExecuteNonQuery(string query, List<SqlParameter> parameters, CommandType commandType)
        {
            SqlCommand cmd = null;
            List<SqlParameter> outParams = new List<SqlParameter>();
            try
            {
                cmd = GetSqlCommand(query, parameters, commandType);

                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.ExecuteNonQuery();

                cmd.Connection.Close();
            }
            finally
            {
                if (cmd != null)
                    cmd.Connection.Close();
            }
            return parameters;
        }

        public List<SqlParameter> ExecuteNonQueryStoredProcedure(string procedureName, List<SqlParameter> parameters)
        {
            return ExecuteNonQuery(procedureName, parameters, CommandType.StoredProcedure);
        }

        private SqlDataAdapter GetSqlDataAdapter(string commandText, List<SqlParameter> parameters, CommandType commandType)
        {
            SqlDataAdapter da = new SqlDataAdapter() { SelectCommand = GetSqlCommand(commandText, parameters, commandType) };
            return da;
        }

        private SqlCommand GetSqlCommand(string commandText, List<SqlParameter> parameters, CommandType commandType)
        {
            var cmd = new SqlCommand(commandText) { CommandType = commandType };
            cmd.Connection = GetSqlConnection();
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }
            return cmd;
        }

        private SqlConnection GetSqlConnection()
        {
            _sqlConnection = new SqlConnection(_connectionString);
            return _sqlConnection;
        }

        public void Dispose()
        {
            if (_sqlConnection != null && _sqlConnection.State == ConnectionState.Open)
            {
                _sqlConnection.Close();
                _sqlConnection.Dispose();
            }
        }
    }
}