using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

namespace Dlbr.CommonLogin.Owin
{
    public class DbAuthenticationSessionStore : IAuthenticationSessionStore
    {
        public DbAuthenticationSessionStore(ISecureDataFormat<AuthenticationTicket> ticketDataFormat, ConnectionStringSettings connectionStringSettings, ILogger logger = null)
        {
            this.TicketDataFormat = ticketDataFormat;
            this.ConnectionString = connectionStringSettings.ConnectionString;
            this.DbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
            this.Logger = logger;
        }

        private const string CookieTableName = "SecurityTokenCacheEntries";
        private static readonly string InsertCommandText = string.Format("INSERT INTO {0} ([Id], [SecurityTokenSerialized], [TimeStamp]) VALUES (@pKey, @pValue, @pAdded)", CookieTableName);
        private static readonly string SelectCommandText = string.Format("SELECT [SecurityTokenSerialized] FROM {0} WHERE [Id] = @pKey", CookieTableName);
        private static readonly string UpdateCommandText = string.Format("UPDATE {0} SET [SecurityTokenSerialized] = @pValue, [TimeStamp] = @pAdded WHERE [Id] = @pKey", CookieTableName);
        private static readonly string DeleteCommandText = string.Format("DELETE FROM {0} WHERE [Id] = @pKey", CookieTableName);

        private ISecureDataFormat<AuthenticationTicket> TicketDataFormat { get; set; }
        private ILogger Logger { get; set; }
        private string ConnectionString { get; set; }
        private DbProviderFactory DbProviderFactory { get; set; }

        //public async Task<string> StoreAsync(AuthenticationTicket ticket)
        //{
        //    var key = Guid.NewGuid().ToString();
        //    var value = this.TicketDataFormat.Protect(ticket);
        //    await this.DbProviderFactory.Execute(
        //        this.ConnectionString,
        //        connection => this.CreateInsertUpdateCommand(connection, InsertCommandText, key, value),
        //        command => command.ExecuteNonQueryAsync());
        //    return key;
        //}

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString();
            var value = this.TicketDataFormat.Protect(ticket);
            this.DbProviderFactory.Execute(
                this.ConnectionString,
                connection => this.CreateInsertUpdateCommand(connection, InsertCommandText, key, value),
                command => command.ExecuteNonQuery());
            return Task.FromResult(key);
        }

        //public async Task<AuthenticationTicket> RetrieveAsync(string key)
        //{
        //    var value = (string)await this.DbProviderFactory.Execute(
        //        this.ConnectionString,
        //        connection => this.CreateSelectDeleteCommand(connection, SelectCommandText, key),
        //        command => command.ExecuteScalarAsync());
        //    return value != null ? this.TicketDataFormat.Unprotect(value) : null;
        //}

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var value = (string)this.DbProviderFactory.Execute(
                this.ConnectionString,
                connection => this.CreateSelectDeleteCommand(connection, SelectCommandText, key),
                command => command.ExecuteScalar());
            return Task.FromResult(value != null ? this.TicketDataFormat.Unprotect(value) : null);
        }

        //public async Task RenewAsync(string key, AuthenticationTicket ticket)
        //{
        //    var value = this.TicketDataFormat.Protect(ticket);
        //    await this.DbProviderFactory.Execute(
        //        this.ConnectionString,
        //        connection => this.CreateInsertUpdateCommand(connection, UpdateCommandText, key, value),
        //        command => command.ExecuteNonQueryAsync());
        //}

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var value = this.TicketDataFormat.Protect(ticket);
            this.DbProviderFactory.Execute(
                this.ConnectionString,
                connection => this.CreateInsertUpdateCommand(connection, UpdateCommandText, key, value),
                command => command.ExecuteNonQuery());
            return Task.FromResult<object>(null);
        }

        //public async Task RemoveAsync(string key)
        //{
        //    await this.DbProviderFactory.Execute(
        //        this.ConnectionString,
        //        connection => this.CreateSelectDeleteCommand(connection, DeleteCommandText, key),
        //        command => command.ExecuteNonQueryAsync());
        //}

        public Task RemoveAsync(string key)
        {
            this.DbProviderFactory.Execute(
                this.ConnectionString,
                connection => this.CreateSelectDeleteCommand(connection, DeleteCommandText, key),
                command => command.ExecuteNonQuery());
            return Task.FromResult<object>(null);
        }

        private DbCommand CreateInsertUpdateCommand(DbConnection connection, string commandText, string key, string value)
        {
            var now = DateTime.Now;
            var command = connection.CreateCommand(commandText)
                .AddParameter("@pKey", DbType.String, key)
                .AddParameter("@pValue", DbType.String, value)
                .AddParameter("@pAdded", DbType.DateTime, now);

            if (this.Logger != null)
            {
                this.Logger.WriteInformation(String.Format("Created command: {0}", command.CommandText));
                this.Logger.WriteInformation(String.Format("Parameters: Key:'{0}' Timestamp:'{1}' SecurityTokenSerialized:'{2}'", key, now, value));
            }

            return command;
        }

        private DbCommand CreateSelectDeleteCommand(DbConnection connection, string commandText, string key)
        {
            var command = connection.CreateCommand(commandText).AddParameter("@pKey", DbType.String, key);

            if (this.Logger != null)
            {
                this.Logger.WriteInformation(String.Format("Created command: {0}", command.CommandText));
                this.Logger.WriteInformation(String.Format("Parameters: Key:'{0}'", key));
            }

            return command;
        }
    }

    internal static class DbProviderFactoryExtensions
    {
        //public static async Task<T> Execute<T>(this DbProviderFactory dbProviderFactory, string connectionString, Func<DbConnection, DbCommand> commandFactory, Func<DbCommand, Task<T>> action)
        //{
        //    using (var connection = dbProviderFactory.CreateConnection())
        //    {
        //        connection.ConnectionString = connectionString;
        //        using (var command = commandFactory(connection))
        //        {
        //            try
        //            {
        //                await connection.OpenAsync();
        //                return await action(command);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //    }
        //}

        public static T Execute<T>(this DbProviderFactory dbProviderFactory, string connectionString, Func<DbConnection, DbCommand> commandFactory, Func<DbCommand, T> action)
        {
            using (var connection = dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                using (var command = commandFactory(connection))
                {
                    try
                    {
                        connection.Open();
                        return action(command);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }

    internal static class DbConnectionExtensions
    {
        public static DbCommand CreateCommand(this DbConnection connection, string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return command;
        }
    }

    internal static class DbCommandExtensions
    {
        public static DbCommand AddParameter(this DbCommand command, string parameterName, DbType dbType, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return command;
        }
    }
}