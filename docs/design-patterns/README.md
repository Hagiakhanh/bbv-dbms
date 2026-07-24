# Features and Design Pattern Follow
This document outlines the Design Patterns implemented within various core components of the BBV-DBMS.

## Visual Summary Database Object

| Priority | Module | Main Feature | Main Classes | Application | Design Pattern | Progress |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
| 🔥 Critical | Database Object | Metadata Hierarchy | `Database`, `Schema`, `Table`, child objects | Treats Database, Schema, Table, and Column objects uniformly as nodes in a hierarchy. | **Composite** | Completed |
| 🔥 Critical | Database Object | Table Definition, Complex Table Construction | `TableDefinition`, `TableBuilder`, `TableDirector` | Separates the construction of complex Table objects from their representation. | **Builder** | Completed |
| 🔥 Critical | Database Object | Constraint Object Creation | `ConstraintFactory` | Encapsulates the instantiation logic for various types of constraints. | **Factory Method** | Completed |
| 🔥 Critical | Database Object | Constraint Validation | Constraint validators | Defines a family of validation algorithms for different constraints and makes them interchangeable. | **Strategy** | Completed |
| 🔥 Critical | Database Object | Index Object Creation | `IndexFactory` | Encapsulates the instantiation logic for different types of indexes. | **Factory Method** | Completed |
| 🔥 Critical | Database Object | Encapsulate DDL Requests | DDL commands and executor | Encapsulates DDL requests as objects, allowing for logging, queuing, and execution. | **Command** | Completed |
| 🔥 Critical | Database Object | Coordinate Create/Drop/Alter | `SchemaService`, `DatabaseService` | Provides a simplified, unified interface to the complex subsystems involved in metadata modifications. | **Facade** | Completed |
| 🔥 Critical | Database Object | Persist and Query Metadata | `CatalogManager`, catalog repositories | Abstracts the underlying data storage mechanism for database metadata. | **Repository** | Completed |
| 🔴 High | Database Object | Metadata Traversal | `CatalogIterator`, `IIterableCatalog` | Provides a way to sequentially access metadata objects without exposing their underlying representation. | **Iterator** | Completed |
| 🔴 High | Metadata Events | Cache, Statistics, Audit Reactions | Event publisher and handlers | Defines a one-to-many dependency so that when metadata changes, all dependent components are notified. | **Observer** | Completed |
| 🟡 Medium | Partition | Select Target Partition | Partition strategies | Defines interchangeable algorithms for selecting the appropriate partition for data. | **Strategy** | Not Started |
| 🟡 Medium | Trigger | Execute Trigger Actions | `TriggerExecutor`, trigger actions | Encapsulates trigger actions as objects for execution. | **Command** | Not Started |
| 🟡 Medium | Metadata Utility | Export DDL, Dependency Scan | Visitors or traversal services | Separates metadata analysis and export algorithms from the object structure on which they operate. | **Visitor** | Not Started |

---

```mermaid
classDiagram
direction LR

%% =====================================================
%% 1. EXTERNAL MODULE PORTS
%% Chỉ biểu diễn boundary, không bung Storage Engine,
%% Query Processor, Transaction hoặc Backup module.
%% =====================================================

class IDdlRequestSource {
    <<External Client Port>>
    +Submit(command : IDdlCommand) DdlResult
}

class IStorageObjectPort {
    <<External Storage Port>>
    +AllocateTable(table : Table)
    +DropTable(tableId : int)
    +AllocateIndex(index : Index)
    +DropIndex(indexId : int)
}

class IMetadataTransactionPort {
    <<External Transaction Port>>
    +Begin()
    +Commit()
    +Rollback()
}

class IBackupCatalogPort {
    <<External Backup Port>>
    +ExportMetadata(databaseId : int)
    +ImportMetadata(source : string)
}

%% =====================================================
%% 2. COMPOSITE HIERARCHY & DOMAIN OBJECTS
%% =====================================================

class ICatalogComponent {
    <<Component>>
    +Name : string
}

class ICatalogComposite {
    <<Composite>>
    +Children : IReadOnlyCollection~ICatalogComponent~
}

ICatalogComponent <|-- ICatalogComposite

class Database {
    <<Composite>>
    +DatabaseId : int
    +Name : string
    +Owner : string
    +Schemas : IReadOnlyCollection~Schema~
    +AddSchema(schema : Schema)
    +RemoveSchema(name : string)
    +GetSchema(name : string) Schema
    +CreateSchemaIterator() ICatalogIterator~Schema~
}

class Schema {
    <<Composite>>
    +SchemaId : int
    +Name : string
    +Parent : Database
    +Tables : IReadOnlyCollection~Table~
    +Views : IReadOnlyCollection~View~
    +Procedures : IReadOnlyCollection~StoredProcedure~
    +Sequences : IReadOnlyCollection~Sequence~
    +AddTable(table : Table)
    +RemoveTable(name : string)
    +GetTable(name : string) Table
    +CreateTableIterator() ICatalogIterator~Table~
}

class Table {
    <<Composite>>
    +TableId : int
    +Name : string
    +Parent : Schema
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~
    +AddColumn(column : Column)
    +RemoveColumn(name : string)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +CreateColumnIterator() ICatalogIterator~Column~
}

class Column {
    <<Leaf>>
    +ColumnId : int
    +Name : string
    +DataType : DataType
    +Nullable : bool
    +DefaultValue : object
    +Rename(newName : string)
    +ValidateValue(value : object) bool
}

class View {
    <<Leaf>>
    +ViewId : int
    +Name : string
    +QueryDefinition : string
}

class StoredProcedure {
    <<Leaf>>
    +Name : string
    +Parameters : IReadOnlyCollection~Column~
    +Body : string
}

class Sequence {
    <<Leaf>>
    +Name : string
    +CurrentValue : long
    +Increment : long
    +NextValue() long
}

class Partition {
    <<Leaf>>
    +Name : string
    +PartitionKey : string
    +PartitionType : PartitionType
}

class Trigger {
    <<Leaf>>
    +Name : string
    +Event : TriggerEvent
    +Timing : TriggerTiming
    +Body : string
}

class Row {
    +RowId : RID
    +Data : RecordData
    +Version : long
    +GetValue(columnId : int) object
    +SetValue(columnId : int, value : object)
}

class RID {
    <<Value Object>>
    +PageId : int
    +SlotNumber : int
    +Equals(other : RID) bool
}

class RecordData {
    <<Value Object>>
    +Bytes : Byte[]
    +Length : int
    +Serialize() Byte[]
    +Deserialize(bytes : Byte[])
}

class DataType {
    <<enumeration>>
    INT
    BIGINT
    VARCHAR
    BOOLEAN
    FLOAT
    DATETIME
}

class PartitionType {
    <<enumeration>>
    RANGE
    LIST
    HASH
}

class TriggerEvent {
    <<enumeration>>
    INSERT
    UPDATE
    DELETE
}

class TriggerTiming {
    <<enumeration>>
    BEFORE
    AFTER
    INSTEAD_OF
}

Database "1" *-- "*" Schema
Schema "1" *-- "*" Table
Schema "1" *-- "*" View
Schema "1" *-- "*" StoredProcedure
Schema "1" *-- "*" Sequence

Table "1" *-- "*" Column
Table "1" *-- "*" Constraint
Table "1" *-- "*" Index
Table "1" *-- "*" Partition
Table "1" *-- "*" Trigger
Table "1" *-- "*" Row

Row *-- RID
Row *-- RecordData
Column --> DataType
Partition --> PartitionType
Trigger --> TriggerEvent
Trigger --> TriggerTiming

ICatalogComposite <|.. Database
ICatalogComposite <|.. Schema
ICatalogComposite <|.. Table

ICatalogComponent <|.. Column
ICatalogComponent <|.. Constraint
ICatalogComponent <|.. Index
ICatalogComponent <|.. Partition
ICatalogComponent <|.. Trigger
ICatalogComponent <|.. View
ICatalogComponent <|.. StoredProcedure
ICatalogComponent <|.. Sequence

%% =====================================================
%% 3. CONSTRAINT VALIDATION — STRATEGY
%% =====================================================

class Constraint {
    <<Abstract Strategy>>
    +Name : string
    +Validate(context : ConstraintValidationContext) ConstraintValidationResult
}

class PrimaryKeyConstraint {
    <<Concrete Strategy>>
    +Columns : IReadOnlyCollection~Column~
}

class UniqueConstraint {
    <<Concrete Strategy>>
    +Columns : IReadOnlyCollection~Column~
}

class ForeignKeyConstraint {
    <<Concrete Strategy>>
    +ReferenceTable : Table
    +ReferenceColumns : IReadOnlyCollection~Column~
}

class CheckConstraint {
    <<Concrete Strategy>>
    +Expression : string
}

class ConstraintValidationContext {
    <<Context Data>>
    +Table : Table
    +Row : Row
    +Operation : ValidationOperation
}

class ConstraintValidationResult {
    <<Result>>
    +IsValid : bool
    +Message : string
}

class ValidationOperation {
    <<enumeration>>
    INSERT
    UPDATE
}

class IRowKeyExtractor {
    <<Domain Service>>
    +ExtractKey(row : Row, columns : IReadOnlyCollection~Column~) object
    +HasNullValue(row : Row, columns : IReadOnlyCollection~Column~) bool
}

Constraint <|-- PrimaryKeyConstraint
Constraint <|-- UniqueConstraint
Constraint <|-- ForeignKeyConstraint
Constraint <|-- CheckConstraint

Constraint --> ConstraintValidationContext
Constraint --> ConstraintValidationResult
ConstraintValidationContext --> ValidationOperation

PrimaryKeyConstraint --> IRowKeyExtractor
UniqueConstraint --> IRowKeyExtractor
ForeignKeyConstraint --> Table : references

%% =====================================================
%% 4. INDEX DOMAIN OBJECTS
%% =====================================================

class Index {
    <<abstract>>
    +IndexId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Unique : bool
    +Insert(key : object, rid : RID)
    +Delete(key : object)
    +Search(key : object) RID
}

class BTreeIndex
class HashIndex
class BitmapIndex

Index <|-- BTreeIndex
Index <|-- HashIndex
Index <|-- BitmapIndex

PrimaryKeyConstraint --> Index : uses
UniqueConstraint --> Index : uses

%% =====================================================
%% 5. CONSTRUCTION DATA
%% =====================================================

class TableDefinition {
    <<Construction Data>>
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~ConstraintOptions~
    +Indexes : IReadOnlyCollection~IndexOptions~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~
}

class ConstraintOptions {
    <<DTO>>
    +Type : ConstraintType
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +ReferenceTable : Table
    +ReferenceColumns : IReadOnlyCollection~Column~
    +Expression : string
}

class IndexOptions {
    <<DTO>>
    +Type : IndexType
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Unique : bool
}

class ConstraintType {
    <<enumeration>>
    PRIMARY_KEY
    UNIQUE
    FOREIGN_KEY
    CHECK
}

class IndexType {
    <<enumeration>>
    BTREE
    HASH
    BITMAP
}

TableDefinition --> Column
TableDefinition --> ConstraintOptions
TableDefinition --> IndexOptions
TableDefinition --> Partition
TableDefinition --> Trigger

ConstraintOptions --> ConstraintType
IndexOptions --> IndexType

%% =====================================================
%% 6. TABLE CONSTRUCTION — BUILDER
%% =====================================================

class ITableBuilder {
    <<Builder>>
    +Reset(name : string)
    +AddColumn(column : Column)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
    +Build() Table
}

class TableBuilder {
    <<Concrete Builder>>
    -currentTable : Table
}

class TableDirector {
    <<Director>>
    +Construct(definition : TableDefinition) Table
}

ITableBuilder <|.. TableBuilder

TableDirector --> ITableBuilder : directs
TableDirector --> IConstraintFactory : creates constraints
TableDirector --> IIndexFactory : creates indexes
TableDirector --> TableDefinition : reads

TableBuilder --> Table : builds

%% =====================================================
%% 7. DYNAMIC CREATION — FACTORIES
%% =====================================================

class IConstraintFactory {
    <<Factory>>
    +Create(options : ConstraintOptions) Constraint
}

class ConstraintFactory {
    <<Concrete Factory>>
}

class IIndexFactory {
    <<Factory>>
    +Create(options : IndexOptions) Index
}

class IndexFactory {
    <<Concrete Factory>>
}

IConstraintFactory <|.. ConstraintFactory
IIndexFactory <|.. IndexFactory

IConstraintFactory --> ConstraintOptions
IConstraintFactory --> Constraint : creates

IIndexFactory --> IndexOptions
IIndexFactory --> Index : creates

%% =====================================================
%% 8. APPLICATION FACADE
%% Chỉ giữ operation nhóm quan trọng.
%% Chi tiết đầy đủ nằm trong Facade Pattern Diagram.
%% =====================================================

class IDatabaseService {
    <<Facade Interface>>
    +CreateSchema(database : Database, name : string) Schema
    +DropSchema(database : Database, name : string, cascade : bool)
    +RenameSchema(database : Database, oldName : string, newName : string)
}

class DatabaseService {
    <<Facade>>
    -catalog : ICatalogManager
    -metadataTransaction : IMetadataTransactionPort
    -eventDispatcher : MetadataEventCommitDispatcher
}

class ISchemaService {
    <<Facade Interface>>
    +CreateTable(schema : Schema, definition : TableDefinition) Table
    +DropTable(schema : Schema, name : string, cascade : bool)
    +AlterTable(table : Table, operation : TableAlterOperation)
    +CreateView(schema : Schema, definition : ViewDefinition) View
    +CreateProcedure(schema : Schema, definition : ProcedureDefinition) StoredProcedure
    +CreateSequence(schema : Schema, definition : SequenceDefinition) Sequence
}

class SchemaService {
    <<Facade>>
    -catalog : ICatalogManager
    -tableDirector : TableDirector
    -storage : IStorageObjectPort
    -metadataTransaction : IMetadataTransactionPort
    -eventDispatcher : MetadataEventCommitDispatcher
}

class TableAlterOperation {
    <<Command Data>>
    +Type : TableAlterType
    +Payload : object
}

class TableAlterType {
    <<enumeration>>
    ADD_COLUMN
    DROP_COLUMN
    ADD_CONSTRAINT
    DROP_CONSTRAINT
    ADD_INDEX
    DROP_INDEX
    RENAME
}

class ViewDefinition {
    <<DTO>>
    +Name : string
    +QueryDefinition : string
}

class ProcedureDefinition {
    <<DTO>>
    +Name : string
    +Parameters : IReadOnlyCollection~Column~
    +Body : string
}

class SequenceDefinition {
    <<DTO>>
    +Name : string
    +StartValue : long
    +Increment : long
}

IDatabaseService <|.. DatabaseService
ISchemaService <|.. SchemaService

DatabaseService --> ICatalogManager
DatabaseService --> IMetadataTransactionPort
DatabaseService --> MetadataChangeContext : supplies event context
DatabaseService --> Database : manages

SchemaService --> ICatalogManager
SchemaService --> TableDirector
SchemaService --> IStorageObjectPort
SchemaService --> IMetadataTransactionPort
SchemaService --> MetadataChangeContext : supplies event context
SchemaService --> Schema : manages
SchemaService --> TableAlterOperation
SchemaService --> ViewDefinition
SchemaService --> ProcedureDefinition
SchemaService --> SequenceDefinition

TableAlterOperation --> TableAlterType

%% =====================================================
%% 9. DDL EXECUTION — COMMAND
%% =====================================================

class IDdlCommand {
    <<Command>>
    +Execute() DdlResult
}

class CreateSchemaCommand {
    <<Concrete Command>>
    -receiver : IDatabaseService
    -database : Database
    -schemaName : string
}

class CreateTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -schema : Schema
    -definition : TableDefinition
}

class AlterTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -table : Table
    -operation : TableAlterOperation
}

class DropTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -schema : Schema
    -tableName : string
    -cascade : bool
}

class IDdlCommandExecutor {
    <<Invoker Interface>>
    +Execute(command : IDdlCommand) DdlResult
}

class DdlCommandExecutor {
    <<Invoker>>
}

class DdlResult {
    <<Result>>
    +Success : bool
    +Message : string
    +AffectedObject : ICatalogComponent
}

IDdlCommand <|.. CreateSchemaCommand
IDdlCommand <|.. CreateTableCommand
IDdlCommand <|.. AlterTableCommand
IDdlCommand <|.. DropTableCommand

IDdlCommandExecutor <|.. DdlCommandExecutor
IDdlCommandExecutor --> IDdlCommand : executes
IDdlCommand --> DdlResult : returns

CreateSchemaCommand --> IDatabaseService : receiver
CreateSchemaCommand --> Database : target

CreateTableCommand --> ISchemaService : receiver
CreateTableCommand --> Schema : target
CreateTableCommand --> TableDefinition : carries

AlterTableCommand --> ISchemaService : receiver
AlterTableCommand --> Table : target
AlterTableCommand --> TableAlterOperation : carries

DropTableCommand --> ISchemaService : receiver
DropTableCommand --> Schema : target

IDdlRequestSource --> IDdlCommandExecutor : submits

%% =====================================================
%% 10. CATALOG COORDINATION
%% CatalogManager persists metadata and records events.
%% Observers are notified only after a successful commit
%% through MetadataEventCommitDispatcher.
%% =====================================================

class ICatalogManager {
    <<Metadata Coordinator>>
    +Register(component : ICatalogComponent, context : MetadataChangeContext)
    +Update(component : ICatalogComponent, context : MetadataChangeContext)
    +Remove(componentId : int, objectType : CatalogObjectType, context : MetadataChangeContext)
    +GetDatabase(name : string) Database
    +GetSchema(databaseId : int, name : string) Schema
    +GetTable(schemaId : int, name : string) Table
    +ObjectExists(parentId : int, parentType : CatalogObjectType, name : string, objectType : CatalogObjectType) bool
}

class CatalogManager {
    <<Metadata Coordinator>>
    -databaseRepository : IDatabaseCatalogRepository
    -schemaRepository : ISchemaCatalogRepository
    -tableRepository : ITableCatalogRepository
    -eventCollector : IMetadataEventCollector
    +Register(component : ICatalogComponent, context : MetadataChangeContext)
    +Update(component : ICatalogComponent, context : MetadataChangeContext)
    +Remove(componentId : int, objectType : CatalogObjectType, context : MetadataChangeContext)
    +GetDatabase(name : string) Database
    +GetSchema(databaseId : int, name : string) Schema
    +GetTable(schemaId : int, name : string) Table
    +ObjectExists(parentId : int, parentType : CatalogObjectType, name : string, objectType : CatalogObjectType) bool
}

class CatalogObjectType {
    <<enumeration>>
    DATABASE
    SCHEMA
    TABLE
    COLUMN
    CONSTRAINT
    INDEX
    PARTITION
    TRIGGER
    VIEW
    PROCEDURE
    SEQUENCE
}

ICatalogManager <|.. CatalogManager
ICatalogManager --> CatalogObjectType
CatalogManager --> IMetadataEventCollector : records changes

%% =====================================================
%% 11. CATALOG REPOSITORIES — REPOSITORY PATTERN
%% Concrete repository không lặp lại toàn bộ method.
%% =====================================================

class IDatabaseCatalogRepository {
    <<Repository>>
    +Add(database : Database)
    +Update(database : Database)
    +Remove(databaseId : int)
    +FindById(databaseId : int) Database
    +FindByName(name : string) Database
    +GetAll() IReadOnlyCollection~Database~
    +Exists(name : string) bool
}

class ISchemaCatalogRepository {
    <<Repository>>
    +Add(schema : Schema)
    +Update(schema : Schema)
    +Remove(schemaId : int)
    +FindById(schemaId : int) Schema
    +FindByName(databaseId : int, name : string) Schema
    +GetByDatabase(databaseId : int) IReadOnlyCollection~Schema~
    +Exists(databaseId : int, name : string) bool
}

class ITableCatalogRepository {
    <<Repository>>
    +Add(table : Table)
    +Update(table : Table)
    +Remove(tableId : int)
    +FindById(tableId : int) Table
    +FindByName(schemaId : int, name : string) Table
    +GetBySchema(schemaId : int) IReadOnlyCollection~Table~
    +Exists(schemaId : int, name : string) bool
}

class DatabaseCatalogRepository {
    <<Concrete Repository>>
}

class SchemaCatalogRepository {
    <<Concrete Repository>>
}

class TableCatalogRepository {
    <<Concrete Repository>>
}

IDatabaseCatalogRepository <|.. DatabaseCatalogRepository
ISchemaCatalogRepository <|.. SchemaCatalogRepository
ITableCatalogRepository <|.. TableCatalogRepository

CatalogManager --> IDatabaseCatalogRepository
CatalogManager --> ISchemaCatalogRepository
CatalogManager --> ITableCatalogRepository

DatabaseCatalogRepository --> Database : maps metadata
SchemaCatalogRepository --> Schema : maps metadata
TableCatalogRepository --> Table : maps metadata

%% =====================================================
%% 12. METADATA EVENTS — OBSERVER PATTERN
%% Cache, statistics and audit react to committed metadata
%% changes without coupling CatalogManager to each subsystem.
%% =====================================================

class MetadataChangeContext {
    <<Event Context>>
    +Actor : string
    +SessionId : string
    +TransactionId : string
    +Timestamp : DateTime
}

class MetadataEvent {
    <<Event>>
    +EventId : Guid
    +EventType : MetadataEventType
    +ObjectId : int
    +ObjectType : CatalogObjectType
    +ObjectName : string
    +ParentId : int
    +Context : MetadataChangeContext
    +PreviousSnapshot : MetadataSnapshot
    +CurrentSnapshot : MetadataSnapshot
}

class MetadataSnapshot {
    <<Event Data>>
    +Properties : IReadOnlyDictionary~string, object~
}

class MetadataEventType {
    <<enumeration>>
    CREATED
    UPDATED
    RENAMED
    REMOVED
}

class IMetadataEventCollector {
    <<Transactional Event Collector>>
    +Add(event : MetadataEvent)
    +GetPendingEvents() IReadOnlyCollection~MetadataEvent~
    +Clear()
}

class MetadataEventCollector {
    <<Concrete Event Collector>>
    -pendingEvents : List~MetadataEvent~
    +Add(event : MetadataEvent)
    +GetPendingEvents() IReadOnlyCollection~MetadataEvent~
    +Clear()
}

class IMetadataEventPublisher {
    <<Subject>>
    +Subscribe(observer : IMetadataObserver)
    +Unsubscribe(observer : IMetadataObserver)
    +Publish(event : MetadataEvent)
}

class MetadataEventPublisher {
    <<Concrete Subject>>
    -observers : List~IMetadataObserver~
    +Subscribe(observer : IMetadataObserver)
    +Unsubscribe(observer : IMetadataObserver)
    +Publish(event : MetadataEvent)
}

class IMetadataObserver {
    <<Observer>>
    +OnMetadataChanged(event : MetadataEvent)
}

class MetadataEventCommitDispatcher {
    <<Commit Dispatcher>>
    -collector : IMetadataEventCollector
    -publisher : IMetadataEventPublisher
    +DispatchCommittedEvents()
    +DiscardRolledBackEvents()
}

class CatalogCacheObserver {
    <<Concrete Observer>>
    -cache : ICatalogCache
    +OnMetadataChanged(event : MetadataEvent)
}

class MetadataStatisticsObserver {
    <<Concrete Observer>>
    -statisticsStore : IMetadataStatisticsStore
    +OnMetadataChanged(event : MetadataEvent)
}

class MetadataAuditObserver {
    <<Concrete Observer>>
    -auditRepository : IMetadataAuditRepository
    +OnMetadataChanged(event : MetadataEvent)
}

class ICatalogCache {
    <<Cache Port>>
    +Get(objectType : CatalogObjectType, objectId : int) ICatalogComponent
    +Set(component : ICatalogComponent)
    +Remove(objectType : CatalogObjectType, objectId : int)
    +InvalidateChildren(parentId : int)
}

class IMetadataStatisticsStore {
    <<Statistics Port>>
    +IncrementObjectCount(objectType : CatalogObjectType)
    +DecrementObjectCount(objectType : CatalogObjectType)
    +RecordModification(objectType : CatalogObjectType, timestamp : DateTime)
}

class IMetadataAuditRepository {
    <<Audit Repository>>
    +Add(entry : MetadataAuditEntry)
}

class MetadataAuditEntry {
    <<Audit Record>>
    +AuditId : Guid
    +EventType : MetadataEventType
    +ObjectType : CatalogObjectType
    +ObjectId : int
    +ObjectName : string
    +Actor : string
    +TransactionId : string
    +Timestamp : DateTime
    +PreviousValues : MetadataSnapshot
    +CurrentValues : MetadataSnapshot
}

IMetadataEventCollector <|.. MetadataEventCollector
IMetadataEventPublisher <|.. MetadataEventPublisher

IMetadataObserver <|.. CatalogCacheObserver
IMetadataObserver <|.. MetadataStatisticsObserver
IMetadataObserver <|.. MetadataAuditObserver

MetadataEventCollector o-- "*" MetadataEvent : pending events

MetadataEventCommitDispatcher --> IMetadataEventCollector : reads or clears
MetadataEventCommitDispatcher --> IMetadataEventPublisher : dispatches after commit

MetadataEventPublisher o-- "*" IMetadataObserver : notifies
MetadataEventPublisher --> MetadataEvent : publishes
IMetadataObserver --> MetadataEvent : receives

CatalogCacheObserver --> ICatalogCache : updates or invalidates
MetadataStatisticsObserver --> IMetadataStatisticsStore : updates counters
MetadataAuditObserver --> IMetadataAuditRepository : writes audit
MetadataAuditObserver --> MetadataAuditEntry : creates

MetadataEvent --> MetadataEventType
MetadataEvent --> CatalogObjectType
MetadataEvent --> MetadataChangeContext
MetadataEvent --> MetadataSnapshot

MetadataAuditEntry --> MetadataEventType
MetadataAuditEntry --> CatalogObjectType
MetadataAuditEntry --> MetadataSnapshot

ICatalogCache --> ICatalogComponent : caches

DatabaseService --> MetadataEventCommitDispatcher : dispatches after commit
SchemaService --> MetadataEventCommitDispatcher : dispatches after commit

%% =====================================================
%% 13. METADATA TRAVERSAL — GENERIC ITERATOR
%% =====================================================

class ICatalogIterator~T~ {
    <<Iterator>>
    +MoveNext() bool
    +Current : T
}

class CatalogIterator~T~ {
    <<Concrete Iterator>>
    -items : IReadOnlyList~T~
    -position : int
}

class CatalogTraversalService {
    <<Iterator Client>>
    +TraverseDatabase(database : Database)
    +TraverseSchema(schema : Schema)
    +TraverseTable(table : Table)
}

ICatalogIterator~T~ <|.. CatalogIterator~T~

Database --> ICatalogIterator~Schema~ : creates
Schema --> ICatalogIterator~Table~ : creates
Table --> ICatalogIterator~Column~ : creates

CatalogTraversalService --> Database
CatalogTraversalService --> Schema
CatalogTraversalService --> Table

%% =====================================================
%% 14. OPTIONAL EXTERNAL INTEGRATIONS
%% =====================================================

IBackupCatalogPort --> Database : exports metadata
```

