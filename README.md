# This overview of DBMS Mindmap

<!-- ```mermaid
graph LR
    %% Styles cho nền tối (GitHub Dark Mode)
    classDef default fill:#2d2d2d,stroke:#888,stroke-width:1px,color:#fff;
    classDef root fill:#ff8888,stroke:#ff5555,stroke-width:2px,font-weight:bold,color:#000;
    classDef layer1 fill:#99ccff,stroke:#5ba3e3,stroke-width:1.5px,font-weight:bold,color:#000;
    classDef layer2 fill:#5fbb97,stroke:#3a9672,stroke-width:1.5px,font-weight:bold,color:#000;

    %% Tăng độ tương phản cho đường nối liên kết giữa các node
    linkStyle default stroke:#ffffff,stroke-width:1.5px;

    db((DBMS)):::root

    %% ===== BÊN TRÁI (module 1-4) =====

    %% 1. Query Processor
    qp[1. Query Processor]:::layer1 --- db
    qp_sp[SQL Parser]:::layer2 --- qp
    qp_qo[Query Optimizer]:::layer2 --- qp
    qp_qe[Query Execution]:::layer2 --- qp
    qp_qv[Query Validation]:::layer2 --- qp
    qp_rp[Result Processing]:::layer2 --- qp

    %% 2. Storage Engine
    se[2. Storage Engine]:::layer1 --- db
    se_df[Data File Manager]:::layer2 --- se
    se_pm[Page Manager]:::layer2 --- se
    se_bp[Buffer Pool + Cache]:::layer2 --- se
    se_rm[Record Management]:::layer2 --- se
    se_im[Index Management]:::layer2 --- se
    se_am[Access Methods]:::layer2 --- se
    se_sa[Storage Allocation]:::layer2 --- se
    se_lf[Log File / WAL]:::layer2 --- se

    %% 3. Transaction & Concurrency
    tx[3. Transaction & Concurrency]:::layer1 --- db
    tx_cc[Concurrency Control]:::layer2 --- tx
    tx_dl[Deadlock Handler]:::layer2 --- tx
    tx_tm[Transaction Manager]:::layer2 --- tx
    tx_lm[Lock Manager]:::layer2 --- tx
    tx_im[Isolation Management]:::layer2 --- tx

    %% 4. Backup & Durability
    bd[4. Backup & Durability]:::layer1 --- db
    bd_bm[Backup Management]:::layer2 --- bd
    bd_rm[Restore Management]:::layer2 --- bd
    bd_tl[Transaction Logging]:::layer2 --- bd
    bd_re[Recovery Manager]:::layer2 --- bd
    bd_cp[Checkpoint Manager]:::layer2 --- bd
    bd_rp[Replication & HA]:::layer2 --- bd

    %% ===== BÊN PHẢI (module 5-8) =====

    %% 5. Performance & Memory
    db --- pf[5. Performance & Memory]:::layer1
    pf --- pf_qa[Performance Analyzer]:::layer2
    pf --- pf_ch[Caching Systems]:::layer2
    pf --- pf_mm[Memory Management]:::layer2
    pf --- pf_dd[Data Distribution]:::layer2
    pf --- pf_ct[Connection & Threads]:::layer2

    %% 6. Database Object Management
    db --- om[6. Object Management]:::layer1
    om --- om_dm[Database Management]:::layer2
    om --- om_sm[Schema Management]:::layer2
    om --- om_tm[Table Management]:::layer2
    om --- om_vm[View Management]:::layer2
    om --- om_rm[Relationship Management]:::layer2
    om --- om_idx[Index Definition]:::layer2
    om --- om_cm[Constraint Management]:::layer2
    om --- om_com[Column Management]:::layer2
    om --- om_po[Programmable Objects]:::layer2
    om --- om_dt[Data Type System]:::layer2
    om --- om_mm[Metadata Management]:::layer2

    %% 7. Security & Access Control
    db --- sa[7. Security & Access Control]:::layer1
    sa --- sa_au[Authentication]:::layer2
    sa --- sa_az[Authorization]:::layer2
    sa --- sa_ac[Access Control Filters]:::layer2
    sa --- sa_um[User Management]:::layer2
    sa --- sa_ec[Encryption Engine]:::layer2
    sa --- sa_ad[Auditing]:::layer2

    %% 8. Administration & Monitoring
    db --- am[8. Admin & Monitoring]:::layer1
    am --- am_bs[Background Strategy]:::layer2
    am --- am_ml[Monitoring & Logging]:::layer2
    am --- am_cm[Configuration]:::layer2
    am --- am_ie[Import & Export]:::layer2
``` -->

![alt text](image.png)

## Diagram 10 — Full Dependency Overview (Cross-Layer)

