using System;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Security;
using DBMS.Domain.Storage;

namespace DBMS.Domain.DatabaseObjects.Databases;

public class DatabaseFactory : IDatabaseFactory
{
    private readonly ICatalogManager _catalog;
    private readonly IStorageEngine _storageEngine;
    private readonly ISecurityManager _securityManager;

    public DatabaseFactory(
        ICatalogManager catalog,
        IStorageEngine storageEngine,
        ISecurityManager securityManager)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _storageEngine = storageEngine ?? throw new ArgumentNullException(nameof(storageEngine));
        _securityManager = securityManager ?? throw new ArgumentNullException(nameof(securityManager));
    }

    public Database Create(DatabaseCreationOptions options)
    {
        // if (options == null)
        //     throw new ArgumentNullException(nameof(options));

        // if (string.IsNullOrWhiteSpace(options.Name))
        //     throw new InvalidNameException("Invalid database name");

        // // 1. Allocate storage for the new database
        // _storageEngine.AllocateDatabase(options.Name);

        // // 2. Instantiate Database object (Product)
        // var database = new Database(0, options.Name, options.Owner ?? "sa");

        // // 3. Register database into catalog
        // _catalog.RegisterDatabase(options.Name);

        // // 4. Grant ownership in security system
        // _securityManager.GrantOwnership(options.Name, options.Owner ?? "sa");

        // return database;

        return null;
    }
}