## Visual Summary Database Server & Database Lifecycle

| Priority | Module | Main Feature | Main Classes | Application | Design Pattern | Progress |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
| 🔥 Critical | Server Management | Server Lifecycle | `DatabaseServer` | Provides a unified interface for starting, stopping, restarting, and recovering the database server. | **Facade** | Completed |
| 🔥 Critical | Server Management | Server State Management | `DatabaseServer`, `IServerState` | Encapsulates behaviors for Stopped, Running, Recovering, and Failed states. | **State** | Not Started |
| 🔥 Critical | Database Management | Database Lifecycle | `DatabaseManager` | Coordinates catalog and connection pool operations for creating, opening, closing, and dropping databases. | **Facade** | Not Started |
| 🔥 Critical | Security | Authentication | `SecurityManager`, `IAuthenticationStrategy` | Supports password, token, certificate, and external authentication mechanisms. | **Strategy** | Not Started |
| 🔥 Critical | Security | Authorization | `SecurityManager`, `IAuthorizationStrategy` | Supports RBAC, ACL, and policy-based permission checking. | **Strategy** | Not Started |
| 🔴 High | Database Management | Database Creation | `IDatabaseFactory`, `DatabaseFactory` | Centralizes the construction and initialization of database objects. | **Factory Method** | Not Started |
| 🔴 High | Database Management | Database State | `Database`, `IDatabaseState` | Controls database behavior in Online, Offline, ReadOnly, and Restoring states. | **State** | Not Started |
| 🔴 High | Configuration | Configuration Loading | `ConfigurationManager`, `IConfigurationLoader` | Supports loading configuration from JSON, XML, environment variables, or command-line sources. | **Strategy** | Not Started |
| 🔴 High | Security | Protected Database Access | `SecuredDatabaseProxy` | Validates permissions before forwarding operations to database objects. | **Proxy** | Not Started |
| 🔴 High | Monitoring | Metrics Collection | `MonitoringManager`, `IMetricCollector` | Separates CPU, memory, query, transaction, and connection metric collection. | **Strategy** | Not Started |
| 🔴 High | Monitoring | Runtime Event Monitoring | `MonitoringManager`, event publishers | Receives query, transaction, connection, and error events from server components. | **Observer** | Not Started |
| 🟡 Medium | Server Management | Administrative Operations | `StartServerCommand`, `StopServerCommand`, `RecoverServerCommand` | Encapsulates server operations for auditing, scheduling, and retrying. | **Command** | Not Started |
| 🟡 Medium | Configuration | Dynamic Configuration | `ConfigurationManager`, configuration observers | Notifies dependent components when configuration values change. | **Observer** | Not Started |
| 🟢 Low | Monitoring | Metrics Export | `PrometheusMetricsAdapter`, `OpenTelemetryAdapter` | Converts internal server metrics into external monitoring formats. | **Adapter** | Not Started |




## Sequence Diagrams (Database Manager & Metadata)

### 1. Hierarchy Management (Composite Pattern)

**Purpose:**  
Represent metadata as a tree so parent and child objects are managed uniformly.

**Application:**  
`Database → Schema → Table → Column`

#### Class Diagram