```mermaid
classDiagram
    %% Query layer
    SqlParser --> QueryValidator
    QueryValidator --> QueryOptimizer
    QueryOptimizer --> ExecutionEngine
    ExecutionEngine --> ResultProcessor
    ExecutionEngine --> IAccessMethod
    ExecutionEngine --> IAuthorizationManager
    ExecutionEngine --> TransactionManager
    ExecutionEngine --> IMetadataCatalog

    %% Access methods
    IAccessMethod <|.. TableScan
    IAccessMethod <|.. IndexScan
    TableScan --> RecordManager
    IndexScan --> IIndexStructure

    %% Storage
    RecordManager --> IBufferPool
    IndexManager --> IBufferPool
    IndexManager --> IIndexStructure
    IIndexStructure <|.. BPlusTree
    IIndexStructure <|.. HashIndex
    IBufferPool <|.. BufferPoolManager
    BufferPoolManager --> DiskManager
    BufferPoolManager --> IReplacementPolicy
    IReplacementPolicy <|.. LRUPolicy
    IReplacementPolicy <|.. ClockPolicy

    %% Logging
    IWALManager <|.. WALManager
    WALManager --> ILogWriter
    ILogWriter <|.. LogWriter
    WALManager --> LSNGenerator

    %% Transaction
    TransactionManager --> LockManager
    TransactionManager --> IWALManager
    TransactionManager --> MVCCManager
    TransactionManager --> TransactionTable
    LockManager --> DeadlockDetector
    MVCCManager --> ISnapshotProvider
    ISnapshotProvider <|.. SnapshotManager

    %% Recovery & Backup
    RecoveryManager --> IWALManager
    RecoveryManager --> IBufferPool
    BackupManager --> IWALManager

    %% Metadata & Security
    IMetadataCatalog <|.. MetadataCatalog
    DatabaseManager --> IMetadataCatalog
    TableManager --> IMetadataCatalog
    IAuthorizationManager <|.. AuthorizationManager
    AuthorizationManager --> IMetadataCatalog
```

---

## Diagram 1 — Value Objects & Exception Hierarchy

> This is the foundation of the entire system. Every class uses these types instead of primitives.

```mermaid
classDiagram

    %% ── VALUE OBJECTS ──────────────────────────────────────
    class PageId {
        <<value object>>
        +FileId fileId
        +Int pageNumber
        +equals(other PageId) Boolean
        +toString() String
    }
    class LSN {
        <<value object>>
        +Long value
        +isAfter(other LSN) Boolean
        +next() LSN
    }
    class TransactionId {
        <<value object>>
        +Long id
        +isValid() Boolean
    }
    class RID {
        <<value object>>
        +PageId pageId
        +Int slotNumber
        +equals(other RID) Boolean
    }
    class DatabaseName {
        <<value object>>
        +String value
        +validate()
    }
    class AbsolutePath {
        <<value object>>
        +String path
        +exists() Boolean
        +resolve(child String) AbsolutePath
    }
    class PoolSize {
        <<value object>>
        +Int value
        +validate()
    }
    class BTreeDegree {
        <<value object>>
        +Int value
        +validate()
    }

    %% ── EXCEPTION HIERARCHY ────────────────────────────────
    class DbmsException {
        <<exception>>
        +String message
        +Throwable cause
    }
    class StorageException {
        <<exception>>
    }
    class PageNotFoundException {
        <<exception>>
        +PageId missingPage
    }
    class LockException {
        <<exception>>
    }
    class LockTimeoutException {
        <<exception>>
        +TransactionId txId
        +Long waitedMs
    }
    class DeadlockException {
        <<exception>>
        +TransactionId victim
    }
    class SqlException {
        <<exception>>
    }
    class SqlSyntaxException {
        <<exception>>
        +Int line
        +Int column
    }
    class PermissionDeniedException {
        <<exception>>
        +String user
        +String object
    }

    DbmsException <|-- StorageException
    DbmsException <|-- LockException
    DbmsException <|-- SqlException
    StorageException <|-- PageNotFoundException
    LockException <|-- LockTimeoutException
    LockException <|-- DeadlockException
    SqlException <|-- SqlSyntaxException
    SqlException <|-- PermissionDeniedException

    RID *-- PageId
```

---

---

## Diagram 2 — Interface Contracts (DIP Foundation)

> All high-level classes depend on these interfaces, regardless of their implementations.

