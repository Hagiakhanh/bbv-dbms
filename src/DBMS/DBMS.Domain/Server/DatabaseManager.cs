using System;
using System.Collections.Generic;
using DBMS.Domain.Catalog;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Factory;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Security;
using DBMS.Domain.Storage;

namespace DBMS.Domain.Server;

public interface IDatabaseManager
{
    void CreateDatabase(string name);
    void CreateDatabase(DatabaseCreationOptions options);
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
    private static volatile DatabaseManager? _instance;
    private static readonly object _lock = new();

    private readonly IDatabaseFactory _databaseFactory;
    private readonly ICatalogManager _catalog;
    private readonly IConnectionPool _connectionPool;
    private readonly IStorageEngine _storageEngine;
    private readonly IBufferPool _bufferPool;
    private readonly IFileManager _fileManager;
    private readonly ISecurityManager _securityManager;

    private DatabaseManager(
        ICatalogManager catalog, 
        IConnectionPool connectionPool, 
        IStorageEngine storageEngine, 
        IBufferPool bufferPool, 
        IFileManager fileManager, 
        ISecurityManager securityManager)
        : this(new DatabaseFactory(catalog, storageEngine, securityManager), catalog, connectionPool, storageEngine, bufferPool, fileManager, securityManager)
    {
    }

    private DatabaseManager(
        IDatabaseFactory databaseFactory,
        ICatalogManager catalog, 
        IConnectionPool connectionPool, 
        IStorageEngine storageEngine, 
        IBufferPool bufferPool, 
        IFileManager fileManager, 
        ISecurityManager securityManager)
    {
        _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
        _storageEngine = storageEngine ?? throw new ArgumentNullException(nameof(storageEngine));
        _bufferPool = bufferPool ?? throw new ArgumentNullException(nameof(bufferPool));
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        _securityManager = securityManager ?? throw new ArgumentNullException(nameof(securityManager));
    }

    public static DatabaseManager Instance
    {
        get
        {
            if (_instance is null)
            {
                throw new InvalidOperationException("DatabaseManager has not been initialized. Call Initialize first.");
            }
            return _instance;
        }
    }

    public static DatabaseManager Initialize(
        ICatalogManager catalog, 
        IConnectionPool connectionPool, 
        IStorageEngine storageEngine, 
        IBufferPool bufferPool, 
        IFileManager fileManager, 
        ISecurityManager securityManager)
    {
        if (_instance is null)
        {
            lock (_lock)
            {
                if (_instance is null)
                {
                    _instance = new DatabaseManager(catalog, connectionPool, storageEngine, bufferPool, fileManager, securityManager);
                }
            }
        }
        return _instance;
    }

    public static DatabaseManager Initialize(
        IDatabaseFactory databaseFactory,
        ICatalogManager catalog, 
        IConnectionPool connectionPool, 
        IStorageEngine storageEngine, 
        IBufferPool bufferPool, 
        IFileManager fileManager, 
        ISecurityManager securityManager)
    {
        if (_instance is null)
        {
            lock (_lock)
            {
                if (_instance is null)
                {
                    _instance = new DatabaseManager(databaseFactory, catalog, connectionPool, storageEngine, bufferPool, fileManager, securityManager);
                }
            }
        }
        return _instance;
    }

    public static void ResetInstanceForTesting()
    {
        lock (_lock)
        {
            _instance = null;
        }
    }

    public void CreateDatabase(string name)
    {
        //CreateDatabase(new DatabaseCreationOptions { Name = name });
        throw new NotImplementedException();
    }

    public void CreateDatabase(DatabaseCreationOptions options)
    {
        // if (options == null) throw new ArgumentNullException(nameof(options));
        // if (string.IsNullOrEmpty(options.Name)) throw new InvalidNameException("Invalid database name");
        // if (!_securityManager.CheckPermission(options.Name, 0, "CREATE")) throw new PermissionDeniedException("Permission denied");
        // if (_catalog.CheckExists(options.Name)) throw new DuplicateNameException("Duplicate database name");

        // // Factory Method delegates database object construction & initialization
        // _databaseFactory.Create(options);
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
        // if (_catalog.CheckExists(newName)) throw new DuplicateNameException("Duplicate database name");
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