```mermaid
classDiagram
direction TB

class ICatalogComponent{
    <<Component>>
    +Name : string
}

class Database{
    <<Composite>>
    +AddSchema(schema)
    +RemoveSchema(name)
    +GetSchemas()
}

class Schema{
    <<Composite>>
    +AddTable(table)
    +RemoveTable(name)
    +GetTables()
}

class Table{
    <<Composite>>
    +AddColumn(column)
    +RemoveColumn(name)
    +GetColumns()
}

class Column{
    <<Leaf>>
}

ICatalogComponent <|.. Database
ICatalogComponent <|.. Schema
ICatalogComponent <|.. Table
ICatalogComponent <|.. Column

Database *-- Schema
Schema *-- Table
Table *-- Column
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant DB as Database
    participant Schema
    participant Table

    Client->>DB: AddSchema(schema)
    Client->>Schema: AddTable(table)
    Client->>DB: GetSchemas()
    DB-->>Client: List<Schema>
```

#### Simplified Code

```csharp
public interface ICatalogComponent
{
    string Name { get; }
}

public class Database : ICatalogComponent
{
    public string Name { get; init; }

    // Add a child Schema
    public void AddSchema(Schema schema) { }

    // Remove a child Schema
    public void RemoveSchema(string name) { }

    // Return all child Schemas
    public IReadOnlyCollection<Schema> GetSchemas() => [];
}

public class Schema : ICatalogComponent
{
    public string Name { get; init; }

    // Add a child Table
    public void AddTable(Table table) { }

    // Return all child Tables
    public IReadOnlyCollection<Table> GetTables() => [];
}

public class Table : ICatalogComponent
{
    public string Name { get; init; }

    // Add a child Column
    public void AddColumn(Column column) { }

    // Return all child Columns
    public IReadOnlyCollection<Column> GetColumns() => [];
}

public class Column : ICatalogComponent
{
    public string Name { get; init; }
}
```

**Benefits**

- Models metadata as a tree structure.
- Parent and child objects are managed consistently.
- Easy to traverse the metadata hierarchy.

**Application:** Models the metadata tree: Database → Schema → Table → Column.

**Why apply?** Composite Pattern structures data into a tree form, providing uniform Add/Remove functions. The diagram below shows assigning objects together to form a parent-child structure, making it easy to access the entire branch (e.g., `GetSchemas()`, `GetTables()`).

```mermaid
classDiagram
direction TB

%% =====================================================
%% Composite Root
%% =====================================================

class Database {
    <<Composite>>
    +DatabaseId : int
    +Name : string
    +Owner : string
    +Schemas : IReadOnlyCollection~Schema~

    +Database(id : int, name : string, owner : string)

    +AddSchema(schema : Schema)
    +RemoveSchema(name : string) bool
    +GetSchema(name : string) Schema
    +GetSchemas() IReadOnlyCollection~Schema~
    +ContainsSchema(name : string) bool
    +Rename(newName : string)
}

%% =====================================================
%% Schema Composite
%% =====================================================

class Schema {
    <<Composite>>
    +SchemaId : int
    +Name : string
    +Parent : Database

    +Tables : IReadOnlyCollection~Table~
    +Views : IReadOnlyCollection~View~
    +Procedures : IReadOnlyCollection~StoredProcedure~
    +Sequences : IReadOnlyCollection~Sequence~

    +Schema(id : int, name : string)

    +AddTable(table : Table)
    +RemoveTable(name : string) bool
    +GetTable(name : string) Table
    +GetTables() IReadOnlyCollection~Table~
    +ContainsTable(name : string) bool

    +AddView(view : View)
    +RemoveView(name : string) bool
    +GetView(name : string) View
    +GetViews() IReadOnlyCollection~View~

    +AddProcedure(procedure : StoredProcedure)
    +RemoveProcedure(name : string) bool
    +GetProcedure(name : string) StoredProcedure
    +GetProcedures() IReadOnlyCollection~StoredProcedure~

    +AddSequence(sequence : Sequence)
    +RemoveSequence(name : string) bool
    +GetSequence(name : string) Sequence
    +GetSequences() IReadOnlyCollection~Sequence~

    +Rename(newName : string)
    +AttachTo(database : Database)
    +Detach()
}

%% =====================================================
%% Table Composite
%% =====================================================

class Table {
    <<Composite>>
    +TableId : int
    +Name : string
    +Parent : Schema

    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~

    +Table(id : int, name : string)

    +AddColumn(column : Column)
    +RemoveColumn(name : string) bool
    +GetColumn(name : string) Column
    +GetColumns() IReadOnlyCollection~Column~
    +ContainsColumn(name : string) bool

    +AddConstraint(constraint : Constraint)
    +RemoveConstraint(name : string) bool
    +GetConstraint(name : string) Constraint
    +GetConstraints() IReadOnlyCollection~Constraint~

    +AddIndex(index : Index)
    +RemoveIndex(name : string) bool
    +GetIndex(name : string) Index
    +GetIndexes() IReadOnlyCollection~Index~

    +AddPartition(partition : Partition)
    +RemovePartition(name : string) bool
    +GetPartition(name : string) Partition
    +GetPartitions() IReadOnlyCollection~Partition~

    +AddTrigger(trigger : Trigger)
    +RemoveTrigger(name : string) bool
    +GetTrigger(name : string) Trigger
    +GetTriggers() IReadOnlyCollection~Trigger~

    +Rename(newName : string)
    +AttachTo(schema : Schema)
    +Detach()
}

%% =====================================================
%% Table Leaves
%% =====================================================

class Column {
    <<Leaf>>
    +ColumnId : int
    +Name : string
    +Parent : Table
    +DataType : DataType
    +Nullable : bool
    +DefaultValue : object
    +Length : int
    +Precision : int
    +Scale : int

    +Column(id : int, name : string, dataType : DataType)

    +Rename(newName : string)
    +SetDataType(dataType : DataType)
    +SetNullable(nullable : bool)
    +SetDefaultValue(value : object)
    +ValidateValue(value : object) bool
    +AttachTo(table : Table)
    +Detach()
}

class Constraint {
    <<abstract Leaf>>
    +ConstraintId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~

    +Validate(context : ConstraintValidationContext) ConstraintValidationResult
    +Rename(newName : string)
}

class Index {
    <<abstract Leaf>>
    +IndexId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Unique : bool

    +Insert(key : object, rid : RID)
    +Delete(key : object)
    +Search(key : object) RID
    +Rename(newName : string)
}

class Partition {
    <<Leaf>>
    +PartitionId : int
    +Name : string
    +PartitionKey : string
    +PartitionType : PartitionType

    +Rename(newName : string)
    +Contains(key : object) bool
}

class Trigger {
    <<Leaf>>
    +TriggerId : int
    +Name : string
    +Event : TriggerEvent
    +Timing : TriggerTiming
    +Body : string
    +Enabled : bool

    +Rename(newName : string)
    +Enable()
    +Disable()
}

%% =====================================================
%% Schema Leaves
%% =====================================================

class View {
    <<Leaf>>
    +ViewId : int
    +Name : string
    +QueryDefinition : string

    +View(id : int, name : string, queryDefinition : string)

    +Rename(newName : string)
    +UpdateDefinition(queryDefinition : string)
}

class StoredProcedure {
    <<Leaf>>
    +ProcedureId : int
    +Name : string
    +Parameters : IReadOnlyCollection~ProcedureParameter~
    +Body : string

    +StoredProcedure(id : int, name : string, body : string)

    +Rename(newName : string)
    +AddParameter(parameter : ProcedureParameter)
    +RemoveParameter(name : string) bool
    +UpdateBody(body : string)
}

class Sequence {
    <<Leaf>>
    +SequenceId : int
    +Name : string
    +CurrentValue : long
    +StartValue : long
    +Increment : long
    +MinimumValue : long
    +MaximumValue : long
    +Cycle : bool

    +Sequence(id : int, name : string, startValue : long, increment : long)

    +NextValue() long
    +Restart(value : long)
    +Rename(newName : string)
}

class ProcedureParameter {
    +Name : string
    +DataType : DataType
    +Direction : ParameterDirection
    +DefaultValue : object
}

%% =====================================================
%% Supporting Types
%% =====================================================

class DataType {
    <<enumeration>>
    INT
    BIGINT
    VARCHAR
    BOOLEAN
    FLOAT
    DECIMAL
    DATETIME
    BINARY
}

class PartitionType {
    <<enumeration>>
    RANGE
    LIST
    HASH
}

class TriggerEvent {
    <<enumeration>>
    INSERT
    UPDATE
    DELETE
}

class TriggerTiming {
    <<enumeration>>
    BEFORE
    AFTER
    INSTEAD_OF
}

class ParameterDirection {
    <<enumeration>>
    INPUT
    OUTPUT
    INPUT_OUTPUT
}

class RID {
    <<value object>>
    +PageId : int
    +SlotNumber : int
}

class ConstraintValidationContext {
    +Table : Table
    +Row : Row
    +Operation : ValidationOperation
}

class ConstraintValidationResult {
    +IsValid : bool
    +Message : string
}

class Row {
    +RowId : RID
    +Values : IReadOnlyDictionary~int, object~
    +GetValue(columnId : int) object
}

class ValidationOperation {
    <<enumeration>>
    INSERT
    UPDATE
}

%% =====================================================
%% Composite Relationships
%% =====================================================

Database "1" *-- "*" Schema : contains

Schema "1" *-- "*" Table : contains
Schema "1" *-- "*" View : contains
Schema "1" *-- "*" StoredProcedure : contains
Schema "1" *-- "*" Sequence : contains

Table "1" *-- "*" Column : contains
Table "1" *-- "*" Constraint : contains
Table "1" *-- "*" Index : contains
Table "1" *-- "*" Partition : contains
Table "1" *-- "*" Trigger : contains

StoredProcedure "1" *-- "*" ProcedureParameter : parameters

Column --> DataType
ProcedureParameter --> DataType
ProcedureParameter --> ParameterDirection
Partition --> PartitionType
Trigger --> TriggerEvent
Trigger --> TriggerTiming

Constraint --> ConstraintValidationContext
Constraint --> ConstraintValidationResult
ConstraintValidationContext --> Row
ConstraintValidationContext --> ValidationOperation
Row *-- RID
```

```mermaid
sequenceDiagram
    actor Engine
    participant DB as Database
    participant Schema as Schema
    participant Table as Table
    participant Column as Column

    Engine->>DB: AddSchema(schema)
    DB->>Schema: SetParent(DB)

    Engine->>Schema: AddTable(table)
    Schema->>Table: SetParent(Schema)

    Engine->>Table: AddColumn(column)
    Table->>Column: SetParent(Table)

    Engine->>DB: GetSchemas()
    DB-->>Engine: List<Schema>
    
    Engine->>Schema: GetTables()
    Schema-->>Engine: List<Table>
```

### 2. Metadata Initialization (Builder Pattern)

**Purpose:**
Construct a complex object step by step instead of using a long constructor.

**Example:**
Build a `Computer` with optional CPU, RAM, storage, and graphics card.

#### Class Diagram

```mermaid
classDiagram
direction LR

class IComputerBuilder {
    <<Builder>>
    +SetCpu(cpu : string)
    +SetRam(ram : int)
    +SetStorage(storage : int)
    +SetGraphicsCard(card : string)
    +Build() Computer
}

class ComputerBuilder {
    <<Concrete Builder>>
    -computer : Computer
}

class Computer {
    <<Product>>
    +Cpu : string
    +Ram : int
    +Storage : int
    +GraphicsCard : string
}

class ComputerDirector {
    <<Director>>
    +BuildGamingComputer() Computer
}

IComputerBuilder <|.. ComputerBuilder
ComputerBuilder --> Computer : builds
ComputerDirector --> IComputerBuilder : directs
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant Director as ComputerDirector
    participant Builder as IComputerBuilder

    Client->>Director: BuildGamingComputer()
    Director->>Builder: SetCpu("Intel Core i7")
    Director->>Builder: SetRam(32)
    Director->>Builder: SetStorage(1000)
    Director->>Builder: SetGraphicsCard("RTX 4070")
    Director->>Builder: Build()
    Builder-->>Director: Computer
    Director-->>Client: Computer
```

#### Simplified Code

```csharp
public class Computer
{
    public string Cpu { get; set; } = string.Empty;
    public int Ram { get; set; }
    public int Storage { get; set; }
    public string GraphicsCard { get; set; } = string.Empty;
}

public interface IComputerBuilder
{
    // Configure the computer processor
    void SetCpu(string cpu);

    // Configure RAM capacity in GB
    void SetRam(int ram);

    // Configure storage capacity in GB
    void SetStorage(int storage);

    // Configure an optional graphics card
    void SetGraphicsCard(string graphicsCard);

    // Return the completed Computer object
    Computer Build();
}

public class ComputerBuilder : IComputerBuilder
{
    private Computer _computer = new();

    public void SetCpu(string cpu)
    {
        // Assign the selected CPU
    }

    public void SetRam(int ram)
    {
        // Assign RAM capacity
    }

    public void SetStorage(int storage)
    {
        // Assign storage capacity
    }

    public void SetGraphicsCard(string graphicsCard)
    {
        // Assign an optional graphics card
    }

    public Computer Build()
    {
        // Validate and return the completed computer
        return _computer;
    }
}

public class ComputerDirector
{
    private readonly IComputerBuilder _builder;

    public ComputerDirector(IComputerBuilder builder)
    {
        _builder = builder;
    }

    public Computer BuildGamingComputer()
    {
        // Define the standard steps for building a gaming computer
        _builder.SetCpu("Intel Core i7");
        _builder.SetRam(32);
        _builder.SetStorage(1000);
        _builder.SetGraphicsCard("RTX 4070");

        return _builder.Build();
    }
}
```

**Benefits**

* Avoids constructors with too many parameters.
* Supports optional object properties.
* Reuses the same construction process for different object configurations.
* Separates object construction from its representation.

**Application:** Initializes tables via `TableBuilder` from DDL syntax.

**Why apply?** Initializing a Table object requires many properties. `TableBuilder` helps gather parameters gradually (Columns, Primary Keys) and only creates the `TableMetadata` object in the final step, making the code coherent and readable.

