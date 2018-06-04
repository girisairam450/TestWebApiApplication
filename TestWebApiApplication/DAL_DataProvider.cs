using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class DAL_DataProvider : IDisposable
    {

        //private static readonly ILog log = LogManager.GetLogger(typeof(SageDataProvider));
        public static string AdminConnName = "Admin";
        public static string AdminSchema = ConfigurationManager.AppSettings["AdminSchema"];
        public static string HistoryConnName = "History";
        public static string HistorySchema = ConfigurationManager.AppSettings["HistorySchema"];
        public static string SageDbProvider = ConfigurationManager.AppSettings["DBMS"];
        public static string MasterConnName = "Master";

        private DbProviderFactory _provider;
        private DbConnection _connection;

        public DAL_DataProvider(string connectionName)
        {
            _provider = DbProviderFactories.GetFactory(SageDbProvider);

            ConnectionName = connectionName;

            if (!String.IsNullOrWhiteSpace(connectionName))
                ConnectionString = GetConnectionString(connectionName);

            if (!String.IsNullOrWhiteSpace(ConnectionString))
                _connection = GetConnection();
        }

        ~DAL_DataProvider()
        {
            Dispose(false);
        }

        #region Methods


        #region Common

        public static string GetConnectionString(string connectionStringName)
        {
            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
                return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            else
                return "";
        }

        private DbConnection GetConnection()
        {
            if (String.IsNullOrWhiteSpace(ConnectionName) && String.IsNullOrWhiteSpace(ConnectionString))
                throw new InvalidOperationException("GetConnection: connection string not defined");
            else if (String.IsNullOrWhiteSpace(ConnectionString))
                ConnectionString = GetConnectionString(ConnectionName);

            if (String.IsNullOrWhiteSpace(ConnectionString))
                throw new InvalidOperationException("GetConnection: connection name is invalid");

            if (_connection == null)
            {
                _connection = _provider.CreateConnection();
                _connection.ConnectionString = ConnectionString;
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.ConnectionString = ConnectionString;
                _connection.Open();
            }

            return _connection;
        }

        private void CloseConnection()
        {
            if ((_connection != null) && (_connection.State != ConnectionState.Closed))
                _connection.Close();
        }

        private DbCommand GetCommand()
        {
            if ((_connection == null) || (_connection.State != ConnectionState.Open))
                _connection = GetConnection();

            return _connection.CreateCommand();
        }

        private void PrepareCommand(DbCommand command, string sql, DbParameter[] parameters, bool isStoreProc)
        {
            // Clear the parameters since the command could have been used before
            command.Parameters.Clear();

            // Add the list of parameters to the DbCommand instance
            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            // Generate the final SQL and assign it to the DbCommand instance
            command.CommandText = sql;
            if (isStoreProc)
                command.CommandType = CommandType.StoredProcedure;

            // Open the connection
            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();
        }

        private void CleanCommand(DbCommand command)
        {
            // Clear the parameters since this command may be re-used
            if (command.Parameters.Count > 0)
                command.Parameters.Clear();
        }

        #endregion Common

        #region DataSet

        public DataSet FillDataSet(string sql)
        {
            return FillDataSet(sql, new DbParameter[] { });
        }

        public DataSet FillDataSet(string sql, string tableName, bool isStoreProc = false)
        {
            return FillDataSet(sql, new DbParameter[] { }, tableName, isStoreProc);
        }

        public DataSet FillDataSet(string sql, DbParameter[] dbParameters, bool isStoreProc = false)
        {
            return FillDataSet(sql, dbParameters, "", isStoreProc);
        }

        public DataSet FillDataSet(string sql, DbParameter[] dbParameters, string tableName, bool isStoreProc = false)
        {
            DataSet dataSet = new DataSet();

            using (DbCommand command = GetCommand())
            {
                using (DbDataAdapter adapter = _provider.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    Fill(sql, dataSet, dbParameters, tableName, adapter, isStoreProc);
                }
            }

            return dataSet;
        }

        public void Fill(string sql, DataSet dataSet)
        {
            Fill(sql, dataSet, new DbParameter[] { });
        }

        public void Fill(string sql, DataSet dataSet, string tableName)
        {
            Fill(sql, dataSet, tableName, new DbParameter[] { });
        }

        public void Fill(string sql, DataSet dataSet, DbParameter[] parameters)
        {
            Fill(sql, dataSet, "", parameters);
        }

        public void Fill(string sql, DataSet dataSet, string tableName, DbParameter[] dbParameters)
        {
            using (DbCommand command = GetCommand())
            {
                using (DbDataAdapter adapter = _provider.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    Fill(sql, dataSet, dbParameters, tableName, adapter);
                }
            }
        }

        private void Fill(string sql, DataSet dataSet, DbParameter[] dbParameters, string tableName, DbDataAdapter dataAdapter, bool isStoreProc = false)
        {
            PrepareCommand(dataAdapter.SelectCommand, sql, dbParameters, isStoreProc);

            // Populate the DataSet
            if (!String.IsNullOrWhiteSpace(tableName))
                dataAdapter.Fill(dataSet, tableName);
            else
                dataAdapter.Fill(dataSet);

            CleanCommand(dataAdapter.SelectCommand);
        }

        #endregion DataSet

        #region Table

        public void Fill(string sql, DataTable dataTable)
        {
            Fill(sql, dataTable, new DbParameter[] { });
        }

        public void Fill(string sql, DataTable dataTable, string tableName)
        {
            Fill(sql, dataTable, new DbParameter[] { }, tableName);
        }

        public void Fill(string sql, DataTable dataTable, DbParameter[] parameters)
        {
            Fill(sql, dataTable, parameters, "");
        }

        public void Fill(string sql, DataTable dataTable, DbParameter[] dbParameters, string tableName)
        {
            using (DbCommand command = GetCommand())
            {
                using (DbDataAdapter adapter = _provider.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    Fill(sql, dataTable, dbParameters, tableName, adapter);
                }
            }
        }

        private void Fill(string sql, DataTable dataTable, DbParameter[] dbParameters, string tableName, DbDataAdapter dataAdapter, bool isStoreProc = false)
        {
            dataTable.TableName = !String.IsNullOrWhiteSpace(tableName) ? tableName : "";

            PrepareCommand(dataAdapter.SelectCommand, sql, dbParameters, isStoreProc);

            // Populate the DataTable
            dataAdapter.Fill(dataTable);

            CleanCommand(dataAdapter.SelectCommand);
        }

        public DataTable FillDataTable(string sql)
        {
            return FillDataTable(sql, new DbParameter[] { });
        }

        public DataTable FillDataTable(string sql, string tableName)
        {
            return FillDataTable(sql, new DbParameter[] { }, tableName);
        }

        public DataTable FillDataTable(string sql, DbParameter[] dbParameters, bool isStoreProc = false)
        {
            return FillDataTable(sql, dbParameters, "", isStoreProc);
        }

        public DataTable FillDataTable(string sql, DbParameter[] dbParameters, string tableName, bool isStoreProc = false)
        {
            DataTable dataTable = new DataTable();

            using (DbCommand command = GetCommand())
            {
                using (DbDataAdapter adapter = _provider.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    Fill(sql, dataTable, dbParameters, tableName, adapter, isStoreProc);
                }
            }

            return dataTable;
        }

        #endregion Table

        #region Scalar

        public object ExecuteScalar(string sql, bool isStoreProc = false)
        {
            return ExecuteScalar(sql, new DbParameter[] { }, isStoreProc);
        }

        public object ExecuteScalar(string sql, DbParameter[] parameters, bool isStoreProc = false)
        {
            using (DbCommand command = GetCommand())
            {
                return ExecuteScalar(sql, parameters, command, isStoreProc);
            }
        }

        public object ExecuteScalar(string sql, DbParameter[] parameters, DbCommand command, bool isStoreProc = false)
        {
            object returnVal = -1;

            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            command.CommandText = sql;

            if (isStoreProc)
                command.CommandType = CommandType.StoredProcedure;

            returnVal = command.ExecuteScalar();

            if (command.Parameters.Count > 0)
                command.Parameters.Clear();

            return returnVal;
        }

        #endregion Scalar

        #region Non-Query

        public int ExecuteNonQuery(string sql, bool isStoreProc = false)
        {
            return ExecuteNonQuery(sql, new DbParameter[] { }, isStoreProc);
        }

        public int ExecuteNonQuery(string sql, DbParameter parameter, bool isStoreProc = false)
        {
            return ExecuteNonQuery(sql, new DbParameter[] { parameter }, isStoreProc);
        }

        public int ExecuteNonQuery(string sql, DbParameter[] parameters, bool isStoreProc = false)
        {
            using (DbCommand command = GetCommand())
            {
                return ExecuteNonQuery(sql, parameters, command, isStoreProc);
            }
        }

        private int ExecuteNonQuery(string sql, DbParameter[] parameters, DbCommand command, bool isStoreProc = false)
        {
            int returnVal = -1;

            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            command.CommandText = sql;

            if (isStoreProc)
                command.CommandType = CommandType.StoredProcedure;

            returnVal = command.ExecuteNonQuery();

            if (command.Parameters.Count > 0)
                command.Parameters.Clear();

            return returnVal;
        }

        public string ExecuteNonQueryWithTransaction(List<string> querylist)
        {
            string strStatus = string.Empty;
            using (DbConnection connection = GetConnection())
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    DbTransaction transaction = null;
                    try
                    {
                        transaction = connection.BeginTransaction();

                        command.Transaction = transaction;

                        foreach (string query in querylist)
                        {
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        //log.Error("Exception at ExecuteNonQueryWithTransaction() " + ex);
                        strStatus = ex.ToString();
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return strStatus;
        }

        #endregion Non-Query

        #region Parameters

        public DbParameter CreateParameter(string parameterName, object parameterValue)
        {
            DbParameter parameter = _provider.CreateParameter();

            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;

            return parameter;
        }

        public DbParameter CreateParameter(string parameterName, object parameterValue, DbType parameterType, ParameterDirection parameterDirection)
        {
            DbParameter parameter = _provider.CreateParameter();

            parameter.ParameterName = parameterName;
            parameter.Value = (parameterValue == null) ? DBNull.Value : parameterValue;
            parameter.DbType = parameterType;
            parameter.Direction = parameterDirection;

            return parameter;
        }

        public DbParameter CreateParameter(string parameterName, object parameterValue, DbType parameterType, ParameterDirection parameterDirection, int parameterSize)
        {
            DbParameter parameter = _provider.CreateParameter();

            parameter.ParameterName = parameterName;
            parameter.Value = (parameterValue == null) ? DBNull.Value : parameterValue;
            parameter.DbType = parameterType;
            parameter.Direction = parameterDirection;
            parameter.Size = parameterSize;

            return parameter;
        }

        #endregion Parameters

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_connection != null)
                {
                    CloseConnection();
                    _connection.Dispose();
                    _connection = null;
                }
                if (_provider != null)
                    _provider = null;
            }
            // free native resources here if there are any.
        }

        #endregion IDisposable

        #endregion Methods

        #region Properties

        public string ConnectionString { get; set; }

        public string ConnectionName { get; set; }

        #endregion Properties
    }
}
