using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace bibliotecaMVC.Data
{
    public class NumericRoundAbortConnectionInterceptor : DbConnectionInterceptor
    {
        private const string Comando = "SET NUMERIC_ROUNDABORT OFF";

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = Comando;
            cmd.ExecuteNonQuery();
        }

        public override async Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = Comando;
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}