```mermaid
classDiagram
direction LR

%% =====================================================
%% Builder Pattern — Table Construction
%% =====================================================

class ITableBuilder {
    <<Builder>>
    +Reset()
    +SetName(name : string)
    +AddColumn(column : Column)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
    +Build() Table
}

class TableBuilder {
    <<Concrete Builder>>
    -currentTable : Table
    -hasName : bool

    +Reset()
    +SetName(name : string)
    +AddColumn(column : Column)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
    +Build() Table

    -EnsureInitialized()
    -ValidateBeforeBuild()
}

class TableDirector {
    <<Director>>
    -builder : ITableBuilder
    -constraintFactory : IConstraintFactory
    -indexFactory : IIndexFactory
    -partitionFactory : IPartitionFactory
    -triggerFactory : ITriggerFactory

    +TableDirector(
        builder : ITableBuilder,
        constraintFactory : IConstraintFactory,
        indexFactory : IIndexFactory,
        partitionFactory : IPartitionFactory,
        triggerFactory : ITriggerFactory
    )

    +Construct(definition : TableDefinition) Table
}

%% =====================================================
%% Construction Data
%% =====================================================

class TableDefinition {
    <<Construction Data>>
    +Name : string
    +Columns : IReadOnlyCollection~ColumnDefinition~
    +Constraints : IReadOnlyCollection~ConstraintOptions~
    +Indexes : IReadOnlyCollection~IndexOptions~
    +Partitions : IReadOnlyCollection~PartitionOptions~
    +Triggers : IReadOnlyCollection~TriggerOptions~

    +Validate() DefinitionValidationResult
}

class ColumnDefinition {
    <<DTO>>
    +Name : string
    +DataType : DataType
    +Nullable : bool
    +DefaultValue : object
}

class ConstraintOptions {
    <<DTO>>
    +Type : ConstraintType
    +Name : string
    +Columns : IReadOnlyCollection~string~
    +ReferenceTable : string
    +ReferenceColumns : IReadOnlyCollection~string~
    +Expression : string
}

class IndexOptions {
    <<DTO>>
    +Type : IndexType
    +Name : string
    +Columns : IReadOnlyCollection~string~
    +Unique : bool
}

class PartitionOptions {
    <<DTO>>
    +Name : string
    +Type : PartitionType
    +PartitionKey : string
    +BoundaryValues : IReadOnlyCollection~object~
}

class TriggerOptions {
    <<DTO>>
    +Name : string
    +Event : TriggerEvent
    +Timing : TriggerTiming
    +Body : string
}

class DefinitionValidationResult {
    <<Result>>
    +IsValid : bool
    +Errors : IReadOnlyCollection~string~
}

%% =====================================================
%% Product
%% =====================================================

class Table {
    <<Product>>
    +TableId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~

    +AddColumn(column : Column)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
}

class Column {
    +ColumnId : int
    +Name : string
    +DataType : DataType
    +Nullable : bool
    +DefaultValue : object
}

class Constraint {
    <<abstract>>
    +Name : string
}

class Index {
    <<abstract>>
    +IndexId : int
    +Name : string
    +Unique : bool
}

class Partition {
    +PartitionId : int
    +Name : string
    +PartitionType : PartitionType
    +PartitionKey : string
}

class Trigger {
    +TriggerId : int
    +Name : string
    +Event : TriggerEvent
    +Timing : TriggerTiming
    +Body : string
}

%% =====================================================
%% Factories Used by Director
%% =====================================================

class IColumnFactory {
    <<Factory>>
    +Create(definition : ColumnDefinition) Column
}

class IConstraintFactory {
    <<Factory>>
    +Create(
        options : ConstraintOptions,
        tableContext : TableBuildContext
    ) Constraint
}

class IIndexFactory {
    <<Factory>>
    +Create(
        options : IndexOptions,
        tableContext : TableBuildContext
    ) Index
}

class IPartitionFactory {
    <<Factory>>
    +Create(options : PartitionOptions) Partition
}

class ITriggerFactory {
    <<Factory>>
    +Create(options : TriggerOptions) Trigger
}

class TableBuildContext {
    <<Build Context>>
    +TableName : string
    +Columns : IReadOnlyCollection~Column~
    +FindColumn(name : string) Column
}

%% =====================================================
%% Supporting Types
%% =====================================================

class DataType {
    <<enumeration>>
    INT
    BIGINT
    VARCHAR
    BOOLEAN
    FLOAT
    DECIMAL
    DATETIME
}

class ConstraintType {
    <<enumeration>>
    PRIMARY_KEY
    UNIQUE
    FOREIGN_KEY
    CHECK
}

class IndexType {
    <<enumeration>>
    BTREE
    HASH
    BITMAP
}

class PartitionType {
    <<enumeration>>
    RANGE
    LIST
    HASH
}

class TriggerEvent {
    <<enumeration>>
    INSERT
    UPDATE
    DELETE
}

class TriggerTiming {
    <<enumeration>>
    BEFORE
    AFTER
    INSTEAD_OF
}

%% =====================================================
%% Relationships
%% =====================================================

ITableBuilder <|.. TableBuilder

TableDirector --> ITableBuilder : directs
TableDirector --> IColumnFactory : creates columns
TableDirector --> IConstraintFactory : creates constraints
TableDirector --> IIndexFactory : creates indexes
TableDirector --> IPartitionFactory : creates partitions
TableDirector --> ITriggerFactory : creates triggers
TableDirector --> TableDefinition : reads
TableDirector --> TableBuildContext : maintains context

TableBuilder --> Table : builds

TableDefinition --> ColumnDefinition
TableDefinition --> ConstraintOptions
TableDefinition --> IndexOptions
TableDefinition --> PartitionOptions
TableDefinition --> TriggerOptions
TableDefinition --> DefinitionValidationResult

ColumnDefinition --> DataType
ConstraintOptions --> ConstraintType
IndexOptions --> IndexType
PartitionOptions --> PartitionType
TriggerOptions --> TriggerEvent
TriggerOptions --> TriggerTiming

IColumnFactory --> Column : creates
IConstraintFactory --> Constraint : creates
IIndexFactory --> Index : creates
IPartitionFactory --> Partition : creates
ITriggerFactory --> Trigger : creates

TableBuildContext --> Column

Table "1" *-- "*" Column
Table "1" *-- "*" Constraint
Table "1" *-- "*" Index
Table "1" *-- "*" Partition
Table "1" *-- "*" Trigger
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Service as ISchemaService
    participant Definition as TableDefinition
    participant Director as TableDirector
    participant Builder as ITableBuilder
    participant ColFactory as IColumnFactory
    participant CFactory as IConstraintFactory
    participant IFactory as IIndexFactory
    participant PFactory as IPartitionFactory
    participant TFactory as ITriggerFactory
    participant Schema
    participant Table

    Client->>Service: CreateTable(schema, definition)

    Service->>Definition: Validate()
    Definition-->>Service: DefinitionValidationResult

    alt Definition is invalid
        Service-->>Client: throw InvalidTableDefinitionException
    else Definition is valid
        Service->>Director: Construct(definition)
        activate Director

        Director->>Builder: Reset()
        Director->>Builder: SetName(definition.Name)

        loop Each ColumnDefinition
            Director->>ColFactory: Create(columnDefinition)
            ColFactory-->>Director: Column
            Director->>Builder: AddColumn(column)
        end

        Note over Director: Column objects must be created first<br/>because constraints and indexes reference columns.

        Director->>Director: Create TableBuildContext(columns)

        loop Each ConstraintOptions
            Director->>CFactory: Create(options, buildContext)
            CFactory-->>Director: Constraint
            Director->>Builder: AddConstraint(constraint)
        end

        loop Each IndexOptions
            Director->>IFactory: Create(options, buildContext)
            IFactory-->>Director: Index
            Director->>Builder: AddIndex(index)
        end

        loop Each PartitionOptions
            Director->>PFactory: Create(options)
            PFactory-->>Director: Partition
            Director->>Builder: AddPartition(partition)
        end

        loop Each TriggerOptions
            Director->>TFactory: Create(options)
            TFactory-->>Director: Trigger
            Director->>Builder: AddTrigger(trigger)
        end

        Director->>Builder: Build()
        Builder->>Builder: ValidateBeforeBuild()

        alt Builder state is invalid
            Builder-->>Director: throw TableBuildException
            Director-->>Service: propagate exception
            Service-->>Client: table creation failed
        else Builder state is valid
            Builder-->>Director: Table
            Director-->>Service: Table
        end

        deactivate Director

        Service->>Schema: AddTable(table)

        alt Duplicate table name
            Schema-->>Service: throw DuplicateTableException
            Service-->>Client: table creation failed
        else Table added
            Schema-->>Service: Success
            Service-->>Client: Table
        end
    end
```

### 3. Constraint Validation (Strategy Pattern)

**Purpose:**
Encapsulate interchangeable algorithms behind a common interface.

**Example:**
A payment system supports multiple payment methods such as Credit Card, PayPal, and Bank Transfer.

#### Class Diagram

```mermaid
classDiagram
direction LR

class IPaymentStrategy {
    <<Strategy>>
    +Pay(amount : decimal)
}

class CreditCardPayment {
    <<Concrete Strategy>>
}

class PayPalPayment {
    <<Concrete Strategy>>
}

class BankTransferPayment {
    <<Concrete Strategy>>
}

class PaymentService {
    <<Context>>
    -strategy : IPaymentStrategy
    +SetStrategy(strategy)
    +Checkout(amount)
}

IPaymentStrategy <|.. CreditCardPayment
IPaymentStrategy <|.. PayPalPayment
IPaymentStrategy <|.. BankTransferPayment

PaymentService --> IPaymentStrategy : uses
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Customer
    participant Service as PaymentService
    participant Strategy as PayPalPayment

    Customer->>Service: SetStrategy(PayPal)
    Customer->>Service: Checkout(100)
    Service->>Strategy: Pay(100)
    Strategy-->>Service: Success
    Service-->>Customer: Payment Completed
```

#### Simplified Code

```csharp
public interface IPaymentStrategy
{
    // Execute the payment using a specific payment method
    void Pay(decimal amount);
}

public class CreditCardPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        // Process payment using a credit card
    }
}

public class PayPalPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        // Process payment using PayPal
    }
}

public class BankTransferPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        // Process payment using a bank transfer
    }
}

public class PaymentService
{
    private IPaymentStrategy _strategy;

    // Change the payment algorithm at runtime
    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy;
    }

    // Delegate payment processing to the selected strategy
    public void Checkout(decimal amount)
    {
        _strategy.Pay(amount);
    }
}
```

**Benefits**

* Easily switch algorithms at runtime.
* Follows the Open/Closed Principle.
* Eliminates large `if-else` or `switch` statements.
* Each algorithm can evolve independently.

**Application:** Evaluates Row validity based on various types of Constraints.

**Why apply?** By applying the Strategy Pattern via the `IConstraint` interface, `RecordManager` doesn't need to care about internal detailed logic (Primary Key checks for duplicates, Check evaluates expressions, Foreign Key checks reference table). It just calls `Validate(row)` and handles the polymorphic result.

```mermaid
classDiagram
direction TB

%% =====================================================
%% Context
%% =====================================================

class RecordManager {
    <<Context>>
    +Insert(table : Table, row : Row) RID
    +Update(table : Table, rid : RID, row : Row)
    -ValidateConstraints(
        table : Table,
        row : Row,
        operation : ValidationOperation,
        currentRid : RID
    ) ConstraintValidationResult
}

class Table {
    +TableId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +GetConstraints() IReadOnlyCollection~Constraint~
}

%% =====================================================
%% Strategy
%% =====================================================

class Constraint {
    <<abstract Strategy>>
    +ConstraintId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Validate(
        context : ConstraintValidationContext
    ) ConstraintValidationResult
}

class PrimaryKeyConstraint {
    <<Concrete Strategy>>
    -keyLookup : IKeyLookup
    -keyExtractor : IRowKeyExtractor
    +Validate(
        context : ConstraintValidationContext
    ) ConstraintValidationResult
}

class UniqueConstraint {
    <<Concrete Strategy>>
    -keyLookup : IKeyLookup
    -keyExtractor : IRowKeyExtractor
    +Validate(
        context : ConstraintValidationContext
    ) ConstraintValidationResult
}

class ForeignKeyConstraint {
    <<Concrete Strategy>>
    +ReferenceTable : Table
    +ReferenceColumns : IReadOnlyCollection~Column~
    -keyLookup : IKeyLookup
    -keyExtractor : IRowKeyExtractor
    +Validate(
        context : ConstraintValidationContext
    ) ConstraintValidationResult
}

class CheckConstraint {
    <<Concrete Strategy>>
    +Expression : string
    -expressionEvaluator : IExpressionEvaluator
    +Validate(
        context : ConstraintValidationContext
    ) ConstraintValidationResult
}

%% =====================================================
%% Validation Context and Result
%% =====================================================

class ConstraintValidationContext {
    <<Context Data>>
    +Table : Table
    +Row : Row
    +Operation : ValidationOperation
    +CurrentRid : RID
}

class ConstraintValidationResult {
    <<Result>>
    +IsValid : bool
    +ConstraintName : string
    +Message : string
    +Success() ConstraintValidationResult
    +Failure(
        constraintName : string,
        message : string
    ) ConstraintValidationResult
}

class ValidationOperation {
    <<enumeration>>
    INSERT
    UPDATE
}

%% =====================================================
%% Collaborators
%% =====================================================

class IRowKeyExtractor {
    <<interface>>
    +ExtractKey(
        row : Row,
        columns : IReadOnlyCollection~Column~
    ) CompositeKey

    +HasNullValue(
        row : Row,
        columns : IReadOnlyCollection~Column~
    ) bool
}

class IKeyLookup {
    <<interface>>
    +Exists(
        table : Table,
        columns : IReadOnlyCollection~Column~,
        key : CompositeKey,
        excludedRid : RID
    ) bool
}

class IExpressionEvaluator {
    <<interface>>
    +Evaluate(
        expression : string,
        row : Row
    ) bool
}

class CompositeKey {
    <<Value Object>>
    +Values : IReadOnlyList~object~
    +Equals(other : CompositeKey) bool
    +GetHashCode() int
}

%% =====================================================
%% Supporting Domain Types
%% =====================================================

class Row {
    +RowId : RID
    +GetValue(columnId : int) object
}

class Column {
    +ColumnId : int
    +Name : string
    +Nullable : bool
}

class RID {
    <<Value Object>>
    +PageId : int
    +SlotNumber : int
}

%% =====================================================
%% Relationships
%% =====================================================

RecordManager --> Table : reads constraints
RecordManager --> ConstraintValidationContext : creates
RecordManager --> ConstraintValidationResult : handles

Table "1" *-- "*" Constraint : holds strategies

Constraint <|-- PrimaryKeyConstraint
Constraint <|-- UniqueConstraint
Constraint <|-- ForeignKeyConstraint
Constraint <|-- CheckConstraint

Constraint --> ConstraintValidationContext
Constraint --> ConstraintValidationResult
Constraint --> Column

ConstraintValidationContext --> Table
ConstraintValidationContext --> Row
ConstraintValidationContext --> RID
ConstraintValidationContext --> ValidationOperation

PrimaryKeyConstraint --> IRowKeyExtractor
PrimaryKeyConstraint --> IKeyLookup

UniqueConstraint --> IRowKeyExtractor
UniqueConstraint --> IKeyLookup

ForeignKeyConstraint --> IRowKeyExtractor
ForeignKeyConstraint --> IKeyLookup
ForeignKeyConstraint --> Table : references
ForeignKeyConstraint --> Column : reference columns

CheckConstraint --> IExpressionEvaluator

IRowKeyExtractor --> CompositeKey : creates
IKeyLookup --> CompositeKey : searches
```

