using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        // Commit tất cả thay đổi đang chờ vào database
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Quản lý transaction tường minh khi cần
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
