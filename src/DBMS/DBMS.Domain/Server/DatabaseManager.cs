using System;
using System.Collections.Generic;
using DBMS.Domain.Catalog;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Server;
using DBMS.Domain.Storage;
using DBMS.Domain.Security;

namespace DBMS.Domain.Server;

public interface IDatabaseManager
{
    void CreateDatabase(string name);
    void DropDatabase(string name, bool cascade = false);
    Database GetDatabase(string name);
    IEnumerable<Database> ListDatabases();
    void OpenDatabase(string name);
    void CloseDatabase(string name);
    void RenameDatabase(string oldName, string newName);
    void SetDatabaseState(string name, DatabaseState state);
    void AttachDatabase(string name, string filePath);
    void DetachDatabase(string name);
}

public class DatabaseManager : IDatabaseManager
{
    private readonly ICatalogManager _catalog;
    private readonly IConnectionPool _connectionPool;
    private readonly IStorageEngine _storageEngine;
    private readonly IBufferPool _bufferPool;
    private readonly IFileManager _fileManager;
    private readonly ISecurityManager _securityManager;

    public DatabaseManager(ICatalogManager catalog, 
        IConnectionPool connectionPool, 
        IStorageEngine storageEngine, 
        IBufferPool bufferPool, 
        IFileManager fileManager, 
        ISecurityManager securityManager)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
        _storageEngine = storageEngine ?? throw new ArgumentNullException(nameof(storageEngine));
        _bufferPool = bufferPool ?? throw new ArgumentNullException(nameof(bufferPool));
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        _securityManager = securityManager ?? throw new ArgumentNullException(nameof(securityManager));
    }

    public void CreateDatabase(string name)
    {
        // if (string.IsNullOrEmpty(name)) throw new InvalidNameException("Invalid name");
        // if (_catalog.CheckExists(name)) throw new DuplicateNameException("Duplicate name");
        // if (!_securityManager.CheckPermission(name, 0, "CREATE")) throw new PermissionDeniedException("Permission denied");
        
        // _catalog.RegisterDatabase(name);
        throw new NotImplementedException();
    }

    public void DropDatabase(string name, bool cascade = false)
    {
        // if (!_catalog.CheckExists(name)) return;
        
        // if (_connectionPool.HasActiveConnections(name))
        // {
        //     if (cascade)
        //     {
        //         _connectionPool.ForceCloseConnections(name);
        //     }
        //     else
        //     {
        //         throw new DatabaseInUseException("Database in use");
        //     }
        // }
        
        // if (!cascade && _catalog.HasSchemas(name))
        // {
        //     throw new DatabaseContainsSchemasException("Database contains schemas");
        // }
        
        // _catalog.RemoveDatabase(name);
        throw new NotImplementedException();
    }

    public Database GetDatabase(string name)
    {
        // return _catalog.GetDatabase(name);
        throw new NotImplementedException();
    }

    public IEnumerable<Database> ListDatabases()
    {
        // return _catalog.ListDatabases();
        throw new NotImplementedException();
    }

    public void OpenDatabase(string name)
    {
        // if (_catalog.GetDatabaseState(name) == DatabaseState.Offline)
        // {
        //     throw new DatabaseOfflineException("Database offline");
        // }
        
        // _storageEngine.InitializeStorageEngine(name);
        // _catalog.LoadCatalog(name);
        throw new NotImplementedException();
    }

    public void CloseDatabase(string name)
    {
        // _bufferPool.FlushDirtyBuffers(name);
        throw new NotImplementedException();
    }

    public void RenameDatabase(string oldName, string newName)
    {
        // if (_catalog.CheckExists(newName)) throw new DuplicateNameException("Duplicate name");
        // _catalog.UpdateDatabaseName(oldName, newName);
        throw new NotImplementedException();
    }

    public void SetDatabaseState(string name, DatabaseState state)
    {
        // _catalog.UpdateState(name, state);
        throw new NotImplementedException();
    }

    public void AttachDatabase(string name, string filePath)
    {
        // _fileManager.ValidateFilesExist(filePath);
        // _catalog.RegisterExistingDatabaseFiles(name, filePath);
        throw new NotImplementedException();
    }

    public void DetachDatabase(string name)
    {
        // _catalog.Unregister(name);
        throw new NotImplementedException();
    }
}