```mermaid
sequenceDiagram
    autonumber

    participant Client
    participant RecordMgr as RecordManager
    participant Table
    participant Constraint as Constraint Strategy
    participant Extractor as IRowKeyExtractor
    participant Lookup as IKeyLookup
    participant Evaluator as IExpressionEvaluator

    Client->>RecordMgr: Insert(table, row)

    RecordMgr->>Table: GetConstraints()
    Table-->>RecordMgr: constraints

    loop Each Constraint
        RecordMgr->>Constraint: Validate(context)

        alt Primary Key
            Constraint->>Extractor: HasNullValue(row, columns)

            alt Contains NULL
                Extractor-->>Constraint: true
                Constraint-->>RecordMgr: Failure("Primary key cannot be null")
            else No NULL
                Extractor-->>Constraint: false
                Constraint->>Extractor: ExtractKey(row, columns)
                Extractor-->>Constraint: CompositeKey

                Constraint->>Lookup: Exists(table, columns, key, currentRid)
                Lookup-->>Constraint: exists

                alt Key exists
                    Constraint-->>RecordMgr: Failure("Duplicate primary key")
                else Key does not exist
                    Constraint-->>RecordMgr: Success
                end
            end

        else Unique Constraint
            Constraint->>Extractor: HasNullValue(row, columns)

            alt Contains NULL
                Extractor-->>Constraint: true
                Constraint-->>RecordMgr: Success
            else No NULL
                Extractor-->>Constraint: false
                Constraint->>Extractor: ExtractKey(row, columns)
                Extractor-->>Constraint: CompositeKey
                Constraint->>Lookup: Exists(table, columns, key, currentRid)
                Lookup-->>Constraint: exists

                alt Key exists
                    Constraint-->>RecordMgr: Failure("Duplicate unique value")
                else Key does not exist
                    Constraint-->>RecordMgr: Success
                end
            end

        else Foreign Key
            Constraint->>Extractor: ExtractKey(row, localColumns)
            Extractor-->>Constraint: CompositeKey
            Constraint->>Lookup: Exists(referenceTable, referenceColumns, key, null)
            Lookup-->>Constraint: exists

            alt Referenced key missing
                Constraint-->>RecordMgr: Failure("Foreign key not found")
            else Referenced key exists
                Constraint-->>RecordMgr: Success
            end

        else Check Constraint
            Constraint->>Evaluator: Evaluate(expression, row)
            Evaluator-->>Constraint: result

            alt Expression is false
                Constraint-->>RecordMgr: Failure("Check constraint failed")
            else Expression is valid
                Constraint-->>RecordMgr: Success
            end
        end

        alt Validation failed
            RecordMgr-->>Client: throw ConstraintViolationException
        end
    end

    RecordMgr->>RecordMgr: Persist row
    RecordMgr-->>Client: RID
```

### 4. Dynamic Allocation (Factory Method Pattern)

**Purpose:**
Define a common method for creating objects while allowing subclasses to decide which concrete object is created.

**Example:**
A notification system creates different notification types such as Email, SMS, and Push Notification.

#### Class Diagram

```mermaid
classDiagram
direction LR

class Notification {
    <<Product Interface>>
    +Send(message : string)
}

class EmailNotification
class SmsNotification
class PushNotification

class NotificationCreator {
    <<Creator>>
    +CreateNotification() Notification
    +Notify(message : string)
}

class EmailNotificationCreator {
    <<Concrete Creator>>
}

class SmsNotificationCreator {
    <<Concrete Creator>>
}

class PushNotificationCreator {
    <<Concrete Creator>>
}

Notification <|.. EmailNotification
Notification <|.. SmsNotification
Notification <|.. PushNotification

NotificationCreator <|-- EmailNotificationCreator
NotificationCreator <|-- SmsNotificationCreator
NotificationCreator <|-- PushNotificationCreator

NotificationCreator --> Notification : creates
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant Creator as EmailNotificationCreator
    participant Product as EmailNotification

    Client->>Creator: Notify("Order completed")
    Creator->>Creator: CreateNotification()
    Creator-->>Creator: EmailNotification
    Creator->>Product: Send("Order completed")
    Product-->>Creator: Success
    Creator-->>Client: Completed
```

#### Simplified Code

```csharp
public interface INotification
{
    // Send a notification message
    void Send(string message);
}

public class EmailNotification : INotification
{
    public void Send(string message)
    {
        // Send the message by email
    }
}

public class SmsNotification : INotification
{
    public void Send(string message)
    {
        // Send the message by SMS
    }
}

public class PushNotification : INotification
{
    public void Send(string message)
    {
        // Send the message as a push notification
    }
}

public abstract class NotificationCreator
{
    // Factory Method: subclasses decide which notification is created
    protected abstract INotification CreateNotification();

    public void Notify(string message)
    {
        // Create the product without depending on a concrete class
        INotification notification = CreateNotification();

        // Use the created product
        notification.Send(message);
    }
}

public class EmailNotificationCreator : NotificationCreator
{
    protected override INotification CreateNotification()
    {
        // Create an email notification
        return new EmailNotification();
    }
}

public class SmsNotificationCreator : NotificationCreator
{
    protected override INotification CreateNotification()
    {
        // Create an SMS notification
        return new SmsNotification();
    }
}

public class PushNotificationCreator : NotificationCreator
{
    protected override INotification CreateNotification()
    {
        // Create a push notification
        return new PushNotification();
    }
}
```

**Benefits**

* Removes direct dependency on concrete product classes.
* Makes adding new product types easier.
* Follows the Open/Closed Principle.
* Keeps object creation logic separate from business logic.

**Application:** Allocates objects like Index and Constraint automatically during DDL execution.

**Why apply?** Delegates the creation of a specific Index (BTree or Hash) to `IndexFactory`. The client doesn't need to know the internal initialization logic, just passes in the desired Index type and receives a common `IIndex` interface back.

```mermaid
classDiagram
direction TB

%% =====================================================
%% Factory Clients
%% =====================================================

class TableDirector {
    <<Client>>
    -constraintFactory : IConstraintFactory
    -indexFactory : IIndexFactory
    +Construct(definition : TableDefinition) Table
}

%% =====================================================
%% Constraint Factory
%% =====================================================

class IConstraintFactory {
    <<Factory>>
    +Create(options : ConstraintOptions) Constraint
}

class ConstraintFactory {
    <<Concrete Factory>>
    +Create(options : ConstraintOptions) Constraint
}

class ConstraintOptions {
    <<DTO>>
    +Type : ConstraintType
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +ReferenceTable : Table
    +ReferenceColumns : IReadOnlyCollection~Column~
    +Expression : string
}

class ConstraintType {
    <<enumeration>>
    PRIMARY_KEY
    UNIQUE
    FOREIGN_KEY
    CHECK
}

class Constraint {
    <<abstract Product>>
    +Name : string
    +Columns : IReadOnlyCollection~Column~
}

class PrimaryKeyConstraint {
    <<Concrete Product>>
}

class UniqueConstraint {
    <<Concrete Product>>
}

class ForeignKeyConstraint {
    <<Concrete Product>>
    +ReferenceTable : Table
    +ReferenceColumns : IReadOnlyCollection~Column~
}

class CheckConstraint {
    <<Concrete Product>>
    +Expression : string
}

%% =====================================================
%% Index Factory
%% =====================================================

class IIndexFactory {
    <<Factory>>
    +Create(options : IndexOptions) Index
}

class IndexFactory {
    <<Concrete Factory>>
    +Create(options : IndexOptions) Index
}

class IndexOptions {
    <<DTO>>
    +Type : IndexType
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Unique : bool
}

class IndexType {
    <<enumeration>>
    BTREE
    HASH
    BITMAP
}

class Index {
    <<abstract Product>>
    +IndexId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Unique : bool
}

class BTreeIndex {
    <<Concrete Product>>
}

class HashIndex {
    <<Concrete Product>>
}

class BitmapIndex {
    <<Concrete Product>>
}

%% =====================================================
%% Supporting Domain Classes
%% =====================================================

class TableDefinition {
    +Constraints : IReadOnlyCollection~ConstraintOptions~
    +Indexes : IReadOnlyCollection~IndexOptions~
}

class Table
class Column

%% =====================================================
%% Relationships
%% =====================================================

TableDirector --> IConstraintFactory : creates constraints
TableDirector --> IIndexFactory : creates indexes
TableDirector --> TableDefinition : reads

IConstraintFactory <|.. ConstraintFactory
IIndexFactory <|.. IndexFactory

IConstraintFactory --> ConstraintOptions
IConstraintFactory --> Constraint : creates

IIndexFactory --> IndexOptions
IIndexFactory --> Index : creates

ConstraintOptions --> ConstraintType
ConstraintOptions --> Column
ConstraintOptions --> Table : referenced table

IndexOptions --> IndexType
IndexOptions --> Column

Constraint <|-- PrimaryKeyConstraint
Constraint <|-- UniqueConstraint
Constraint <|-- ForeignKeyConstraint
Constraint <|-- CheckConstraint

Index <|-- BTreeIndex
Index <|-- HashIndex
Index <|-- BitmapIndex
```

```mermaid
sequenceDiagram
    autonumber

    participant Director as TableDirector
    participant CFactory as IConstraintFactory
    participant IFactory as IIndexFactory
    participant Builder as ITableBuilder

    loop Each ConstraintOptions
        Director->>CFactory: Create(options)
        alt PRIMARY_KEY
            CFactory->>CFactory: Validate primary key options
            CFactory-->>Director: PrimaryKeyConstraint
        else UNIQUE
            CFactory->>CFactory: Validate unique options
            CFactory-->>Director: UniqueConstraint
        else FOREIGN_KEY
            CFactory->>CFactory: Validate foreign key options
            CFactory-->>Director: ForeignKeyConstraint
        else CHECK
            CFactory->>CFactory: Validate check options
            CFactory-->>Director: CheckConstraint
        else Unsupported type
            CFactory-->>Director: throw UnsupportedConstraintTypeException
        end
        Director->>Builder: AddConstraint(constraint)
    end
    loop Each IndexOptions
        Director->>IFactory: Create(options)
        alt BTREE
            IFactory-->>Director: BTreeIndex
        else HASH
            IFactory-->>Director: HashIndex
        else BITMAP
            IFactory-->>Director: BitmapIndex
        else Unsupported type
            IFactory-->>Director: throw UnsupportedIndexTypeException
        end
        Director->>Builder: AddIndex(index)
    end
```

### 5. Hierarchy Traversal (Iterator Pattern)

**Purpose:**
Traverse elements in a collection without exposing its internal structure.

**Example:**
A playlist allows the client to browse songs one by one without directly accessing the underlying list.

#### Class Diagram

```mermaid
classDiagram
direction LR

class IIterator~T~ {
    <<Iterator>>
    +HasNext() bool
    +Next() T
}

class PlaylistIterator {
    <<Concrete Iterator>>
    -songs : IReadOnlyList~Song~
    -position : int
}

class IPlaylist {
    <<Aggregate>>
    +CreateIterator() IIterator~Song~
}

class Playlist {
    <<Concrete Aggregate>>
    -songs : List~Song~
    +AddSong(song : Song)
}

class Song {
    +Title : string
}

IIterator~T~ <|.. PlaylistIterator
IPlaylist <|.. Playlist

Playlist --> Song : contains
Playlist --> PlaylistIterator : creates
PlaylistIterator --> Song : traverses
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant Playlist
    participant Iterator as PlaylistIterator

    Client->>Playlist: CreateIterator()
    Playlist-->>Client: Iterator

    loop While HasNext()
        Client->>Iterator: HasNext()
        Iterator-->>Client: true
        Client->>Iterator: Next()
        Iterator-->>Client: Song
    end
```

#### Simplified Code

```csharp
public class Song
{
    public string Title { get; init; } = string.Empty;
}

public interface IIterator<T>
{
    // Check whether another item is available
    bool HasNext();

    // Return the next item in the collection
    T Next();
}

public interface IPlaylist
{
    // Create an iterator for traversing songs
    IIterator<Song> CreateIterator();
}

public class Playlist : IPlaylist
{
    private readonly List<Song> _songs = [];

    public void AddSong(Song song)
    {
        // Add a song to the playlist
    }

    public IIterator<Song> CreateIterator()
    {
        // Create an iterator without exposing the internal list
        return new PlaylistIterator(_songs);
    }
}

public class PlaylistIterator : IIterator<Song>
{
    private readonly IReadOnlyList<Song> _songs;
    private int _position;

    public PlaylistIterator(IReadOnlyList<Song> songs)
    {
        _songs = songs;
    }

    public bool HasNext()
    {
        // Check whether the current position is valid
        return false;
    }

    public Song Next()
    {
        // Return the current song and move to the next position
        return default!;
    }
}
```

**Benefits**

* Hides the internal collection structure.
* Provides a consistent traversal mechanism.
* Supports multiple traversal strategies.
* Separates traversal logic from collection logic.

**Application:** Traverses the database metadata hierarchy (Database, Schema, Table) sequentially without exposing the underlying representations.

**Why apply?** The Iterator Pattern provides a unified interface `ICatalogIterator` for clients (like `SchemaService`) to iterate through catalog components (Schemas in a Database, Tables in a Schema, Columns in a Table) regardless of whether they are stored in an `IReadOnlyList` or `IReadOnlyCollection`.

```mermaid
classDiagram
direction TB

class ICatalogComponent {
    <<Component>>
    +Name : string
}

class ICatalogComposite {
    <<Composite>>
    +Children : IReadOnlyCollection~ICatalogComponent~
}

class IIterableCatalog {
    <<Iterable Collection>>
    +CreateIterator() ICatalogIterator
}

class ICatalogIterator {
    <<Iterator>>
    +Current : ICatalogComponent
    +MoveNext() bool
    +Reset()
}

class CatalogIterator {
    <<Concrete Iterator>>
    -components : IReadOnlyList~ICatalogComponent~
    -position : int
    +CatalogIterator(components : IReadOnlyList~ICatalogComponent~)
    +Current : ICatalogComponent
    +MoveNext() bool
    +Reset()
}

class Database {
    <<Concrete Collection>>
    +Name : string
    +Schemas : IReadOnlyCollection~Schema~
    +CreateIterator() ICatalogIterator
}

class Schema {
    <<Concrete Collection>>
    +Name : string
    +Tables : IReadOnlyCollection~Table~
    +CreateIterator() ICatalogIterator
}

class Table {
    <<Concrete Collection>>
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +CreateIterator() ICatalogIterator
}

class Column {
    <<Leaf>>
    +Name : string
}

class CatalogTraversalService {
    <<Client>>
    +Traverse(collection : IIterableCatalog)
}

ICatalogComponent <|-- ICatalogComposite

ICatalogComposite <|.. Database
ICatalogComposite <|.. Schema
ICatalogComposite <|.. Table
ICatalogComponent <|.. Column

IIterableCatalog <|.. Database
IIterableCatalog <|.. Schema
IIterableCatalog <|.. Table

ICatalogIterator <|.. CatalogIterator

Database "1" *-- "*" Schema
Schema "1" *-- "*" Table
Table "1" *-- "*" Column

IIterableCatalog --> ICatalogIterator : creates
CatalogIterator --> ICatalogComponent : returns
CatalogTraversalService --> IIterableCatalog : requests iterator
CatalogTraversalService --> ICatalogIterator : traverses
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Service as CatalogTraversalService
    participant Database
    participant Iterator as ICatalogIterator
    participant Component as ICatalogComponent

    Client->>Service: TraverseChildren(database)

    Service->>Database: CreateIterator()
    Database->>Database: Copy schemas as component collection
    Database-->>Service: CatalogIterator

    loop While iterator has next item
        Service->>Iterator: MoveNext()
        Iterator-->>Service: true

        Service->>Iterator: Current
        Iterator-->>Service: Component

        Service->>Component: Name
        Component-->>Service: componentName

        Service->>Service: Process(component)
    end

    Service->>Iterator: MoveNext()
    Iterator-->>Service: false

    Service-->>Client: Traversal completed
```

### 6. DDL Execution (Command Pattern)

**Purpose:**
Encapsulate a request as an object, allowing it to be executed, queued, or logged independently.

**Example:**
A remote control can execute different commands such as turning a light on or off without knowing how the light works.

#### Class Diagram