```mermaid
classDiagram

    %% ── INTERFACES ─────────────────────────────────────────
    class IBufferPool {
        <<interface>>
        +fetchPage(id PageId) Page
        +unpinPage(id PageId)
        +flushPage(id PageId)
        +markDirty(id PageId)
    }
    class IWALManager {
        <<interface>>
        +append(record LogRecord) LSN
        +flush(upToLSN LSN)
        +truncateBefore(lsn LSN)
    }
    class ILogWriter {
        <<interface>>
        +write(bytes Byte[])
        +fsync()
        +currentOffset() Long
    }
    class IIndexStructure {
        <<interface>>
        +insert(key Key, rid RID)
        +delete(key Key)
        +search(key Key) Optional~RID~
        +rangeScan(from Key, to Key) Iterator~RID~
    }
    class IReplacementPolicy {
        <<interface>>
        +recordAccess(frameId Int)
        +evictFrame() Optional~Int~
        +pin(frameId Int)
        +unpin(frameId Int)
    }
    class ISnapshotProvider {
        <<interface>>
        +createSnapshot(txId TransactionId) SnapshotId
        +releaseSnapshot(id SnapshotId)
        +isVisible(txId TransactionId, snapshotId SnapshotId) Boolean
    }
    class IAccessMethod {
        <<interface>>
        +open(ctx ScanContext)
        +next() Optional~Tuple~
        +close()
    }

    %% ── IMPLEMENTATIONS ────────────────────────────────────
    class BufferPoolManager {
        -Page[] frames
        -PageTable pageTable
        -IReplacementPolicy policy
        -findInCache(id PageId) Optional~Page~
        -loadFromDisk(id PageId) Page
        -evictIfFull()
        +fetchPage(id PageId) Page
        +unpinPage(id PageId)
        +flushPage(id PageId)
        +markDirty(id PageId)
    }
    class WALManager {
        -LogBuffer buffer
        -ILogWriter writer
        -LSNGenerator lsnGen
        -writeToBuffer(record LogRecord) LSN
        -shouldForceFlush() Boolean
        +append(record LogRecord) LSN
        +flush(upToLSN LSN)
        +truncateBefore(lsn LSN)
    }
    class LogWriter {
        -AbsolutePath logFilePath
        -Long currentOffset
        +write(bytes Byte[])
        +fsync()
        +currentOffset() Long
    }
    class BPlusTree {
        -PageId rootPageId
        -BTreeDegree degree
        -IBufferPool bufferPool
        -splitNode(nodeId PageId) PageId
        -propagateSplit(child PageId, parent PageId)
        -findLeaf(key Key) PageId
        +insert(key Key, rid RID)
        +delete(key Key)
        +search(key Key) Optional~RID~
        +rangeScan(from Key, to Key) Iterator~RID~
    }
    class HashIndex {
        -PageId directoryPageId
        -IBufferPool bufferPool
        -resolveChain(hash Int) PageId
        +insert(key Key, rid RID)
        +delete(key Key)
        +search(key Key) Optional~RID~
        +rangeScan(from Key, to Key) Iterator~RID~
    }
    class LRUPolicy {
        -PoolSize frameCount
        -LinkedHashMap~Int~ usageOrder
        +recordAccess(frameId Int)
        +evictFrame() Optional~Int~
        +pin(frameId Int)
        +unpin(frameId Int)
    }
    class ClockPolicy {
        -PoolSize frameCount
        -Boolean[] referenceBits
        -Int clockHand
        +recordAccess(frameId Int)
        +evictFrame() Optional~Int~
        +pin(frameId Int)
        +unpin(frameId Int)
    }
    class SnapshotManager {
        -List~Snapshot~ activeSnapshots
        -Snapshot buildSnapshot(txId TransactionId) Snapshot
        +createSnapshot(txId TransactionId) SnapshotId
        +releaseSnapshot(id SnapshotId)
        +isVisible(txId TransactionId, snapshotId SnapshotId) Boolean
    }
    class TableScan {
        -TableId tableId
        -RID currentRid
        -RecordManager recordMgr
        +open(ctx ScanContext)
        +next() Optional~Tuple~
        +close()
    }
    class IndexScan {
        -IndexId indexId
        -Key searchKey
        -IIndexStructure index
        +open(ctx ScanContext)
        +next() Optional~Tuple~
        +close()
    }

    IBufferPool <|.. BufferPoolManager
    IWALManager <|.. WALManager
    ILogWriter <|.. LogWriter
    IIndexStructure <|.. BPlusTree
    IIndexStructure <|.. HashIndex
    IReplacementPolicy <|.. LRUPolicy
    IReplacementPolicy <|.. ClockPolicy
    ISnapshotProvider <|.. SnapshotManager
    IAccessMethod <|.. TableScan
    IAccessMethod <|.. IndexScan

    WALManager --> ILogWriter
    BufferPoolManager --> IReplacementPolicy
```

---

## Diagram 3 — Query Processor Layer

