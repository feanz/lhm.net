using System.Collections.Generic;
using System.Data;

namespace lhm.net
{
    public interface ILhmConnection
    {
        IDbConnection DbConnection { get; }

        IDbTransaction BeginTransaction();

        int Execute(string sql, object param = null, IDbTransaction transaction = null);

        T ExecuteScalar<T>(string sql, object param = null);

        IEnumerable<T> Query<T>(string sql, object param = null);

        void Close();
    }
}