```mermaid
classDiagram
direction LR

class ICommand {
    <<Command>>
    +Execute()
}

class TurnOnLightCommand {
    <<Concrete Command>>
}

class TurnOffLightCommand {
    <<Concrete Command>>
}

class Light {
    <<Receiver>>
    +TurnOn()
    +TurnOff()
}

class RemoteControl {
    <<Invoker>>
    +SetCommand(command)
    +PressButton()
}

ICommand <|.. TurnOnLightCommand
ICommand <|.. TurnOffLightCommand

TurnOnLightCommand --> Light
TurnOffLightCommand --> Light

RemoteControl --> ICommand : executes
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor User
    participant Remote as RemoteControl
    participant Command as TurnOnLightCommand
    participant Light

    User->>Remote: PressButton()
    Remote->>Command: Execute()
    Command->>Light: TurnOn()
    Light-->>Command: Done
    Command-->>Remote: Success
```

#### Simplified Code

```csharp
public interface ICommand
{
    // Execute the request
    void Execute();
}

public class Light
{
    public void TurnOn()
    {
        // Turn the light on
    }

    public void TurnOff()
    {
        // Turn the light off
    }
}

public class TurnOnLightCommand : ICommand
{
    private readonly Light _light;

    public TurnOnLightCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        // Delegate the request to the receiver
        _light.TurnOn();
    }
}

public class TurnOffLightCommand : ICommand
{
    private readonly Light _light;

    public TurnOffLightCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        // Delegate the request to the receiver
        _light.TurnOff();
    }
}

public class RemoteControl
{
    private ICommand _command;

    // Configure the command to execute
    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    // Execute the configured command
    public void PressButton()
    {
        _command.Execute();
    }
}
```

**Benefits**

* Decouples the sender from the receiver.
* Supports undo, redo, logging, and command queues.
* Makes adding new commands easy.
* Follows the Open/Closed Principle.

**Application:** Encapsulates Data Definition Language (DDL) requests (like `CreateTable`, `CreateSchema`) as standalone objects that contain all information about the request.

**Why apply?** The Command Pattern allows the `QueryProcessor` to parameterize the `DdlCommandExecutor` with different requests, decouple the invoker from the receivers (`SchemaService`, `DatabaseService`), and supports future capabilities like queuing, logging, or undoing operations.

```mermaid
classDiagram
direction LR

class IDdlCommand {
    <<Command>>
    +Execute(context : DdlExecutionContext) DdlResult
}

class CreateSchemaCommand {
    <<Concrete Command>>
    -receiver : IDatabaseService
    -database : Database
    -schemaName : string
    +Execute(context : DdlExecutionContext) DdlResult
}

class CreateTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -schema : Schema
    -definition : TableDefinition
    +Execute(context : DdlExecutionContext) DdlResult
}

class AlterTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -schema : Schema
    -tableName : string
    -operation : TableAlterOperation
    +Execute(context : DdlExecutionContext) DdlResult
}

class DropTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -schema : Schema
    -tableName : string
    +Execute(context : DdlExecutionContext) DdlResult
}

class IDdlCommandExecutor {
    <<Invoker>>
    +Execute(
        command : IDdlCommand,
        context : DdlExecutionContext
    ) DdlResult
}

class DdlCommandExecutor {
    <<Concrete Invoker>>
}

class IDdlCommandFactory {
    <<Factory>>
    +Create(request : DdlRequest) IDdlCommand
}

class IDatabaseService {
    <<Receiver>>
    +CreateSchema(
        database : Database,
        name : string,
        context : DdlExecutionContext
    ) Schema
}

class ISchemaService {
    <<Receiver>>
    +CreateTable(
        schema : Schema,
        definition : TableDefinition,
        context : DdlExecutionContext
    ) Table

    +AlterTable(
        schema : Schema,
        tableName : string,
        operation : TableAlterOperation,
        context : DdlExecutionContext
    ) Table

    +DropTable(
        schema : Schema,
        tableName : string,
        context : DdlExecutionContext
    )
}

class DdlRequest
class DdlExecutionContext
class DdlResult
class Database
class Schema
class TableDefinition
class TableAlterOperation

IDdlCommand <|.. CreateSchemaCommand
IDdlCommand <|.. CreateTableCommand
IDdlCommand <|.. AlterTableCommand
IDdlCommand <|.. DropTableCommand

IDdlCommandExecutor <|.. DdlCommandExecutor
DdlCommandExecutor --> IDdlCommand : invokes

IDdlCommandFactory --> DdlRequest : reads
IDdlCommandFactory --> IDdlCommand : creates

CreateSchemaCommand --> IDatabaseService : receiver
CreateSchemaCommand --> Database : target

CreateTableCommand --> ISchemaService : receiver
CreateTableCommand --> Schema : target
CreateTableCommand --> TableDefinition : request data

AlterTableCommand --> ISchemaService : receiver
AlterTableCommand --> TableAlterOperation : request data

DropTableCommand --> ISchemaService : receiver
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Executor as IDdlCommandExecutor
    participant Command as CreateSchemaCommand
    participant Service as IDatabaseService
    participant Tx as IMetadataTransactionPort
    participant Database
    participant Catalog as ICatalogManager

    Client->>Executor: Execute(command, context)
    Executor->>Command: Execute(context)

    Command->>Service: CreateSchema(database, schemaName, context)
    activate Service

    Service->>Tx: Begin(context)
    Service->>Database: GetSchema(schemaName)

    alt Schema already exists
        Database-->>Service: Existing Schema
        Service->>Tx: Rollback(context)
        Service-->>Command: throw DuplicateSchemaException

    else Schema does not exist
        Database-->>Service: Not found

        Service->>Service: new Schema(schemaName)
        Service->>Database: AddSchema(schema)
        Database-->>Service: Success

        Service->>Catalog: Register(schema)

        alt Registration failed
            Catalog-->>Service: throw CatalogException
            Service->>Database: RemoveSchema(schemaName)
            Service->>Tx: Rollback(context)
            Service-->>Command: propagate exception

        else Registration succeeded
            Catalog-->>Service: Success
            Service->>Tx: Commit(context)
            Service-->>Command: Schema
        end
    end

    deactivate Service

    Command-->>Executor: DdlResult
    Executor-->>Client: DdlResult
```

### 7. DDL Coordination (Facade Pattern)

**Purpose:**
Provide a simple interface to a complex subsystem.

**Example:**
A home theater can be started with a single method instead of operating each device individually.

#### Class Diagram

```mermaid
classDiagram
direction LR

class TV {
    +TurnOn()
}

class SoundSystem {
    +TurnOn()
}

class DVDPlayer {
    +TurnOn()
    +Play(movie)
}

class HomeTheaterFacade {
    <<Facade>>
    +WatchMovie(movie : string)
}

HomeTheaterFacade --> TV
HomeTheaterFacade --> SoundSystem
HomeTheaterFacade --> DVDPlayer
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor User
    participant Facade as HomeTheaterFacade
    participant TV
    participant Sound as SoundSystem
    participant DVD as DVDPlayer

    User->>Facade: WatchMovie("Avatar")
    Facade->>TV: TurnOn()
    Facade->>Sound: TurnOn()
    Facade->>DVD: TurnOn()
    Facade->>DVD: Play("Avatar")
    Facade-->>User: Ready
```

#### Simplified Code

```csharp
public class TV
{
    public void TurnOn()
    {
        // Turn on the TV
    }
}

public class SoundSystem
{
    public void TurnOn()
    {
        // Turn on the sound system
    }
}

public class DVDPlayer
{
    public void TurnOn()
    {
        // Turn on the DVD player
    }

    public void Play(string movie)
    {
        // Play the selected movie
    }
}

public class HomeTheaterFacade
{
    private readonly TV _tv = new();
    private readonly SoundSystem _sound = new();
    private readonly DVDPlayer _dvd = new();

    // Provide one simple operation to coordinate the subsystem
    public void WatchMovie(string movie)
    {
        _tv.TurnOn();
        _sound.TurnOn();
        _dvd.TurnOn();
        _dvd.Play(movie);
    }
}
```

**Benefits**

* Hides subsystem complexity.
* Provides a simple, unified interface.
* Reduces coupling between clients and subsystem classes.
* Makes the subsystem easier to use and maintain.

**Application:** `SchemaService` and `DatabaseService` coordinate complex Create, Drop, and Alter operations for database objects.

**Why apply?** The Facade Pattern provides a unified, high-level interface for DDL operations, shielding the clients (like DDL Commands) from the complexities of the underlying subsystems. Instead of manually coordinating `CatalogManager`, `TableDirector`, `StorageEngine`, and various factories, the clients simply call methods like `CreateTable()` or `DropSchema()` on these services.

```mermaid
classDiagram
direction LR

class IDatabaseService {
    <<Facade Interface>>

    +CreateSchema(
        database : Database,
        definition : SchemaDefinition
    ) Schema

    +DropSchema(
        database : Database,
        name : string,
        cascade : bool
    ) DdlResult

    +RenameSchema(
        database : Database,
        oldName : string,
        newName : string
    ) Schema
}

class ISchemaService {
    <<Facade Interface>>

    +CreateTable(
        schema : Schema,
        definition : TableDefinition
    ) Table

    +DropTable(
        schema : Schema,
        name : string,
        cascade : bool
    ) DdlResult

    +AlterTable(
        schema : Schema,
        tableName : string,
        operation : TableAlterOperation
    ) Table

    +CreateView(
        schema : Schema,
        definition : ViewDefinition
    ) View

    +CreateProcedure(
        schema : Schema,
        definition : ProcedureDefinition
    ) StoredProcedure

    +CreateSequence(
        schema : Schema,
        definition : SequenceDefinition
    ) Sequence
}

class DatabaseService {
    <<Facade>>

    -catalog : ICatalogManager
    -transactionPort : IMetadataTransactionPort
}

class SchemaService {
    <<Facade>>

    -catalog : ICatalogManager
    -dependencyService : ICatalogDependencyService
    -tableDirector : TableDirector
    -storagePort : IStorageObjectPort
    -transactionPort : IMetadataTransactionPort
}

class ICatalogManager {
    <<Subsystem>>

    +ObjectExists(parent, name) bool
    +Register(component)
    +Update(component)
    +Remove(component)
}

class ICatalogDependencyService {
    <<Subsystem>>

    +GetDependencies(component) IReadOnlyCollection~ICatalogComponent~
    +RemoveDependencies(component)
}

class TableDirector {
    <<Subsystem>>

    +Construct(definition : TableDefinition) Table
}

class IStorageObjectPort {
    <<External Subsystem Port>>

    +AllocateTable(table : Table)
    +AlterTable(table : Table, operation : TableAlterOperation)
    +DeallocateTable(table : Table)
}

class IMetadataTransactionPort {
    <<External Subsystem Port>>

    +Begin()
    +Commit()
    +Rollback()
}

class Database
class Schema
class Table
class View
class StoredProcedure
class Sequence
class SchemaDefinition
class TableDefinition
class TableAlterOperation
class ViewDefinition
class ProcedureDefinition
class SequenceDefinition
class DdlResult
class ICatalogComponent

IDatabaseService <|.. DatabaseService
ISchemaService <|.. SchemaService

DatabaseService --> ICatalogManager
DatabaseService --> IMetadataTransactionPort
DatabaseService --> Database

SchemaService --> ICatalogManager
SchemaService --> ICatalogDependencyService
SchemaService --> TableDirector
SchemaService --> IStorageObjectPort
SchemaService --> IMetadataTransactionPort

SchemaService --> Schema
SchemaService --> Table
SchemaService --> View
SchemaService --> StoredProcedure
SchemaService --> Sequence
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Facade as ISchemaService
    participant Catalog as ICatalogManager
    participant Director as TableDirector
    participant Storage as IStorageObjectPort
    participant Schema
    participant Tx as IMetadataTransactionPort

    Client->>Facade: CreateTable(schema, definition, context)

    Facade->>Catalog: ObjectExists(schema, definition.Name)
    Catalog-->>Facade: exists

    alt Table already exists
        Facade-->>Client: throw DuplicateObjectException
    else Table does not exist
        Facade->>Tx: Begin(context)

        Facade->>Director: Construct(definition)
        Director-->>Facade: Table

        Facade->>Storage: AllocateTable(table)

        alt Storage allocation failed
            Storage-->>Facade: error
            Facade->>Tx: Rollback(context)
            Facade-->>Client: DdlResult.Failed
        else Storage allocated
            Storage-->>Facade: success

            Facade->>Schema: AddTable(table)
            Schema-->>Facade: success

            Facade->>Catalog: Register(table)

            alt Catalog registration failed
                Catalog-->>Facade: error
                Facade->>Storage: DeallocateTable(table)
                Facade->>Schema: RemoveTable(table.Name)
                Facade->>Tx: Rollback(context)
                Facade-->>Client: DdlResult.Failed
            else Catalog registered
                Catalog-->>Facade: success
                Facade->>Tx: Commit(context)
                Facade-->>Client: Table
            end
        end
    end
```



### 8. Persist and Query Metadata (Repository Pattern)

**Purpose:**
Encapsulate data access logic behind a repository interface.

**Example:**
A user management system retrieves and stores users without exposing where the data comes from (database, file, or memory).

#### Class Diagram

```mermaid
classDiagram
direction LR

class IUserRepository {
    <<Repository>>
    +GetById(id : int) User
    +GetAll() List~User~
    +Add(user : User)
    +Remove(id : int)
}

class UserRepository {
    <<Concrete Repository>>
}

class User {
    +Id : int
    +Name : string
}

class UserService {
    <<Client>>
    +Register(user : User)
    +FindUser(id : int)
}

IUserRepository <|.. UserRepository

UserRepository --> User : stores
UserService --> IUserRepository : uses
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant Service as UserService
    participant Repository as IUserRepository

    Client->>Service: FindUser(1)
    Service->>Repository: GetById(1)
    Repository-->>Service: User
    Service-->>Client: User
```

#### Simplified Code

```csharp
public class User
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public interface IUserRepository
{
    // Find a user by id
    User? GetById(int id);

    // Return all users
    IReadOnlyCollection<User> GetAll();

    // Store a new user
    void Add(User user);

    // Remove an existing user
    void Remove(int id);
}

public class UserRepository : IUserRepository
{
    public User? GetById(int id)
    {
        // Retrieve a user from the data source
        return default;
    }

    public IReadOnlyCollection<User> GetAll()
    {
        // Retrieve all users
        return [];
    }

    public void Add(User user)
    {
        // Save the user
    }

    public void Remove(int id)
    {
        // Delete the user
    }
}

public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    // Delegate data access to the repository
    public User? FindUser(int id)
    {
        return _repository.GetById(id);
    }

    // Delegate persistence to the repository
    public void Register(User user)
    {
        _repository.Add(user);
    }
}
```

**Benefits**

* Separates business logic from data access logic.
* Makes the data source easy to replace.
* Improves testability through repository interfaces.
* Centralizes CRUD operations in one place.

**Application:** `CatalogManager` and catalog repositories manage the persistence and retrieval of database metadata.

**Why apply?** The Repository Pattern acts as an in-memory collection interface for accessing domain objects (metadata like schemas, tables, columns, indexes). It abstracts away the underlying storage details (such as reading or writing to system tables or files) and provides a clean, domain-centric interface. This shields the `CatalogManager` and Facade services from the complexities of data access, allowing them to focus purely on metadata coordination and DDL logic.