```mermaid
classDiagram

    class SqlParser {
        -String sqlText
        -tokenize() Token[]
        -buildAST(tokens Token[]) ASTNode
        +parse(sql String) ASTNode
    }
    note for SqlParser "Throws SqlSyntaxException if tokenize() or buildAST() fail"

    class QueryValidator {
        -IMetadataCatalog catalog
        -IAuthorizationManager authz
        -checkTableExists(node ASTNode)
        -checkColumnTypes(node ASTNode)
        +validate(ast ASTNode) ValidatedAst
    }
    note for QueryValidator "Throws PermissionDeniedException or SqlSyntaxException"

    class QueryOptimizer {
        -CostModel costModel
        -IMetadataCatalog catalog
        -toLogicalPlan(ast ValidatedAst) LogicalPlan
        -toPhysicalPlan(plan LogicalPlan) ExecutionPlan
        +optimize(ast ValidatedAst) ExecutionPlan
    }

    class ExecutionEngine {
        -IAccessMethod scanner
        -IAuthorizationManager authz
        -openOperators(plan ExecutionPlan)
        -closeOperators()
        +execute(plan ExecutionPlan, ctx TransactionContext) ResultCursor
    }
    note for ExecutionEngine "plan is a parameter, not a field. Avoid keeping temporary state."

    class ResultProcessor {
        +format(cursor ResultCursor) ResultSet
        +serialize(row Tuple) Byte[]
        +paginate(cursor ResultCursor, pageSize Int) Page~Tuple~
    }
    note for ResultProcessor "No field state — everything is received via parameters."

    SqlParser --> QueryValidator : passes ASTNode
    QueryValidator --> QueryOptimizer : passes ValidatedAst
    QueryOptimizer --> ExecutionEngine : passes ExecutionPlan
    ExecutionEngine --> ResultProcessor : passes ResultCursor
    ExecutionEngine --> IAccessMethod : depends on interface
```

---

## Diagram 4 — Storage Engine Layer

```mermaid
classDiagram

    class DiskManager {
        -Map~FileId, FileHandle~ openFiles
        -AbsolutePath dataDir
        -ensureOpen(path AbsolutePath) FileHandle
        +readPage(id PageId) Byte[]
        +writePage(id PageId, data Byte[])
        +allocateFile(path AbsolutePath) FileId
    }
    note for DiskManager "Throws StorageException if file does not exist"

    class TablespaceManager {
        -Map~TablespaceId, Tablespace~ tablespaces
        -IMetadataCatalog catalog
        +createTablespace(name String, path AbsolutePath)
        +getTablespace(id TablespaceId) Tablespace
        +mapDatabaseToFile(db DatabaseName) AbsolutePath
    }

    class PageManager {
        -DiskManager diskManager
        -SpaceManager spaceMgr
        +allocatePage(tableId TableId) PageId
        +fetchPage(id PageId) RawPage
        +freePage(id PageId)
    }

    class PageFormatter {
        -formatHeader(page RawPage) PageHeader
        -computeFreeSpace(page RawPage) Int
        +formatSlottedPage(page RawPage) FormattedPage
        +insertTuple(page FormattedPage, tuple Byte[]) SlotId
    }
    note for PageFormatter "insertTuple throw StorageException if does not have enough free space"

    class BufferPoolManager {
        -Page[] frames
        -PageTable pageTable
        -IReplacementPolicy policy
        -DiskManager diskManager
        -findInCache(id PageId) Optional~Page~
        -loadFromDisk(id PageId) Page
        -evictIfFull()
        +fetchPage(id PageId) Page
        +unpinPage(id PageId)
        +flushPage(id PageId)
        +markDirty(id PageId)
    }

    class RecordManager {
        -IBufferPool bufferPool
        -RecordLayoutManager layoutMgr
        -RIDGenerator ridGen
        +insertRecord(tableId TableId, data Byte[]) RID
        +deleteRecord(rid RID)
        +updateRecord(rid RID, data Byte[])
        +readRecord(rid RID) Optional~Record~
    }

    class RecordLayoutManager {
        -Schema schema
        +getFieldOffset(colId ColId) Int
        +serialize(record Record) Byte[]
        +deserialize(data Byte[]) Record
    }

    class RIDGenerator {
        -PageId currentPageId
        -Int nextSlotId
        +generateNextRID(tableId TableId) RID
    }

    class IndexManager {
        -IBufferPool bufferPool
        -IMetadataCatalog catalog
        -Map~IndexId, IIndexStructure~ indexCache
        +createIndex(def IndexDef) IndexId
        +dropIndex(id IndexId)
        +getIndex(id IndexId) IIndexStructure
    }

    class SpaceManager {
        -ExtentManager extentMgr
        +allocateSpace(size Int) ExtentId
        +freeSpace(extent ExtentId)
        +availableSpace() Long
    }

    class ExtentManager {
        -Map~ExtentId, ExtentInfo~ extents
        -SegmentManager segMgr
        +allocateExtent(segId SegmentId) ExtentId
        +freeExtent(id ExtentId)
    }

    class SegmentManager {
        -Map~SegmentId, SegmentInfo~ segments
        +createSegment(tableId TableId) SegmentId
        +growSegment(id SegmentId)
        +dropSegment(id SegmentId)
    }

    RecordManager --> IBufferPool
    IndexManager --> IBufferPool
    BufferPoolManager --> DiskManager
    BufferPoolManager --> IReplacementPolicy
    PageManager --> DiskManager
    PageManager --> SpaceManager
    DiskManager --> TablespaceManager
    SpaceManager --> ExtentManager
    ExtentManager --> SegmentManager
    RecordManager --> RecordLayoutManager
    RecordManager --> RIDGenerator
    IndexManager *-- IIndexStructure
```

