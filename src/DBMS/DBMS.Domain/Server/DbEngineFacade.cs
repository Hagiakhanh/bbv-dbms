using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Storage;
using DBMS.Domain.Transactions;

namespace DBMS.Domain.Server;

public class DbEngineFacade : IDbEngineFacade
{
    private readonly IDiskManager _diskManager;
    private readonly IStorageEngine _storageEngine;
    private readonly ICatalogManager _catalogManager;
    private readonly ITransactionManager _transactionManager;
    private readonly IRecoveryManager _recoveryManager;

    public DbEngineFacade(
        IDiskManager diskManager,
        IStorageEngine storageEngine,
        ICatalogManager catalogManager,
        ITransactionManager transactionManager,
        IRecoveryManager recoveryManager)
    {
        _diskManager = diskManager ?? throw new ArgumentNullException(nameof(diskManager));
        _storageEngine = storageEngine ?? throw new ArgumentNullException(nameof(storageEngine));
        _catalogManager = catalogManager ?? throw new ArgumentNullException(nameof(catalogManager));
        _transactionManager = transactionManager ?? throw new ArgumentNullException(nameof(transactionManager));
        _recoveryManager = recoveryManager ?? throw new ArgumentNullException(nameof(recoveryManager));
    }

    public void Start(bool safeMode)
    {
        throw new NotImplementedException();
    }

    public void Stop(bool force)
    {
        throw new NotImplementedException();
    }

    public void Restart()
    {
        throw new NotImplementedException();
    }

    public void Recover()
    {
        throw new NotImplementedException();
    }
}