```mermaid
classDiagram
direction LR

%% =====================================================
%% Catalog Application Contract
%% =====================================================

class ICatalogManager {
    <<Catalog Service Interface>>

    +Register(component : ICatalogComponent)
    +Update(component : ICatalogComponent)
    +Remove(component : ICatalogComponent)

    +GetDatabase(name : string) Database

    +GetSchema(
        databaseId : int,
        name : string
    ) Schema

    +GetTable(
        schemaId : int,
        name : string
    ) Table

    +ObjectExists(
        parent : ICatalogComposite,
        name : string,
        objectType : CatalogObjectType
    ) bool
}

class CatalogManager {
    <<Catalog Service>>

    -databaseRepository : IDatabaseCatalogRepository
    -schemaRepository : ISchemaCatalogRepository
    -tableRepository : ITableCatalogRepository

    +Register(component : ICatalogComponent)
    +Update(component : ICatalogComponent)
    +Remove(component : ICatalogComponent)

    +GetDatabase(name : string) Database

    +GetSchema(
        databaseId : int,
        name : string
    ) Schema

    +GetTable(
        schemaId : int,
        name : string
    ) Table

    +ObjectExists(
        parent : ICatalogComposite,
        name : string,
        objectType : CatalogObjectType
    ) bool
}

%% =====================================================
%% Repository Contracts
%% =====================================================

class IDatabaseCatalogRepository {
    <<Repository>>

    +Add(database : Database)
    +Update(database : Database)
    +Remove(databaseId : int)

    +FindById(databaseId : int) Database
    +FindByName(name : string) Database

    +GetAll() IReadOnlyCollection~Database~
    +Exists(name : string) bool
}

class ISchemaCatalogRepository {
    <<Repository>>

    +Add(schema : Schema)
    +Update(schema : Schema)
    +Remove(schemaId : int)

    +FindById(schemaId : int) Schema

    +FindByName(
        databaseId : int,
        name : string
    ) Schema

    +GetByDatabase(
        databaseId : int
    ) IReadOnlyCollection~Schema~

    +Exists(
        databaseId : int,
        name : string
    ) bool
}

class ITableCatalogRepository {
    <<Repository>>

    +Add(table : Table)
    +Update(table : Table)
    +Remove(tableId : int)

    +FindById(tableId : int) Table

    +FindByName(
        schemaId : int,
        name : string
    ) Table

    +GetBySchema(
        schemaId : int
    ) IReadOnlyCollection~Table~

    +Exists(
        schemaId : int,
        name : string
    ) bool
}

%% =====================================================
%% Concrete Repositories
%% =====================================================

class DatabaseCatalogRepository {
    <<Concrete Repository>>

    -store : ICatalogDataStore

    +Add(database : Database)
    +Update(database : Database)
    +Remove(databaseId : int)

    +FindById(databaseId : int) Database
    +FindByName(name : string) Database

    +GetAll() IReadOnlyCollection~Database~
    +Exists(name : string) bool
}

class SchemaCatalogRepository {
    <<Concrete Repository>>

    -store : ICatalogDataStore

    +Add(schema : Schema)
    +Update(schema : Schema)
    +Remove(schemaId : int)

    +FindById(schemaId : int) Schema

    +FindByName(
        databaseId : int,
        name : string
    ) Schema

    +GetByDatabase(
        databaseId : int
    ) IReadOnlyCollection~Schema~

    +Exists(
        databaseId : int,
        name : string
    ) bool
}

class TableCatalogRepository {
    <<Concrete Repository>>

    -store : ICatalogDataStore

    +Add(table : Table)
    +Update(table : Table)
    +Remove(tableId : int)

    +FindById(tableId : int) Table

    +FindByName(
        schemaId : int,
        name : string
    ) Table

    +GetBySchema(
        schemaId : int
    ) IReadOnlyCollection~Table~

    +Exists(
        schemaId : int,
        name : string
    ) bool
}

%% =====================================================
%% Catalog Persistence Port
%% =====================================================

class ICatalogDataStore {
    <<Persistence Port>>

    +Insert(
        catalogName : string,
        record : CatalogRecord
    )

    +Update(
        catalogName : string,
        id : int,
        record : CatalogRecord
    )

    +Delete(
        catalogName : string,
        id : int
    )

    +FindById(
        catalogName : string,
        id : int
    ) CatalogRecord

    +FindOne(
        catalogName : string,
        criteria : CatalogCriteria
    ) CatalogRecord

    +FindMany(
        catalogName : string,
        criteria : CatalogCriteria
    ) IReadOnlyCollection~CatalogRecord~

    +Exists(
        catalogName : string,
        criteria : CatalogCriteria
    ) bool
}

class CatalogRecord {
    <<Persistence Model>>

    +Id : int
    +Values : IReadOnlyDictionary~string, object~
}

class CatalogCriteria {
    <<Query Object>>

    +Conditions : IReadOnlyDictionary~string, object~
}

%% =====================================================
%% Existing Facades
%% =====================================================

class DatabaseService {
    <<Facade>>

    -catalog : ICatalogManager

    +CreateSchema(
        database : Database,
        name : string
    ) Schema

    +DropSchema(
        database : Database,
        name : string,
        cascade : bool
    )

    +RenameSchema(
        database : Database,
        oldName : string,
        newName : string
    )
}

class SchemaService {
    <<Facade>>

    -catalog : ICatalogManager

    +CreateTable(
        schema : Schema,
        definition : TableDefinition
    ) Table

    +DropTable(
        schema : Schema,
        name : string,
        cascade : bool
    )

    +RenameTable(
        schema : Schema,
        oldName : string,
        newName : string
    )
}

%% =====================================================
%% Catalog Domain Contracts
%% =====================================================

class ICatalogComponent {
    <<Component>>

    +CatalogId : int
    +Name : string
    +ObjectType : CatalogObjectType
}

class ICatalogComposite {
    <<Composite>>

    +Children : IReadOnlyCollection~ICatalogComponent~
}

ICatalogComponent <|-- ICatalogComposite

class CatalogObjectType {
    <<enumeration>>

    DATABASE
    SCHEMA
    TABLE
    COLUMN
    CONSTRAINT
    INDEX
    PARTITION
    TRIGGER
    VIEW
    STORED_PROCEDURE
    SEQUENCE
}

%% =====================================================
%% Database Objects
%% =====================================================

class Database {
    <<Aggregate Root>>

    +DatabaseId : int
    +Name : string
    +Owner : string
    +Schemas : IReadOnlyCollection~Schema~
}

class Schema {
    <<Aggregate>>

    +SchemaId : int
    +Name : string
    +Parent : Database
    +Tables : IReadOnlyCollection~Table~
}

class Table {
    <<Aggregate Root>>

    +TableId : int
    +Name : string
    +Parent : Schema
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~
}

class Column {
    +ColumnId : int
    +Name : string
}

class Constraint {
    <<abstract>>

    +ConstraintId : int
    +Name : string
}

class Index {
    <<abstract>>

    +IndexId : int
    +Name : string
}

class Partition {
    +PartitionId : int
    +Name : string
}

class Trigger {
    +TriggerId : int
    +Name : string
}

class TableDefinition

%% =====================================================
%% Catalog Service Relationships
%% =====================================================

ICatalogManager <|.. CatalogManager

CatalogManager --> IDatabaseCatalogRepository : routes database metadata
CatalogManager --> ISchemaCatalogRepository : routes schema metadata
CatalogManager --> ITableCatalogRepository : routes table metadata

CatalogManager --> ICatalogComponent : registers
CatalogManager --> ICatalogComposite : searches parent
CatalogManager --> CatalogObjectType : resolves type

%% =====================================================
%% Repository Relationships
%% =====================================================

IDatabaseCatalogRepository <|.. DatabaseCatalogRepository
ISchemaCatalogRepository <|.. SchemaCatalogRepository
ITableCatalogRepository <|.. TableCatalogRepository

DatabaseCatalogRepository --> ICatalogDataStore : persists metadata
SchemaCatalogRepository --> ICatalogDataStore : persists metadata
TableCatalogRepository --> ICatalogDataStore : persists aggregate metadata

DatabaseCatalogRepository --> Database : maps
SchemaCatalogRepository --> Schema : maps
TableCatalogRepository --> Table : maps

ICatalogDataStore --> CatalogRecord : reads/writes
ICatalogDataStore --> CatalogCriteria : queries

%% =====================================================
%% Facade Relationships
%% =====================================================

DatabaseService --> ICatalogManager : persists and queries schemas
SchemaService --> ICatalogManager : persists and queries tables

%% =====================================================
%% Composite Relationships
%% =====================================================

ICatalogComposite <|.. Database
ICatalogComposite <|.. Schema
ICatalogComposite <|.. Table

ICatalogComponent <|.. Column
ICatalogComponent <|.. Constraint
ICatalogComponent <|.. Index
ICatalogComponent <|.. Partition
ICatalogComponent <|.. Trigger

Database "1" *-- "*" Schema : contains
Schema "1" *-- "*" Table : contains

Table "1" *-- "*" Column : contains
Table "1" *-- "*" Constraint : contains
Table "1" *-- "*" Index : contains
Table "1" *-- "*" Partition : contains
Table "1" *-- "*" Trigger : contains
```

```mermaid
sequenceDiagram
    autonumber

    participant Service as SchemaService
    participant Catalog as ICatalogManager
    participant Manager as CatalogManager
    participant TableRepo as ITableCatalogRepository
    participant Store as ICatalogDataStore

    Service->>Catalog: GetTable(schemaId, tableName)
    Catalog->>Manager: GetTable(schemaId, tableName)

    Manager->>TableRepo: FindByName(schemaId, tableName)

    TableRepo->>Store: FindOne("sys_tables", criteria)
    Store-->>TableRepo: tableRecord

    alt Table not found
        TableRepo-->>Manager: null
        Manager-->>Service: null

    else Table found
        TableRepo->>Store: FindMany("sys_columns", tableId)
        Store-->>TableRepo: columnRecords

        TableRepo->>Store: FindMany("sys_constraints", tableId)
        Store-->>TableRepo: constraintRecords

        TableRepo->>Store: FindMany("sys_indexes", tableId)
        Store-->>TableRepo: indexRecords

        TableRepo->>TableRepo: Map records to Table aggregate
        TableRepo-->>Manager: Table
        Manager-->>Service: Table
    end
```

### 9. Metadata Events (Observer Pattern)

**Purpose:**
Define a one-to-many dependency so that when an object's state changes, all registered observers are notified automatically.

**Example:**
A weather station notifies multiple displays whenever the temperature changes.

#### Class Diagram

```mermaid
classDiagram
direction LR

class IObserver {
    <<Observer>>
    +Update(temperature : double)
}

class CurrentDisplay {
    <<Concrete Observer>>
}

class StatisticsDisplay {
    <<Concrete Observer>>
}

class WeatherStation {
    <<Subject>>
    +Attach(observer)
    +Detach(observer)
    +Notify()
    +SetTemperature(value : double)
}

IObserver <|.. CurrentDisplay
IObserver <|.. StatisticsDisplay

WeatherStation --> IObserver : notifies
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Sensor
    participant Station as WeatherStation
    participant Display1 as CurrentDisplay
    participant Display2 as StatisticsDisplay

    Sensor->>Station: SetTemperature(28)
    Station->>Display1: Update(28)
    Display1-->>Station: Updated

    Station->>Display2: Update(28)
    Display2-->>Station: Updated
```

#### Simplified Code

```csharp
public interface IObserver
{
    // Receive notification when the subject changes
    void Update(double temperature);
}

public class CurrentDisplay : IObserver
{
    public void Update(double temperature)
    {
        // Display the current temperature
    }
}

public class StatisticsDisplay : IObserver
{
    public void Update(double temperature)
    {
        // Update statistical information
    }
}

public class WeatherStation
{
    private readonly List<IObserver> _observers = [];

    // Register an observer
    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
    }

    // Unregister an observer
    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    // Notify all registered observers
    public void Notify()
    {
        // Invoke Update() on each observer
    }

    // Update the temperature and notify observers
    public void SetTemperature(double value)
    {
        // Store the new temperature

        // Notify all observers about the change
        Notify();
    }
}
```

**Benefits**

* Decouples the subject from its observers.
* Supports one-to-many event notification.
* Makes adding new observers easy.
* Follows the Open/Closed Principle.

**Application:** `IMetadataEventPublisher` notifies various handlers (`CacheInvalidationHandler`, `StatisticsUpdateHandler`, `AuditLoggingHandler`) whenever metadata (like Tables or Schemas) is created, altered, or dropped.

