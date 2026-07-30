global using DBMS.Domain.Core;
global using DBMS.Domain.Exceptions;

// Module 1: Server Administration
global using DBMS.Domain.Server;
global using DBMS.Domain.Server.States;
global using DBMS.Domain.Server.Facade;
global using DBMS.Domain.Server.Commands;

// Module 2: Authentication & Security
global using DBMS.Domain.Security;

// Module 5: Database Object Management
global using DBMS.Domain.DatabaseObjects.Common;
global using DBMS.Domain.DatabaseObjects.Databases;
global using DBMS.Domain.DatabaseObjects.Schemas;
global using DBMS.Domain.DatabaseObjects.Tables;
global using DBMS.Domain.DatabaseObjects.Columns;
global using DBMS.Domain.DatabaseObjects.Constraints;
global using DBMS.Domain.DatabaseObjects.Indexes;

// Module 6: Catalog & Metadata
global using DBMS.Domain.Catalog;
global using DBMS.Domain.Catalog.Services;
global using DBMS.Domain.Catalog.Exporters;
global using DBMS.Domain.Catalog.Events;
global using DBMS.Domain.Catalog.Iterators;
global using DBMS.Domain.Catalog.ScriptGenerators;

// Module 7: Query Processing APIs
global using DBMS.Domain.QueryProcessing.Models;
global using DBMS.Domain.QueryProcessing.Parsing;
global using DBMS.Domain.QueryProcessing.Optimization;
global using DBMS.Domain.QueryProcessing.Execution;
global using DBMS.Domain.QueryProcessing.Commands;

// Module 8: Transaction & Concurrency
global using DBMS.Domain.Transactions;

// Subsystem: Storage Engine
global using DBMS.Domain.Storage;
global using DBMS.Domain.Storage.Engine;
global using DBMS.Domain.Storage.Proxies;
global using DBMS.Domain.Storage.Policies;

// Application Services
global using DBMS.Domain.Services;
global using DBMS.Domain.Services.Facade;

// Namespace aliases
global using Index = DBMS.Domain.DatabaseObjects.Indexes.Index;