---

## Diagram 5 — WAL & Logging Layer

```mermaid
classDiagram

    class WALManager {
        -LogBuffer buffer
        -ILogWriter writer
        -LSNGenerator lsnGen
        -writeToBuffer(record LogRecord) LSN
        -shouldForceFlush() Boolean
        +append(record LogRecord) LSN
        +flush(upToLSN LSN)
        +truncateBefore(lsn LSN)
    }

    class LogWriter {
        -AbsolutePath logFilePath
        -Long currentOffset
        +write(bytes Byte[])
        +fsync()
        +currentOffset() Long
    }

    class LSNGenerator {
        -LSN currentLSN
        +getNextLSN() LSN
        +getCurrentLSN() LSN
    }

    WALManager --> ILogWriter
    WALManager --> LSNGenerator
    LogWriter ..|> ILogWriter
```

---

## Diagram 6 — Transaction Layer

```mermaid
classDiagram

    class TransactionManager {
        -TransactionTable txTable
        -LockManager lockMgr
        -IWALManager walMgr
        -MVCCManager mvccMgr
        +begin() TransactionId
        +commit(txId TransactionId)
        +abort(txId TransactionId)
    }
    note for TransactionManager "commit() forces flush WAL before return\nabort() calls UNDO log and releaseLock()"

    class TransactionTable {
        -Map~TransactionId, TxState~ activeTxns
        +addTx(txId TransactionId)
        +removeTx(txId TransactionId)
        +getState(txId TransactionId) TxState
        +getActiveIds() List~TransactionId~
    }

    class LockManager {
        -Map~ResourceId, LockQueue~ lockTable
        -DeadlockDetector detector
        -waitFor(txId TransactionId, resId ResourceId)
        +acquireLock(txId TransactionId, resId ResourceId, mode LockMode)
        +releaseLock(txId TransactionId, resId ResourceId)
        +releaseAll(txId TransactionId)
    }
    note for LockManager "acquireLock throw LockTimeoutException if wait for too long"

    class MVCCManager {
        -ISnapshotProvider snapshotProvider
        +readVersion(rid RID, snapshotId SnapshotId) Optional~Record~
        +writeVersion(rid RID, txId TransactionId, data Byte[])
        +vacuum(olderThan LSN)
    }

    class DeadlockDetector {
        -WaitForGraph graph
        -buildGraph(txTable TransactionTable) WaitForGraph
        -detectCycle(graph WaitForGraph) Optional~List~TransactionId~~
        -selectVictim(cycle List~TransactionId~) TransactionId
        +check(txTable TransactionTable)
    }
    note for DeadlockDetector "check() throw DeadlockException with victim TransactionId"

    TransactionManager --> TransactionTable
    TransactionManager --> LockManager
    TransactionManager --> IWALManager
    TransactionManager --> MVCCManager
    LockManager --> DeadlockDetector
    MVCCManager --> ISnapshotProvider
```

---

## Diagram 7 — Recovery & Backup Layer

```mermaid
classDiagram

    class RecoveryManager {
        -IWALManager walMgr
        -IBufferPool bufferPool
        -TransactionTable txTable
        -analyzePass(startLSN LSN) Map~TransactionId, LSN~
        -applyRedo(record LogRecord)
        -applyUndo(record LogRecord)
        +recover(checkpointLSN LSN)
    }
    note for RecoveryManager "recover() = analyzePass + redoPass + undoPass\nEach step is a separate method — SRP"

    class BackupManager {
        -AbsolutePath backupDir
        -IWALManager walMgr
        -DiskManager diskManager
        -snapshotFiles() List~AbsolutePath~
        -verifyChecksum(file AbsolutePath) Boolean
        +startFullBackup() BackupId
        +startIncrementalBackup(since LSN) BackupId
        +verifyBackup(id BackupId) Boolean
    }

    RecoveryManager --> IWALManager
    RecoveryManager --> IBufferPool
    BackupManager --> IWALManager
    BackupManager --> DiskManager
```

