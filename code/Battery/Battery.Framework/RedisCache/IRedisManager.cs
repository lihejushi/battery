using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using StackExchange.Redis;

namespace TuoKe.Framework.RedisCache
{
    public interface IRedisManager
    {
        IDatabase Database
        {
            get;
        }
    }

    public class RedisManager : IRedisManager
    {
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionMultiplexers = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private const string SERVICEKEY = "Redis.Cache";

        public IDatabase Database
        {
            get
            {
                return _connectionMultiplexer.GetDatabase();
            }
        }

        public RedisManager()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[SERVICEKEY];

            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException("A connection string is expected for " + SERVICEKEY);
            }
            string connectionString = connectionStringSettings.ConnectionString;
            _connectionMultiplexer = ConnectionMultiplexers.GetOrAdd(
                    connectionString,
                    ConnectionMultiplexer.Connect(connectionString)
            );
        }
    }
}
