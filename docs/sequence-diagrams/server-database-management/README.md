# Server & Database Management
## 1. Class Diagram
```mermaid
classDiagram
direction LR
class DatabaseServer {
    +ServerId : int
    +Version : string
    +Status : ServerStatus
    +Start(safeMode : bool)
    +Stop(force : bool)
    +Restart()
    +Recover()
    +HandleSignal(signal : string)
    +GetStatus() ServerStatus
}
class DatabaseManager {
    -_catalog : ICatalogManager
    -_connectionPool : IConnectionPool
    +DatabaseManager(catalog : ICatalogManager, connectionPool : IConnectionPool)
    +CreateDatabase(name : string)
    +DropDatabase(name : string, cascade : bool)
    +GetDatabase(name : string) Database
    +ListDatabases() IEnumerable~Database~
    +OpenDatabase(name : string)
    +CloseDatabase(name : string)
    +RenameDatabase(oldName : string, newName : string)
    +SetDatabaseState(name : string, state : DatabaseState)
    +AttachDatabase(name : string, filePath : string)
    +DetachDatabase(name : string)
}
class ConfigurationManager {
    -configData : Map~string, string~
    +LoadConfiguration(filePath : string)
    +UpdateConfiguration(key : string, value : string)
    +GetConfiguration(key : string) string
}
class SecurityManager {
    -userDb : Map~string, HashedCredential~
    +Authenticate(username : string, password : string) Session
    +CheckPermission(user : string, obj : int, action : string) bool
    +GrantRole(user : string, role : string)
    +RevokeRole(user : string, role : string)
}
note for SecurityManager "Authenticate() throws\nPermissionDeniedException if invalid"
class MonitoringManager {
    -metrics : ServerMetrics
    +CollectMetrics()
    +GetMetrics() ServerMetrics
}
DatabaseServer --> DatabaseManager
DatabaseServer --> ConfigurationManager
DatabaseServer --> SecurityManager
DatabaseServer --> MonitoringManager
```
## 2. Sequence Diagrams
### 2.1 DatabaseServer Operations
#### Start Server Flow
Covers: `Start_ShouldInitializeAllServices`, `Start_ShouldOpenNetworkPortForConnections`, `Start_ShouldStartBackgroundWorkers`, `Start_ShouldStartInSafeMode_WhenConfigured`, `Start_ShouldReject_WhenServerAlreadyRunning`
```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant Server as DatabaseServer
    participant Network as NetworkListener
    participant Worker as BackgroundWorker
    participant Config as ConfigurationManager
    Admin->>Server: Start(safeMode)
    alt Status == Running
        Server-->>Admin: Throw ServerAlreadyRunningException
    else Status != Running
        Server->>Config: LoadConfiguration()
        
        alt safeMode == true
            Note over Server: StartInSafeMode() - Bypass user plugins/extensions
        else safeMode == false
            Server->>Server: InitializeAllServices()
        end
        
        Server->>Worker: StartBackgroundWorkers()
        Server->>Network: OpenNetworkPortForConnections()
        Server->>Server: Status = Running
        Server-->>Admin: Server Started Successfully
    end
```
#### Stop & Restart Server Flow
Covers: `Stop_ShouldShutdownAllServices`, `Stop_ShouldFlushDirtyPagesBeforeShutdown`, `Stop_ShouldRejectNewConnections_WhileShuttingDown`, `Stop_ShouldWaitForActiveTransactions_WhenGraceful`, `Stop_ShouldTerminateActiveConnections_WhenForced`, `Restart_ShouldRestartServerSuccessfully`, `HandleSignal_ShouldInitiateGracefulShutdown`, `GetStatus_ShouldReturnCorrectServerState`
```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant OS as Operating System
    participant Server as DatabaseServer
    participant Network as NetworkListener
    participant TxMgr as TransactionManager
    participant Buffer as BufferPool
    OS->>Server: HandleSignal(SIGTERM)
    Note right of OS: Initiates Graceful Shutdown
    
    Admin->>Server: GetStatus()
    Server-->>Admin: Return Current Status
    
    Admin->>Server: Stop(force)
    Server->>Network: RejectNewConnections()
    
    alt force == false (Graceful)
        Server->>TxMgr: WaitForActiveTransactions()
    else force == true (Forced)
        Server->>TxMgr: TerminateActiveConnections()
        Server->>TxMgr: AbortActiveTransactions()
    end
    
    Server->>Buffer: FlushDirtyPagesBeforeShutdown()
    Server->>Server: ShutdownAllServices()
    Server->>Server: Status = Stopped
    Server-->>Admin: Server Stopped
    opt Restart
        Admin->>Server: Restart()
        Server->>Server: Stop(force=false)
        Server->>Server: Start(safeMode=false)
        Server-->>Admin: Server Restarted
    end
```
#### Crash Recovery Flow
Covers: `RecoverAfterCrash_ShouldReplayWAL`
```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant Server as DatabaseServer
    participant Recovery as RecoveryManager
    participant WAL as WALManager
    Admin->>Server: Start()
    Server->>Server: Detect Improper Shutdown
    Server->>Recovery: Recover()
    Recovery->>WAL: ReadLogFromLastCheckpoint()
    loop For each log record
        Recovery->>Recovery: ReplayWAL (Redo)
        Recovery->>Recovery: UndoUncommitted()
    end
    Recovery-->>Server: Recovery Complete
    Server->>Server: Status = Running
```
### 2.2 DatabaseManager Operations
#### Create & Drop Database
Covers: `CreateDatabase_ShouldCreateDatabaseSuccessfully`, `CreateDatabase_ShouldRejectDuplicateDatabaseName`, `CreateDatabase_ShouldRejectInvalidName`, `DropDatabase_ShouldRemoveDatabaseSuccessfully`, `DropDatabase_ShouldRejectOpenDatabase`, `DropDatabase_ShouldForceCloseConnections_WhenCascade`
```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant DBManager as DatabaseManager
    participant Catalog as CatalogManager
    participant Connection as ConnectionPool
    %% Create Database
    Client->>DBManager: CreateDatabase("NewDB")
    alt name is invalid (e.g. symbols/spaces)
        DBManager-->>Client: Throw InvalidNameException
    else name is valid
        DBManager->>Catalog: CheckExists("NewDB")
        alt exists == true
            DBManager-->>Client: Throw DuplicateNameException
        else exists == false
            DBManager->>Catalog: RegisterDatabase("NewDB")
            DBManager-->>Client: Success
        end
    end
    %% Drop Database
    Client->>DBManager: DropDatabase("OldDB", cascade)
    DBManager->>Catalog: CheckExists("OldDB")
    DBManager->>Connection: HasActiveConnections("OldDB")
    
    alt Active connections > 0
        alt cascade == true
            DBManager->>Connection: ForceCloseConnections("OldDB")
        else cascade == false
            DBManager-->>Client: Throw DatabaseInUseException (RejectOpenDatabase)
        end
    end
    
    DBManager->>Catalog: RemoveDatabase("OldDB")
    DBManager-->>Client: Success
```
#### Open, Close & Rename Database
Covers: `OpenDatabase_ShouldLoadStorageAndCatalog`, `OpenDatabase_ShouldReject_WhenDatabaseIsOffline`, `CloseDatabase_ShouldFlushDirtyBuffers`, `RenameDatabase_ShouldUpdateNameSuccessfully`, `RenameDatabase_ShouldRejectDuplicateName`
```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant DBManager as DatabaseManager
    participant Catalog as CatalogManager
    participant Buffer as BufferPool
    participant Storage as StorageEngine
    %% Open
    Client->>DBManager: OpenDatabase("MyDb")
    DBManager->>Catalog: GetDatabaseState("MyDb")
    alt State == Offline
        DBManager-->>Client: Throw DatabaseOfflineException
    else State == Online
        DBManager->>Storage: InitializeStorageEngine("MyDb")
        DBManager->>Catalog: LoadCatalog("MyDb")
        DBManager-->>Client: Database Opened
    end
    %% Close
    Client->>DBManager: CloseDatabase("MyDb")
    DBManager->>Buffer: FlushDirtyBuffers("MyDb")
    DBManager-->>Client: Database Closed
    %% Rename
    Client->>DBManager: RenameDatabase("MyDb", "NewName")
    DBManager->>Catalog: CheckExists("NewName")
    alt exists == true
        DBManager-->>Client: Throw DuplicateNameException
    else
        DBManager->>Catalog: UpdateDatabaseName("MyDb", "NewName")
        DBManager-->>Client: Name Updated
    end
```
#### Database State & Attachment
Covers: `SetDatabaseState_ShouldSetToReadOnly`, `SetDatabaseState_ShouldSetToOffline`, `AttachDatabase_ShouldRegisterExistingDatabaseFiles`, `DetachDatabase_ShouldUnregisterButKeepFiles`
```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant DBManager as DatabaseManager
    participant Catalog as CatalogManager
    participant FileSys as FileSystem
    %% Change State
    Admin->>DBManager: SetDatabaseState("MyDb", ReadOnly / Offline)
    DBManager->>Catalog: UpdateState("MyDb", newState)
    DBManager-->>Admin: State Updated
    %% Attach
    Admin->>DBManager: AttachDatabase("ArchivedDb", "/data/archived.db")
    DBManager->>FileSys: ValidateFilesExist("/data/archived.db")
    DBManager->>Catalog: RegisterExistingDatabaseFiles("ArchivedDb", "/data/archived.db")
    DBManager-->>Admin: Attached Successfully
    %% Detach
    Admin->>DBManager: DetachDatabase("ArchivedDb")
    DBManager->>Catalog: Unregister("ArchivedDb")
    Note over FileSys: Files are NOT deleted
    DBManager-->>Admin: Detached Successfully
```
### 2.3 Configuration & Monitoring
#### Configuration Management
Covers: `LoadConfiguration_ShouldLoadServerConfiguration`, `LoadConfiguration_ShouldUseDefaultConfiguration_WhenFileNotExists`, `UpdateConfiguration_ShouldPersistChanges`, `GetConfiguration_ShouldReturnConfiguredValue`
```mermaid
sequenceDiagram
    autonumber
    actor System
    participant Config as ConfigurationManager
    participant Disk as FileSystem
    System->>Config: LoadConfiguration("config.json")
    Config->>Disk: ReadFile("config.json")
    alt file exists
        Config->>Config: Parse config file
    else file not exists
        Config->>Config: Apply Default Configuration
    end
    Config-->>System: Loaded
    System->>Config: UpdateConfiguration("MaxConnections", "1000")
    Config->>Config: Update Memory Map
    Config->>Disk: PersistChanges("config.json")
    Config-->>System: Success
    System->>Config: GetConfiguration("MaxConnections")
    Config-->>System: "1000"
```
#### Monitoring Management
Covers: `CollectMetrics_ShouldCollectServerMetrics`, `CollectMetrics_ShouldCollectBufferPoolStatistics`, `CollectMetrics_ShouldCollectTransactionStatistics`, `GetMetrics_ShouldReturnLatestMetrics`
```mermaid
sequenceDiagram
    autonumber
    participant Timer as BackgroundTimer
    participant Monitor as MonitoringManager
    participant Buffer as BufferPool
    participant TxMgr as TransactionManager
    actor Client
    Timer->>Monitor: CollectMetrics()
    Monitor->>Buffer: GetBufferPoolStatistics()
    Monitor->>TxMgr: GetTransactionStatistics()
    Monitor->>Monitor: Aggregate Server Metrics
    
    Client->>Monitor: GetMetrics()
    Monitor-->>Client: Return Latest ServerMetrics
```
### 2.4 Integration Flow
#### Full Server Boot & DB Initialization Flow
Covers: `StartServer_ShouldLoadConfigurationBeforeInitializingServices`, `StartServer_ShouldInitializeDatabaseManager`, `StartServer_ShouldInitializeStorageEngine`, `CreateDatabase_ShouldRegisterDatabaseInCatalog`, `OpenDatabase_ShouldInitializeStorageEngine`
```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant Server as DatabaseServer
    participant Config as ConfigurationManager
    participant DBManager as DatabaseManager
    participant Storage as StorageEngine
    participant Catalog as CatalogManager
    Admin->>Server: Start()
    Server->>Config: LoadConfiguration()
    Note over Server, Config: LoadConfiguration Before Initializing Services
    
    Server->>DBManager: Initialize()
    Server->>Storage: Initialize()
    
    Admin->>DBManager: CreateDatabase("AppDB")
    DBManager->>Catalog: RegisterDatabaseInCatalog("AppDB")
    
    Admin->>DBManager: OpenDatabase("AppDB")
    DBManager->>Storage: InitializeStorageEngine("AppDB")
```
#### Full Server Shutdown Flow
Covers: `StopServer_ShouldFlushDirtyPagesBeforeShutdown`, `StopServer_ShouldShutdownAllManagers`, `CloseDatabase_ShouldFlushPendingChanges`
```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant Server as DatabaseServer
    participant DBManager as DatabaseManager
    participant Buffer as BufferPool
    participant Storage as StorageEngine
    Admin->>Server: Stop(graceful)
    Server->>DBManager: Close All Databases
    
    loop For each open DB
        DBManager->>DBManager: CloseDatabase()
        DBManager->>Buffer: FlushPendingChanges()
    end
    
    Server->>Buffer: FlushDirtyPagesBeforeShutdown()
    Server->>DBManager: Shutdown()
    Server->>Storage: Shutdown()
    Note over Server, Storage: ShutdownAllManagers
    
    Server->>Server: Status = Stopped
    Server-->>Admin: Stopped
```
