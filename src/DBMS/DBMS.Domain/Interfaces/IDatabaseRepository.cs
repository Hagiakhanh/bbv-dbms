using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS.Domain.Interfaces
{
    public interface IDatabaseRepository
    {
        Task<Database> CreateAsync(Database database, CancellationToken cancellationToken = default);
        Task<IEnumerable<Database>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Database?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    }
}