---

## Diagram 8 — Database Object Management Layer

```mermaid
classDiagram

    class IMetadataCatalog {
        <<interface>>
        +getTableMeta(name TableName) Optional~TableMeta~
        +getIndexMeta(id IndexId) Optional~IndexMeta~
        +updateMeta(obj CatalogObject)
        +deleteMeta(id ObjectId)
    }

    class MetadataCatalog {
        -Map~String, CatalogObject~ sysTables
        -IBufferPool bufferPool
        -loadFromStorage(id ObjectId) CatalogObject
        +getTableMeta(name TableName) Optional~TableMeta~
        +getIndexMeta(id IndexId) Optional~IndexMeta~
        +updateMeta(obj CatalogObject)
        +deleteMeta(id ObjectId)
    }

    class DatabaseManager {
        -IMetadataCatalog catalog
        -TablespaceManager tablespaceMgr
        +createDatabase(name DatabaseName)
        +dropDatabase(name DatabaseName)
        +getDatabase(name DatabaseName) Optional~DatabaseMeta~
    }

    class SchemaManager {
        -IMetadataCatalog catalog
        +createSchema(dbName DatabaseName, schemaName String)
        +dropSchema(dbName DatabaseName, schemaName String)
    }

    class TableManager {
        -IMetadataCatalog catalog
        -IndexManager indexMgr
        -SpaceManager spaceMgr
        +createTable(def TableDef)
        +alterTable(name TableName, patch TablePatch)
        +dropTable(name TableName)
    }

    MetadataCatalog ..|> IMetadataCatalog
    DatabaseManager --> IMetadataCatalog
    SchemaManager --> IMetadataCatalog
    TableManager --> IMetadataCatalog
    TableManager --> IndexManager
    TableManager --> SpaceManager
```

---

## Diagram 9 — Security Layer

```mermaid
classDiagram

    class IAuthorizationManager {
        <<interface>>
        +checkPermission(user UserId, obj ObjectId, action Action)
        +grantRole(user UserId, role RoleId)
        +revokeRole(user UserId, role RoleId)
    }

    class AuthenticationManager {
        -Map~String, HashedCredential~ userDb
        -hashPassword(raw String, salt String) HashedCredential
        +authenticate(username String, password String) Session
    }
    note for AuthenticationManager "authenticate throw PermissionDeniedException if credentials is not valid"

    class AuthorizationManager {
        -IMetadataCatalog catalog
        -resolveRoles(user UserId) Set~RoleId~
        +checkPermission(user UserId, obj ObjectId, action Action)
        +grantRole(user UserId, role RoleId)
        +revokeRole(user UserId, role RoleId)
    }

    AuthorizationManager ..|> IAuthorizationManager
    AuthorizationManager --> IMetadataCatalog
```

# Unit Test Specifications

## DiskManager
* `DiskManagerInitialize_NewDatabase_CreatesDataFiles`
* `DiskManagerReadPage_ValidPageId_ReturnsRawBytes`
* `DiskManagerReadPage_InvalidPageId_ThrowsFileNotFoundException`
* `DiskManagerWritePage_ValidPageId_WritesBytesCorrectly`

## TablespaceManager
* `TablespaceManagerAllocateTablespace_WithinQuota_ReturnsTablespaceId`
* `TablespaceManagerAllocateTablespace_ExceedsQuota_ThrowsQuotaExceededException`

## PageFormatter
* `PageFormatterFormat_NewRawPage_SetsHeaderCorrectly`
* `PageFormatterAddTuple_ToFormattedPage_UpdatesSlotArrayAndFreeSpace`
* `PageFormatterAddTuple_InsufficientSpace_ThrowsPageOverflowException`

## PageManager
* `PageManagerGetPage_FromCacheOrDisk_ReturnsFormattedPage`
* `PageManagerFlushDirtyPage_WritesToDisk_AndClearsDirtyFlag`

## BufferPoolManager
* `BufferPoolManagerFetchPage_NotInCache_LoadsFromDiskAndPins`
* `BufferPoolManagerFetchPage_InCache_IncrementsPinCount`
* `BufferPoolManagerUnpinPage_PinCountZero_LeavesPageUnpinned`
* `BufferPoolManagerUnpinPage_DirtyPage_MarksAsDirty`
* `BufferPoolManagerEvictPage_WhenCapacityFull_EvictsLeastRecentlyUsed`
* `BufferPoolManagerFlushAll_DirtiesOnlyWritesDirtyPages`

## LRUPolicy
* `LRUPolicySelectVictim_AfterMultipleAccesses_ReturnsLeastRecentlyUsed`

## ClockPolicy
* `ClockPolicySelectVictim_ReferenceBitHandling_EvictsCorrectPage`

