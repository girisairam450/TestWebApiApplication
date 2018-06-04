using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Configuration;

namespace Api
{
    public class ConnectionStrings
    {
        #region Fields

        private Configuration _configuration;
        private ConnectionStringsSection _connectionStringSection;
        private bool _changed;

        private static readonly List<string> _microsoftSqlServerDriverStrings =
            new List<string> { "SQL Native Client", "SQL Server Native Client", "SQL Server" };
        private static readonly List<string> _mySqlDriverStrings =
            new List<string> { "MySQL" };
        private static readonly List<string> _oracleDriverStrings =
            new List<string> { "Oracle" };

        #endregion Fields

        #region Constructors

        public ConnectionStrings()
        {
            _configuration = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            _connectionStringSection = (ConnectionStringsSection)_configuration.GetSection("connectionStrings");
            _changed = false;
        }

        public ConnectionStrings(string webConfigPath)
        {
            _configuration = WebConfigurationManager.OpenWebConfiguration(webConfigPath);
            _connectionStringSection = (ConnectionStringsSection)_configuration.GetSection("connectionStrings");
            _changed = false;
        }

        #endregion Constructors

        #region Public Methods

        public static void LoadConnectionStrings()
        {
            ConnectionStrings connectionStrings = new ConnectionStrings("/");
            List<string> allStrings = connectionStrings.GetAllConnectionStringNames();

            if (!allStrings.Contains("Admin"))
            {
                KeyValuePair<string, string> adminPair = new KeyValuePair<string, string>("Admin", "MySQL ODBC 5.3 Unicode Driver;Server=localhost;UID=psftmaster;PWD=17utn18atn;database=profsoft_admin;");
                connectionStrings.SetConnectionStrings(new List<KeyValuePair<string, string>> { adminPair });
            }
        }

        /// <summary>
        /// Adds a connection string to the web configuration file
        /// </summary>
        /// <param name="connectionName">Name of the connection to add</param>
        /// <param name="connectionString">The connection string to add</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool AddConnectionString(string connectionName, string connectionString)
        {
            ConnectionStringSettings newConnectionString = new ConnectionStringSettings(connectionName, connectionString);

            if (UnprotectSection())
            {
                _connectionStringSection.ConnectionStrings.Add(newConnectionString);
                _changed = true;
                if (ProtectSection())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Deletes a connection string from the web configuration file
        /// </summary>
        /// <param name="connectionName">Name of the connection to delete</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool DeleteConnectionString(string connectionName)
        {
            if (UnprotectSection())
            {
                _connectionStringSection.ConnectionStrings.Remove(connectionName);
                _changed = true;
                if (ProtectSection())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the value for the specified connection string in the web configuration file
        /// </summary>
        /// <param name="connectionName">Name of the connection to set</param>
        /// <param name="connectionString">The connection string to set</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool SetConnectionString(string connectionName, string connectionString)
        {
            if (UnprotectSection())
            {
                _connectionStringSection.ConnectionStrings[connectionName].ConnectionString = connectionString;
                _changed = true;
                if (ProtectSection())
                    return true;
            }
            return false;
        }

        public bool SetConnectionStrings(List<KeyValuePair<string, string>> nameValuePairs)
        {
            if (UnprotectSection())
            {
                foreach (KeyValuePair<string, string> kvp in nameValuePairs)
                {
                    if (_connectionStringSection.ConnectionStrings[kvp.Key] != null)
                        _connectionStringSection.ConnectionStrings[kvp.Key].ConnectionString = kvp.Value;
                    else
                        _connectionStringSection.ConnectionStrings.Add(new ConnectionStringSettings(kvp.Key, kvp.Value));
                    _changed = true;
                }
                if (ProtectSection())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a specified connection string from the web configuration file
        /// </summary>
        /// <param name="connectionName">Name of the connection to get</param>
        /// <returns>The requested connection string or an empty string on failure</returns>
        public string GetConnectionString(string connectionName)
        {
            if (UnprotectSection())
            {
                string connectionString = _connectionStringSection.ConnectionStrings[connectionName].ConnectionString;
                if (ProtectSection())
                    return connectionString;
            }
            return "";
        }

        /// <summary>
        /// Returns the entire collection of connection string from the web configuration file
        /// </summary>
        /// <returns>A collection of connection strings</returns>
        public ConnectionStringSettingsCollection GetAllConnectionStrings()
        {
            if (UnprotectSection())
            {
                ConnectionStringSettingsCollection connectionStringSettingsCollection = _connectionStringSection.ConnectionStrings;
                if (ProtectSection())
                    return connectionStringSettingsCollection;
            }
            return null;
        }

        /// <summary>
        /// Returns a list of strings containing the names of all the connections
        /// </summary>
        /// <returns>a list of strings</returns>
        public List<string> GetAllConnectionStringNames()
        {
            List<string> connectionStringNames = new List<string>();
            foreach (ConnectionStringSettings css in GetAllConnectionStrings())
            {
                connectionStringNames.Add(css.Name);
            }
            return connectionStringNames;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Encrypts the the connectionStrings config section
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        protected bool ProtectSection()
        {
            try
            {
                if ((_connectionStringSection != null) && !_connectionStringSection.SectionInformation.IsProtected)
                {
                    _connectionStringSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    if (_changed)
                    {
                        _configuration.Save();
                        _changed = false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Decrypts the connectionString config section
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        protected bool UnprotectSection()
        {
            try
            {
                if (_connectionStringSection != null)
                    if (_connectionStringSection.SectionInformation.IsProtected)
                        _connectionStringSection.SectionInformation.UnprotectSection();
                    else
                        return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion Protected Methods

        #region Static Methods

        public static bool isMSSQL(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
                return false;

            foreach (string driverString in _microsoftSqlServerDriverStrings)
            {
                if (connectionString.ToUpper().Contains(driverString.ToUpper()))
                    return true;
            }
            return false;
        }

        public static bool isMySQL(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
                return false;

            foreach (string driverString in _mySqlDriverStrings)
            {
                if (connectionString.ToUpper().Contains(driverString.ToUpper()))
                    return true;
            }
            return false;
        }

        public static bool isOracle(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
                return false;

            foreach (string driverString in _oracleDriverStrings)
            {
                if (connectionString.ToUpper().Contains(driverString.ToUpper()))
                    return true;
            }
            return false;
        }

        #endregion Static Methods
    }
}
