using System;
using System.Collections.Generic;
using DBMS.Domain.Catalog;
using DBMS.Domain.Core;
using DBMS.Domain.Storage;

namespace DBMS.Domain.Server;

public class DatabaseManager
{
    private readonly ICatalogManager _catalog;
    private readonly IConnectionPool _connectionPool;
    private readonly IStorageEngine _storageEngine;
    private readonly IBufferPool _bufferPool;
    private readonly IFileManager _fileManager;

    public DatabaseManager(ICatalogManager catalog, IConnectionPool connectionPool, IStorageEngine storageEngine, IBufferPool bufferPool, IFileManager fileManager)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
        _storageEngine = storageEngine ?? throw new ArgumentNullException(nameof(storageEngine));
        _bufferPool = bufferPool ?? throw new ArgumentNullException(nameof(bufferPool));
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
    }

    public void CreateDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public void DropDatabase(string name, bool cascade = false)
    {
        throw new NotImplementedException();
    }

    public Database GetDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Database> ListDatabases()
    {
        throw new NotImplementedException();
    }

    public void OpenDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public void CloseDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public void RenameDatabase(string oldName, string newName)
    {
        throw new NotImplementedException();
    }

    public void SetDatabaseState(string name, DatabaseState state)
    {
        throw new NotImplementedException();
    }

    public void AttachDatabase(string name, string filePath)
    {
        throw new NotImplementedException();
    }

    public void DetachDatabase(string name)
    {
        throw new NotImplementedException();
    }
}