## RecordManager
* `RecordManagerSerializeTuple_CorrectByteLayout`
* `RecordManagerDeserializeRecord_ValidBytes_ReturnsEquivalentTuple`

## RIDGenerator
* `RIDGeneratorGenerate_UniqueAcrossThreads_ReturnsUniqueRID`

## BPlusTree
* `BPlusTreeInsert_KeyCausesLeafSplit_CreatesNewRoot`
* `BPlusTreeSearch_ExistingKey_ReturnsCorrectLeafNode`
* `BPlusTreeSearch_NonExistingKey_ReturnsNull`
* `BPlusTreeDelete_KeyLeavesNodeUnderflow_PerformsMergeOrBorrow`

## HashIndex
* `HashIndexInsert_DuplicateKey_ThrowsDuplicateKeyException`
* `HashIndexLookup_ExistingKey_ReturnsRidList`
* `HashIndexDelete_ExistingKey_RemovesEntry`

## TableScan
* `TableScanInitialize_EmptyTable_ReturnsNoRows`
* `TableScanNext_IteratesAllRows_InOrder`

## IndexScan
* `IndexScanInitialize_WithBTreeIndex_ReturnsRowsInKeyOrder`

## SpaceManager
* `SpaceManagerAllocateExtent_WhenFreeSpaceAvailable_ReturnsExtentId`
* `SpaceManagerFreeExtent_ValidId_MarksAsFree`

## ExtentManager
* `ExtentManagerAllocateSegment_WithinLimits_ReturnsSegmentId`

## SegmentManager
* `SegmentManagerAllocatePage_InSegment_ReturnsPageId`

## WALManager
* `WALManagerAppendLogRecord_ValidRecord_AppendsToBuffer`
* `WALManagerAppendLogRecord_BufferFull_TriggersFlush`

## LogWriter
* `LogWriterFlush_EmptyBuffer_DoesNothing`
* `LogWriterFlush_NonEmptyBuffer_WritesToDiskAndUpdatesLSN`

## LSNGenerator
* `LSNGeneratorNext_IncrementsMonotonically_ReturnsHigherLSN`

## PageHeader
* `PageHeaderParseHeader_FromRawBytes_ReturnsCorrectValues`

## LRUCache
* `LRUCacheGet_AddsToCache_AndEvictsOldestWhenFull`

## TransactionManager
* `TransactionManagerBeginTransaction_ReturnsActiveContext`
* `TransactionManagerCommitTransaction_WithAllLocks_ReleasesLocksAndMarksCommitted`
* `TransactionManagerAbortTransaction_RollsBackChanges_AndReleasesLocks`
* `TransactionManagerCommitTransaction_WhenDeadlockDetected_ThrowsDeadlockException`

## TransactionTable
* `TransactionTableAddTransaction_NewEntry_AppearInActiveList`
* `TransactionTableRemoveTransaction_WhenCommittedOrAborted_NotPresentInActiveList`

## LockManager
* `LockManagerAcquireSharedLock_WhenNoConflict_GrantsImmediately`
* `LockManagerAcquireExclusiveLock_WhenSharedExists_WaitsInQueue`
* `LockManagerReleaseLock_TriggersNextWaitingTransaction`
* `LockManagerUpgradeLock_FromSharedToExclusive_IfNoOtherShared_Grants`

## MVCCManager
* `MVCCManagerCreateVersion_OnUpdate_GeneratesNewVersionChainNode`
* `MVCCManagerReadVisibleVersion_ForSnapshot_ReturnsCorrectVersion`
* `MVCCManagerCleanupObsoleteVersions_AfterCheckpoint_RemovesOldVersions`

## SnapshotManager
* `SnapshotManagerCreateSnapshot_IncludesAllActiveTxIds`
* `SnapshotManagerIsVisible_VisibleForCommittedTx_ReturnsTrue`
* `SnapshotManagerIsVisible_InvisibleForFutureTx_ReturnsFalse`

## DeadlockDetector
* `DeadlockDetectorDetectCycle_WithTwoTx_ReturnsTrueAndVictimChosen`
* `DeadlockDetectorResolveDeadlock_KillsVictimAndUnblocksOtherTx`

## TransactionContext
* `TransactionContextEquals_TwoContextsWithSameId_ReturnsTrue`

## LockQueue
* `LockQueueEnqueue_WhenLockBusy_AddsToQueueInOrder`
* `LockQueueDequeue_AfterRelease_RemovesFirstInQueue`

## SqlParser
* `SqlParserParse_ValidSelectStatement_ReturnsValidAst`
* `SqlParserParse_SelectWithJoin_ReturnsAstContainingJoinNode`
* `SqlParserParse_InvalidSyntax_ThrowsSqlSyntaxException`
* `SqlParserParse_LexicalTokens_CorrectlyIdentifiesKeywordsAndIdentifiers`

