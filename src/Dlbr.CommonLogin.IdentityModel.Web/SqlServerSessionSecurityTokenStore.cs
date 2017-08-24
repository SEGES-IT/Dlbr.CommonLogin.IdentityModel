using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens;
using Dlbr.CommonLogin.IdentityModel.Web.Logging;

namespace Dlbr.CommonLogin.IdentityModel.Web
{
    public class SqlServerSessionSecurityTokenStore : SessionSecurityTokenStore
    {
        private static readonly ILog Log = LogProvider.For<SqlServerSessionSecurityTokenStore>();

        public SqlServerSessionSecurityTokenStore()
        {
            ConnectionStringName = "SecurityTokenCacheContext";
            CookieTableName = "SecurityTokenCacheEntries";
        }

        public string ConnectionStringName { get; set; }

        public string CookieTableName { get; set; }

        protected virtual ConnectionStringSettings ReadConnectionStringSettings()
        {
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (connectionString == null)
            {
                throw new InvalidOperationException(string.Format("ConnectionString {0} is not defined in config",
                    ConnectionStringName));
            }
            return connectionString;
        }

        private DbConnection CreateOpenConnection()
        {
            ConnectionStringSettings settings = ReadConnectionStringSettings();
            DbProviderFactory factory = DbProviderFactories.GetFactory(settings.ProviderName);
            DbConnection sqlConnection = factory.CreateConnection();
            sqlConnection.ConnectionString = settings.ConnectionString;
            sqlConnection.Open();
            return sqlConnection;
        }

        public override Tuple<DateTime, SessionSecurityToken> ReadTokenFromStore(SessionSecurityTokenCacheKey key)
        {
            using (DbConnection connection = CreateOpenConnection())
            {
                using (DbCommand selectCommand = CreateSelectCommand(connection, key))
                {
                    Log.DebugFormat("Executing {0}", selectCommand.CommandText);
                    var securityTokenSerialized = (string) selectCommand.ExecuteScalar();
                    if (securityTokenSerialized == null)
                    {
                        return null;
                    }
                    Tuple<DateTime, SessionSecurityToken> tokenWithExpiry = Deserialize(securityTokenSerialized);
                    return tokenWithExpiry;
                }
            }
        }

        public override void RemoveTokenFromStore(SessionSecurityTokenCacheKey cacheKey)
        {
            using (DbConnection connection = CreateOpenConnection())
            {
                using (DbCommand deleteCommand = CreateDeleteCommand(connection, cacheKey))
                {
                    Log.DebugFormat("Executing {0}", deleteCommand.CommandText);
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }

        public override void UpdateTokenInStore(SessionSecurityTokenCacheKey key, SessionSecurityToken value,DateTime expiryTime)
        {
            using (DbConnection sqlConnection = CreateOpenConnection())
            {
                using (DbTransaction transaction = sqlConnection.BeginTransaction())
                {
                    using (DbCommand deleteCommand = CreateDeleteCommand(sqlConnection, key))
                    {
                        deleteCommand.Transaction = transaction;
                        using (DbCommand insertCommand = CreateInsertCommand(sqlConnection, key, value, expiryTime))
                        {
                            insertCommand.Transaction = transaction;
                            Log.DebugFormat("Executing {0}", deleteCommand.CommandText);
                            deleteCommand.ExecuteNonQuery();
                            Log.DebugFormat("Executing {0}", insertCommand.CommandText);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        private DbCommand CreateSelectCommand(DbConnection connection, SessionSecurityTokenCacheKey key)
        {
            DbCommand command = connection.CreateCommand();
            DbParameter keyParameter = command.CreateParameter();
            keyParameter.DbType = DbType.StringFixedLength;
            keyParameter.ParameterName = "@pKey";
            keyParameter.Value = GenerateCompositeCacheKey(key);
            command.CommandText = string.Format("SELECT [SecurityTokenSerialized] FROM {0} WHERE [Id] = @pKey",
                CookieTableName);
            command.Parameters.AddRange(new[] {keyParameter});
            Log.DebugFormat("Created command: {0}", command.CommandText);
            Log.DebugFormat("Parameters: Id:'{0}'", keyParameter.Value);
            return command;
        }

        private DbCommand CreateDeleteCommand(DbConnection connection, SessionSecurityTokenCacheKey key)
        {
            DbCommand command = connection.CreateCommand();
            DbParameter keyParameter = command.CreateParameter();
            keyParameter.DbType = DbType.StringFixedLength;
            keyParameter.ParameterName = "@pKey";
            keyParameter.Value = GenerateCompositeCacheKey(key);
            command.CommandText = string.Format("DELETE FROM {0} WHERE [Id] = @pKey", CookieTableName);
            command.Parameters.AddRange(new[] {keyParameter});
            Log.DebugFormat("Created command: {0}", command.CommandText);
            Log.DebugFormat("Parameters: Id:'{0}'", keyParameter.Value);
            return command;
        }

        private DbCommand CreateInsertCommand(DbConnection connection, SessionSecurityTokenCacheKey key,
            SessionSecurityToken value, DateTime expiryTime)
        {
            DbCommand command = connection.CreateCommand();

            DbParameter keyParameter = command.CreateParameter();
            keyParameter.DbType = DbType.StringFixedLength;
            keyParameter.ParameterName = "@pKey";
            keyParameter.Value = GenerateCompositeCacheKey(key);

            DbParameter valueParameter = command.CreateParameter();
            valueParameter.DbType = DbType.StringFixedLength;
            valueParameter.ParameterName = "@pValue";
            valueParameter.Value = Serialize(expiryTime, value);

            DbParameter addedParameter = command.CreateParameter();
            addedParameter.DbType = DbType.DateTime;
            addedParameter.ParameterName = "@pAdded";
            addedParameter.Value = DateTime.Now;

            command.CommandText = string.Format(
                "INSERT INTO {0} ([Id], [SecurityTokenSerialized], [TimeStamp]) VALUES (@pKey, @pValue, @pAdded)",
                CookieTableName);

            command.Parameters.AddRange(new[] {keyParameter, valueParameter, addedParameter});
            Log.DebugFormat("Created command: {0}", command.CommandText);
            Log.DebugFormat("Parameters: Id:'{0}' Timestamp:'{1}' SecurityTokenSerialized:'{2}'", keyParameter.Value,
                valueParameter.Value, addedParameter.Value);

            return command;
        }
    }
}