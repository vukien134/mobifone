using System.Threading.Tasks;

namespace Accounting.Data;

public interface IAccountingDbSchemaMigrator
{
    Task MigrateAsync();
    Task MigrateAsync(string connStr);
}