## QueryValidator
* `QueryValidatorValidate_ValidAst_AcceptsWithoutError`
* `QueryValidatorValidate_UnknownTable_ThrowsSqlException`
* `QueryValidatorValidate_ColumnTypeMismatch_ThrowsSqlException`

## QueryOptimizer
* `QueryOptimizerApplyRule_PredicatePushdown_MovesFilterBelowScan`
* `QueryOptimizerApplyRule_JoinReordering_ChoosesCheapestJoinOrder`
* `QueryOptimizerEstimateCost_IndexAvailable_ReturnsLowerCostThanSeqScan`
* `QueryOptimizerGeneratePhysicalPlan_FromLogicalPlan_ProducesExecutionPlan`

## ExecutionEngine
* `ExecutionEngineExecute_SequentialScan_ReturnsAllRows`
* `ExecutionEngineExecute_HashJoin_CorrectlyMatchesRows`
* `ExecutionEngineExecute_Projection_OutputsOnlySelectedColumns`
* `ExecutionEngineExecute_Aggregation_GroupByCorrectResult`
* `ExecutionEngineExecute_SortOperator_SortsRowsAccordingToOrderBy`

## ResultProcessor
* `ResultProcessorProcessResultSet_ToResultCursor_ReturnsCursorWithCorrectMetadata`
* `ResultProcessorFormatResult_WithJsonFormatter_ReturnsValidJson`

## ASTNode
* `ASTNodeEquals_SameStructure_ReturnsTrue`

## Token
* `TokenIsKeyword_ReturnsTrueForSelect`

## CostModel
* `CostModelCalculateCost_ForLogicalPlan_ReturnsExpectedValue`

## LogicalPlan
* `LogicalPlanValidate_NoCycles_InPlanGraph`

## ExecutionPlan
* `ExecutionPlanClone_DeepCopy_ReturnsIndependentCopy`

## DatabaseManager
* `DatabaseManagerCreateDatabase_ValidName_AddsToSystemCatalog`
* `DatabaseManagerDropDatabase_Existing_RemovesCatalogEntriesAndPhysicalFiles`
* `DatabaseManagerGetDatabaseMeta_ReturnsCorrectMetadata`

## SchemaManager
* `SchemaManagerCreateSchema_ValidDefinition_AddsToCatalog`
* `SchemaManagerResolveSchema_Existing_ReturnsSchemaObject`
* `SchemaManagerRenameSchema_UpdatesAllDependentObjects`

## TableManager
* `TableManagerCreateTable_WithColumns_AddsTableToCatalog`
* `TableManagerAlterTable_AddColumn_UpdatesTableMetaAndPhysicalStorage`
* `TableManagerDropTable_RemovesCatalogAndPhysicalPages`

## IndexManager
* `IndexManagerCreateIndex_OnColumn_AddsIndexMetaAndBuildsStructure`
* `IndexManagerDropIndex_RemovesMetaAndPhysicalStructure`

## MetadataCatalog
* `MetadataCatalogLookupTable_Existing_ReturnsTableMeta`
* `MetadataCatalogLookupColumn_Existing_ReturnsColumnMeta`

## ConstraintManager
* `ConstraintManagerAddPrimaryKey_ValidColumns_SetsUniqueConstraint`
* `ConstraintManagerAddForeignKey_ValidReference_EnforcesReferentialIntegrity`
* `ConstraintManagerValidateInsert_WithConstraintViolations_ThrowsConstraintException`

## CatalogObject
* `CatalogObjectEquals_TwoObjectsSameId_ReturnsTrue`

## ObjectId
* `ObjectIdGenerate_UniqueAcrossCatalog_ReturnsUniqueId`

## TableMeta
* `TableMetaClone_DeepCopy_ReturnsIndependentMeta`

## IndexMeta
* `IndexMetaUpdateStatistics_UpdatesCostEstimates`

## AuthenticationManager
* `AuthenticationManagerAuthenticate_ValidCredentials_ReturnsSession`
* `AuthenticationManagerAuthenticate_InvalidPassword_ThrowsAuthenticationException`
* `AuthenticationManagerHashPassword_ProducesDeterministicHash_WithSalt`

## AuthorizationManager
* `AuthorizationManagerAuthorize_UserHasPermission_ReturnsTrue`
* `AuthorizationManagerAuthorize_UserMissingPermission_ThrowsPermissionDeniedException`
* `AuthorizationManagerCheckRoleHierarchy_InheritedPermissions_AreEffectivePermissions`

## Session
* `SessionIsActive_AfterLogout_ReturnsFalse`

## RoleId / UserId
* `RoleId_UserIdEquals_SameGuid_ReturnsTrue`