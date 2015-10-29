using System;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace lhm.net
{
    public class LhmConnection : ILhmConnection, IDisposable
    {
        private IDbConnection _connection;

        public LhmConnection(IDbConnection connection)
        {
            _connection = connection;
        }

        public IDbConnection DbConnection
        {
            get { return _connection; }
        }

        public int Execute(string sql, object param = null,IDbTransaction transaction = null)
        {
            return _connection.Execute(sql, param, transaction);
        }

        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            return _connection.Query<T>(sql, param);
        }

        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }
    }
}