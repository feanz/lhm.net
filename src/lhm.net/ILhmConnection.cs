using System.Collections.Generic;
using System.Data;

namespace lhm.net
{
    public interface ILhmConnection
    {
        IDbConnection DbConnection { get; }

        int Execute(string sql, object param = null, IDbTransaction transaction = null);

        IEnumerable<T> Query<T>(string sql, object param = null);

        void Close();

        IDbTransaction BeginTransaction();
    }
}