**Why apply?** The Observer Pattern decouples the core metadata operations from secondary tasks like caching, statistics gathering, and auditing. Instead of hardcoding these reactions inside `CatalogManager` or DDL Commands, the system simply publishes a `MetadataEvent`. Handlers independently subscribe to these events and react accordingly, making the system highly extensible (e.g., adding a new handler doesn't require modifying existing catalog code).

```mermaid
classDiagram
direction LR

%% =====================================================
%% Subject / Publisher
%% =====================================================

class IMetadataEventPublisher {
    <<Subject>>
    +Subscribe(observer : IMetadataObserver)
    +Unsubscribe(observer : IMetadataObserver)
    +Publish(event : MetadataEvent)
}

class MetadataEventPublisher {
    <<Concrete Subject>>
    -observers : List~IMetadataObserver~
    +Subscribe(observer : IMetadataObserver)
    +Unsubscribe(observer : IMetadataObserver)
    +Publish(event : MetadataEvent)
}

%% =====================================================
%% Observer Contract
%% =====================================================

class IMetadataObserver {
    <<Observer>>
    +OnMetadataChanged(event : MetadataEvent)
}

%% =====================================================
%% Concrete Observers
%% =====================================================

class CatalogCacheObserver {
    <<Concrete Observer>>
    -cache : ICatalogCache
    +OnMetadataChanged(event : MetadataEvent)
}

class MetadataStatisticsObserver {
    <<Concrete Observer>>
    -statisticsStore : IMetadataStatisticsStore
    +OnMetadataChanged(event : MetadataEvent)
}

class MetadataAuditObserver {
    <<Concrete Observer>>
    -auditRepository : IMetadataAuditRepository
    +OnMetadataChanged(event : MetadataEvent)
}

%% =====================================================
%% Metadata Events
%% =====================================================

class MetadataEvent {
    <<Event>>
    +EventId : Guid
    +EventType : MetadataEventType
    +ObjectId : int
    +ObjectType : CatalogObjectType
    +ObjectName : string
    +ParentId : int
    +Timestamp : DateTime
    +Actor : string
    +PreviousSnapshot : MetadataSnapshot
    +CurrentSnapshot : MetadataSnapshot
}

class MetadataSnapshot {
    <<Event Data>>
    +Properties : IReadOnlyDictionary~string, object~
}

class MetadataEventType {
    <<enumeration>>
    CREATED
    UPDATED
    RENAMED
    REMOVED
}

class CatalogObjectType {
    <<enumeration>>
    DATABASE
    SCHEMA
    TABLE
    COLUMN
    CONSTRAINT
    INDEX
    PARTITION
    TRIGGER
    VIEW
    STORED_PROCEDURE
    SEQUENCE
}

%% =====================================================
%% Cache Subsystem
%% =====================================================

class ICatalogCache {
    <<Cache Port>>
    +Get(key : CatalogCacheKey) ICatalogComponent
    +Set(key : CatalogCacheKey, component : ICatalogComponent)
    +Remove(key : CatalogCacheKey)
    +InvalidateChildren(parentId : int)
}

class CatalogCacheKey {
    <<Value Object>>
    +ObjectType : CatalogObjectType
    +ObjectId : int
    +ParentId : int
    +Name : string
}

%% =====================================================
%% Statistics Subsystem
%% =====================================================

class IMetadataStatisticsStore {
    <<Statistics Port>>
    +IncrementObjectCount(type : CatalogObjectType)
    +DecrementObjectCount(type : CatalogObjectType)
    +RecordModification(type : CatalogObjectType, timestamp : DateTime)
    +RecordRename(type : CatalogObjectType)
}

%% =====================================================
%% Audit Subsystem
%% =====================================================

class IMetadataAuditRepository {
    <<Audit Port>>
    +Add(entry : MetadataAuditEntry)
}

class MetadataAuditEntry {
    <<Audit Record>>
    +AuditId : Guid
    +EventType : MetadataEventType
    +ObjectType : CatalogObjectType
    +ObjectId : int
    +ObjectName : string
    +Actor : string
    +Timestamp : DateTime
    +PreviousValues : MetadataSnapshot
    +CurrentValues : MetadataSnapshot
}

%% =====================================================
%% Relationships
%% =====================================================

IMetadataEventPublisher <|.. MetadataEventPublisher
IMetadataObserver <|.. CatalogCacheObserver
IMetadataObserver <|.. MetadataStatisticsObserver
IMetadataObserver <|.. MetadataAuditObserver

MetadataEventPublisher o-- "*" IMetadataObserver : observers
MetadataEventPublisher --> MetadataEvent : publishes
IMetadataObserver --> MetadataEvent : receives

MetadataEvent --> MetadataEventType
MetadataEvent --> CatalogObjectType
MetadataEvent --> MetadataSnapshot

CatalogCacheObserver --> ICatalogCache : updates or invalidates
CatalogCacheObserver --> CatalogCacheKey : creates

MetadataStatisticsObserver --> IMetadataStatisticsStore : updates statistics

MetadataAuditObserver --> IMetadataAuditRepository : writes audit
MetadataAuditObserver --> MetadataAuditEntry : creates

MetadataAuditEntry --> MetadataEventType
MetadataAuditEntry --> CatalogObjectType
MetadataAuditEntry --> MetadataSnapshot

ICatalogCache --> ICatalogComponent : caches
CatalogCacheKey --> CatalogObjectType
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Service as SchemaService
    participant Tx as IMetadataTransactionPort
    participant Director as TableDirector
    participant Storage as IStorageObjectPort
    participant Schema
    participant Catalog as CatalogManager
    participant Repo as ITableCatalogRepository
    participant Collector as IMetadataEventCollector
    participant Dispatcher as MetadataEventCommitDispatcher
    participant Publisher as IMetadataEventPublisher
    participant Cache as CatalogCacheObserver
    participant Stats as MetadataStatisticsObserver
    participant Audit as MetadataAuditObserver

    Client->>Service: CreateTable(schema, definition)

    Service->>Tx: Begin()

    Service->>Director: Construct(definition)
    Director-->>Service: Table

    Service->>Storage: AllocateTable(table)
    Storage-->>Service: Success

    Service->>Schema: AddTable(table)
    Schema-->>Service: Success

    Service->>Catalog: Register(table, context)
    Catalog->>Repo: Add(table)
    Repo-->>Catalog: Success

    Catalog->>Collector: Add(MetadataCreatedEvent)
    Collector-->>Catalog: Event queued

    Catalog-->>Service: Registration completed

    alt Transaction commit succeeds
        Service->>Tx: Commit()
        Tx->>Dispatcher: DispatchCommittedEvents()

        Dispatcher->>Collector: GetPendingEvents()
        Collector-->>Dispatcher: Events

        loop Each committed metadata event
            Dispatcher->>Publisher: Publish(event)

            par Notify cache
                Publisher->>Cache: OnMetadataChanged(event)
                Cache->>Cache: Update or invalidate cache
            and Notify statistics
                Publisher->>Stats: OnMetadataChanged(event)
                Stats->>Stats: Increment table count
            and Notify audit
                Publisher->>Audit: OnMetadataChanged(event)
                Audit->>Audit: Save audit record
            end
        end

        Dispatcher->>Collector: Clear()
        Service-->>Client: Table

    else Transaction fails
        Service->>Tx: Rollback()
        Tx->>Dispatcher: DiscardRolledBackEvents()
        Dispatcher->>Collector: Clear()
        Service-->>Client: DDL failure
    end
```

### 10. System Initialization (Facade Pattern)

**Application:** `DbEngineFacade` groups complex startup steps for Disk, Storage, and Catalog.

**Why apply?** The Facade Pattern provides a unified, high-level interface to the complex subsystems of the database engine (Disk, Storage, Catalog, Transaction, Recovery). This simplifies the interaction for the `DatabaseServer`, which only needs to call `Start()`, `Stop()`, or `Recover()` without managing the intricate initialization order and dependencies of each internal manager.

```mermaid
classDiagram
direction LR

class DbEngineFacade {
    -diskManager : IDiskManager
    -storageEngine : IStorageEngine
    -catalogManager : ICatalogManager
    -transactionManager : ITransactionManager
    -recoveryManager : IRecoveryManager
    +Start(safeMode : bool)
    +Stop(force : bool)
    +Restart()
    +Recover()
}

class DatabaseServer {
    -engineFacade : DbEngineFacade
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
    -catalog : ICatalogManager
    -connectionPool : IConnectionPool
    +CreateDatabase(name : string)
    +DropDatabase(name : string, cascade : bool)
    +OpenDatabase(name : string)
    +CloseDatabase(name : string)
    +AttachDatabase(name : string, filePath : string)
    +DetachDatabase(name : string)
}

class IDiskManager
class IStorageEngine
class ICatalogManager
class ITransactionManager
class IRecoveryManager
class IConnectionPool

DatabaseServer --> DbEngineFacade : controls
DatabaseServer --> DatabaseManager : manages databases

DbEngineFacade --> IDiskManager : initializes
DbEngineFacade --> IStorageEngine : initializes
DbEngineFacade --> ICatalogManager : initializes
DbEngineFacade --> ITransactionManager : initializes
DbEngineFacade --> IRecoveryManager : coordinates recovery

DatabaseManager --> ICatalogManager : updates metadata
DatabaseManager --> IConnectionPool : manages connections
```

```mermaid
sequenceDiagram
    autonumber

    actor Admin
    participant Server as DatabaseServer
    participant Facade as DbEngineFacade
    participant Config as ConfigurationManager
    participant Disk as IDiskManager
    participant Storage as IStorageEngine
    participant Catalog as ICatalogManager
    participant Recovery as IRecoveryManager
    participant Tx as ITransactionManager
    participant Monitor as MonitoringManager

    Admin->>Server: Start(safeMode)
    Server->>Server: Status = Starting
    Server->>Facade: Start(safeMode)

    Facade->>Config: LoadConfiguration(filePath)
    Config-->>Facade: configuration

    Facade->>Disk: Initialize(configuration)
    Disk-->>Facade: diskReady

    Facade->>Storage: Initialize(Disk)
    Storage-->>Facade: storageReady

    Facade->>Catalog: LoadCatalog()
    Catalog-->>Facade: catalogReady

    alt Unclean previous shutdown
        Facade->>Recovery: Recover()
        Recovery->>Storage: RedoCommittedOperations()
        Recovery->>Storage: UndoIncompleteTransactions()
        Storage-->>Recovery: recoveryCompleted
        Recovery-->>Facade: recoveryCompleted
    end

    alt Normal mode
        Facade->>Tx: Initialize()
        Tx-->>Facade: transactionManagerReady
    else Safe mode
        Note over Facade,Tx: Transaction processing is limited
    end

    Facade->>Monitor: CollectMetrics()
    Monitor-->>Facade: initialMetrics

    Facade-->>Server: startupCompleted
    Server->>Server: Status = Running
    Server-->>Admin: ServerStatus.Running
```

### 11. Server State Management (State Pattern)

**Purpose:**  
Encapsulate behaviors for Stopped, Running, Recovering, and Failed states of the Database Server to allow it to change its behavior when its internal state changes.

**Application:**  
`DatabaseServer`, `IServerState`

#### Class Diagram

```mermaid
classDiagram
direction TB

class DatabaseServer{
    <<Context>>
    -currentState : IServerState
    +SetState(state: IServerState)
    +Start()
    +Stop()
    +Restart()
    +Recover()
}

class IServerState{
    <<State>>
    +Start(server: DatabaseServer)
    +Stop(server: DatabaseServer)
    +Restart(server: DatabaseServer)
    +Recover(server: DatabaseServer)
}

class StoppedState{
    <<ConcreteState>>
    +Start(server: DatabaseServer)
    +Stop(server: DatabaseServer)
}

class RunningState{
    <<ConcreteState>>
    +Start(server: DatabaseServer)
    +Stop(server: DatabaseServer)
}

class RecoveringState{
    <<ConcreteState>>
    +Recover(server: DatabaseServer)
}

class FailedState{
    <<ConcreteState>>
    +Recover(server: DatabaseServer)
    +Stop(server: DatabaseServer)
}

DatabaseServer o-- IServerState : current state
IServerState <|.. StoppedState
IServerState <|.. RunningState
IServerState <|.. RecoveringState
IServerState <|.. FailedState
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Admin
    participant Server as DatabaseServer
    participant Stopped as StoppedState
    participant Running as RunningState

    Note over Server: Initially in StoppedState
    Admin->>Server: Start()
    Server->>Stopped: Start(this)
    Stopped->>Server: Initialize Engine
    Stopped->>Server: SetState(new RunningState())
    Server-->>Admin: Success

    Note over Server: Now in RunningState
    Admin->>Server: Start()
    Server->>Running: Start(this)
    Running-->>Server: throw InvalidOperationException("Already running")
    Server-->>Admin: Error
```

#### Simplified Code

```csharp
public class DatabaseServer
{
    private IServerState _currentState;

    public DatabaseServer()
    {
        _currentState = new StoppedState();
    }

    public void SetState(IServerState state)
    {
        _currentState = state;
    }

    public void Start() => _currentState.Start(this);
    public void Stop() => _currentState.Stop(this);
    public void Recover() => _currentState.Recover(this);
}

public interface IServerState
{
    void Start(DatabaseServer server);
    void Stop(DatabaseServer server);
    void Recover(DatabaseServer server);
}

public class StoppedState : IServerState
{
    public void Start(DatabaseServer server)
    {
        // Start engine...
        server.SetState(new RunningState());
    }

    public void Stop(DatabaseServer server)
    {
        // Already stopped, do nothing or log
    }

    public void Recover(DatabaseServer server)
    {
        server.SetState(new RecoveringState());
        // Do recovery...
    }
}

public class RunningState : IServerState
{
    public void Start(DatabaseServer server)
    {
        throw new InvalidOperationException("Server is already running.");
    }

    public void Stop(DatabaseServer server)
    {
        // Stop engine...
        server.SetState(new StoppedState());
    }

    public void Recover(DatabaseServer server)
    {
        throw new InvalidOperationException("Cannot recover while running.");
    }
}
```

**Benefits**

- Localizes state-specific behavior in individual classes (`StoppedState`, `RunningState`, etc.).
- Avoids large monolithic `switch` or `if-else` statements in `DatabaseServer`.
- Makes state transitions explicit and easier to maintain.

**Application:** State pattern is applied to `DatabaseServer` to handle its lifecycle states.

**Why apply?** The `DatabaseServer` has complex behaviors that vary depending on its current state (e.g., starting an already running server should fail, stopping a stopped server is a no-op). The State Pattern encapsulates these state-specific rules into separate classes, making the `DatabaseServer` cleaner and making it simple to add new states in the future.

```mermaid
classDiagram
direction TB

class DatabaseServer {
    <<Context>>

    +ServerId : int
    +Version : string
    +Status : ServerStatus

    -currentState : IServerState
    -databaseManager : DatabaseManager
    -configurationManager : ConfigurationManager
    -securityManager : SecurityManager
    -monitoringManager : MonitoringManager

    +SetState(state : IServerState)
    +Start(safeMode : bool)
    +Stop(force : bool)
    +Restart()
    +Recover()
    +HandleSignal(signal : string)
    +GetStatus() ServerStatus

    ~InitializeComponents(safeMode : bool)
    ~ShutdownComponents(force : bool)
    ~RestartComponents()
    ~RecoverComponents()
}

class IServerState {
    <<State>>

    +Status : ServerStatus
    +Start(server : DatabaseServer, safeMode : bool)
    +Stop(server : DatabaseServer, force : bool)
    +Restart(server : DatabaseServer)
    +Recover(server : DatabaseServer)
}

class StoppedState {
    <<ConcreteState>>

    +Status : ServerStatus
    +Start(server : DatabaseServer, safeMode : bool)
    +Stop(server : DatabaseServer, force : bool)
    +Restart(server : DatabaseServer)
    +Recover(server : DatabaseServer)
}

class RunningState {
    <<ConcreteState>>

    +Status : ServerStatus
    +Start(server : DatabaseServer, safeMode : bool)
    +Stop(server : DatabaseServer, force : bool)
    +Restart(server : DatabaseServer)
    +Recover(server : DatabaseServer)
}

class RecoveringState {
    <<ConcreteState>>

    +Status : ServerStatus
    +Start(server : DatabaseServer, safeMode : bool)
    +Stop(server : DatabaseServer, force : bool)
    +Restart(server : DatabaseServer)
    +Recover(server : DatabaseServer)
}

class FailedState {
    <<ConcreteState>>

    +Status : ServerStatus
    +Start(server : DatabaseServer, safeMode : bool)
    +Stop(server : DatabaseServer, force : bool)
    +Restart(server : DatabaseServer)
    +Recover(server : DatabaseServer)
}

class DatabaseManager
class ConfigurationManager
class SecurityManager
class MonitoringManager

DatabaseServer o-- IServerState : current state

IServerState <|.. StoppedState
IServerState <|.. RunningState
IServerState <|.. RecoveringState
IServerState <|.. FailedState

DatabaseServer --> DatabaseManager
DatabaseServer --> ConfigurationManager
DatabaseServer --> SecurityManager
DatabaseServer --> MonitoringManager
```

```mermaid
sequenceDiagram
    autonumber

    actor Admin
    participant Server as DatabaseServer
    participant Stopped as StoppedState
    participant Config as ConfigurationManager
    participant Security as SecurityManager
    participant DBManager as DatabaseManager
    participant Monitoring as MonitoringManager
    participant Running as RunningState

    Note over Server: currentState = StoppedState

    Admin->>Server: Start(safeMode)
    Server->>Stopped: Start(this, safeMode)

    Stopped->>Server: InitializeComponents(safeMode)

    Server->>Config: LoadConfiguration(configPath)
    Config-->>Server: Configuration loaded

    Server->>Security: Initialize security services
    Security-->>Server: Ready

    Server->>DBManager: Initialize database management
    DBManager-->>Server: Ready

    Server->>Monitoring: Start metric collection
    Monitoring-->>Server: Ready

    Stopped->>Server: SetState(new RunningState())
    Server-->>Admin: Success

    Note over Server: currentState = RunningState

    Admin->>Server: Start(safeMode)
    Server->>Running: Start(this, safeMode)
    Running-->>Server: throw InvalidServerStateException
    Server-->>Admin: Error: Server is already running
```