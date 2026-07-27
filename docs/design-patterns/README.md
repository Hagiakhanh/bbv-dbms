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
| 🔴 High | Database Object | Metadata Traversal | `CatalogIterator`, `IIterableCatalog` | Provides a way to sequentially access metadata objects without exposing their underlying representation. | **Iterator** | Completed |
| 🔴 High | Metadata Events | Cache, Statistics, Audit Reactions | Event publisher and handlers | Defines a one-to-many dependency so that when metadata changes, all dependent components are notified. | **Observer** | Completed |
| 🔴 High | Database Object | DDL Script Generation | `DdlScriptGenerator`, `CreateTableScriptGenerator`, `AlterTableScriptGenerator`, `DropTableScriptGenerator` | Defines a fixed script-generation workflow—header, body, and footer—that each DDL statement type customizes to emit valid SQL DDL output. | **Template Method** | Completed |
| 🟡 Medium | Metadata Utility | Export DDL, Dependency Scan | Visitors or traversal services | Separates metadata analysis and export algorithms from the object structure on which they operate. | **Visitor** | Completed |
| 🟡 Medium | Trigger | Execute Trigger Actions | `TriggerExecutor`, trigger actions | Encapsulates trigger actions as objects for execution. | **Command** | Not Started |


---

```mermaid
classDiagram
direction LR

%% =====================================================
%% 1. EXTERNAL PORTS
%% =====================================================

class IDdlRequestSource {
    <<Client Port>>
    +Submit(command : IDdlCommand) DdlResult
}

class IStorageObjectPort {
    <<External Port>>
    +AllocateTable(table : Table)
    +DropTable(tableId : int)
    +AllocateIndex(index : Index)
    +DropIndex(indexId : int)
}

class IMetadataTransactionPort {
    <<External Port>>
    +Begin()
    +Commit()
    +Rollback()
}

%% =====================================================
%% 2. COMPOSITE — CATALOG OBJECT HIERARCHY
%% =====================================================

class ICatalogComponent {
    <<Component / Visitable Element>>
    +Name : string
    +Accept(visitor : IMetadataVisitor)
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
    <<Composite / Product>>
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
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
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
}

class View {
    <<Leaf>>
    +ViewId : int
    +Name : string
    +QueryDefinition : string
}

class StoredProcedure {
    <<Leaf>>
    +ProcedureId : int
    +Name : string
    +Body : string
}

class Sequence {
    <<Leaf>>
    +SequenceId : int
    +Name : string
    +CurrentValue : long
    +Increment : long
    +NextValue() long
}

class Partition {
    <<Leaf>>
    +PartitionId : int
    +Name : string
    +PartitionKey : string
    +PartitionType : PartitionType
}

class Trigger {
    <<Leaf>>
    +TriggerId : int
    +Name : string
    +Event : TriggerEvent
    +Timing : TriggerTiming
    +Body : string
}

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

Column --> DataType
Partition --> PartitionType
Trigger --> TriggerEvent
Trigger --> TriggerTiming

%% =====================================================
%% 3. STRATEGY — CONSTRAINT VALIDATION
%% =====================================================

class Constraint {
    <<Abstract Strategy / Leaf>>
    +ConstraintId : int
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
    +Columns : IReadOnlyCollection~Column~
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
    +Values : IReadOnlyDictionary~int, object~
    +Operation : ValidationOperation
}

class ConstraintValidationResult {
    <<Result>>
    +IsValid : bool
    +Message : string
}

class IRowKeyExtractor {
    <<Collaborator>>
    +ExtractKey(values : IReadOnlyDictionary~int, object~, columns : IReadOnlyCollection~Column~) object
    +HasNullValue(values : IReadOnlyDictionary~int, object~, columns : IReadOnlyCollection~Column~) bool
}

Constraint <|-- PrimaryKeyConstraint
Constraint <|-- UniqueConstraint
Constraint <|-- ForeignKeyConstraint
Constraint <|-- CheckConstraint
Constraint --> ConstraintValidationContext : validates
Constraint --> ConstraintValidationResult : returns
ConstraintValidationContext --> Table
ConstraintValidationContext --> ValidationOperation
PrimaryKeyConstraint --> IRowKeyExtractor
UniqueConstraint --> IRowKeyExtractor
ForeignKeyConstraint --> Table : references
PrimaryKeyConstraint --> Index : checks uniqueness
UniqueConstraint --> Index : checks uniqueness

%% =====================================================
%% 4. INDEX DOMAIN OBJECTS
%% Chỉ mô tả metadata; thao tác vật lý thuộc Storage Engine.
%% =====================================================

class Index {
    <<abstract Leaf>>
    +IndexId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Unique : bool
}

class BTreeIndex
class HashIndex
class BitmapIndex

Index <|-- BTreeIndex
Index <|-- HashIndex
Index <|-- BitmapIndex
Index --> Column : indexes

%% =====================================================
%% 5. CONSTRUCTION DATA
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
    +PartitionKey : string
    +Type : PartitionType
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

TableDefinition "1" *-- "*" ColumnDefinition
TableDefinition "1" *-- "*" ConstraintOptions
TableDefinition "1" *-- "*" IndexOptions
TableDefinition "1" *-- "*" PartitionOptions
TableDefinition "1" *-- "*" TriggerOptions
TableDefinition --> DefinitionValidationResult : returns
ColumnDefinition --> DataType
ConstraintOptions --> ConstraintType
IndexOptions --> IndexType
PartitionOptions --> PartitionType
TriggerOptions --> TriggerEvent
TriggerOptions --> TriggerTiming

%% =====================================================
%% 6. BUILDER — TABLE CONSTRUCTION
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
    -initialized : bool
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
    +Construct(definition : TableDefinition) Table
    -CreateColumn(definition : ColumnDefinition) Column
    -CreatePartition(options : PartitionOptions) Partition
    -CreateTrigger(options : TriggerOptions) Trigger
    -CreateBuildContext(tableName : string, columns : IReadOnlyCollection~Column~) TableBuildContext
}

class TableBuildContext {
    <<Build Context>>
    +TableName : string
    +Columns : IReadOnlyCollection~Column~
    +FindColumn(name : string) Column
}

ITableBuilder <|.. TableBuilder
TableDirector --> ITableBuilder : directs
TableDirector --> IConstraintFactory : creates constraints
TableDirector --> IIndexFactory : creates indexes
TableDirector --> TableDefinition : reads
TableDirector --> ColumnDefinition : creates columns
TableDirector --> PartitionOptions : creates partitions
TableDirector --> TriggerOptions : creates triggers
TableDirector --> TableBuildContext : creates
TableBuilder --> Table : builds
TableBuilder --> Column : adds
TableBuilder --> Constraint : adds
TableBuilder --> Index : adds
TableBuilder --> Partition : adds
TableBuilder --> Trigger : adds
TableBuildContext --> Column : resolves

%% =====================================================
%% 7. FACTORY METHOD — DYNAMIC OBJECT CREATION
%% =====================================================

class IConstraintFactory {
    <<Factory>>
    +Create(options : ConstraintOptions, context : TableBuildContext) Constraint
}

class ConstraintFactory {
    <<Concrete Factory>>
    +Create(options : ConstraintOptions, context : TableBuildContext) Constraint
}

class IIndexFactory {
    <<Factory>>
    +Create(options : IndexOptions, context : TableBuildContext) Index
}

class IndexFactory {
    <<Concrete Factory>>
    +Create(options : IndexOptions, context : TableBuildContext) Index
}

IConstraintFactory <|.. ConstraintFactory
IIndexFactory <|.. IndexFactory
IConstraintFactory --> ConstraintOptions : reads
IConstraintFactory --> TableBuildContext : resolves columns
IConstraintFactory --> Constraint : creates
IIndexFactory --> IndexOptions : reads
IIndexFactory --> TableBuildContext : resolves columns
IIndexFactory --> Index : creates

%% =====================================================
%% 8. FACADE — APPLICATION ENTRY POINT
%% Facade chỉ điều phối domain và subsystem.
%% Transaction workflow thuộc DdlCommandTemplate.
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
}

IDatabaseService <|.. DatabaseService
ISchemaService <|.. SchemaService

DatabaseService --> ICatalogManager : coordinates metadata
DatabaseService --> Database : manages

SchemaService --> ICatalogManager : coordinates metadata
SchemaService --> TableDirector : builds tables
SchemaService --> IStorageObjectPort : coordinates storage
SchemaService --> Schema : manages

%% =====================================================
%% 9. COMMAND — DDL EXECUTION
%% Command đóng gói yêu cầu DDL thành các object độc lập.
%% =====================================================


class IDdlCommand {
    <<Command>>
    +Execute() DdlResult
}

class DdlCommandTemplate {
    <<Abstract Command>>

    #catalog : ICatalogManager
    #transaction : IMetadataTransactionPort
    #eventCollector : IMetadataEventCollector
    #eventDispatcher : MetadataEventCommitDispatcher
    #context : MetadataChangeContext
    #affectedObject : ICatalogComponent

    +Execute() DdlResult
    #PersistMetadata(component : ICatalogComponent)
    #RecordEvent(event : MetadataEvent)
    #CreateSuccessResult(component : ICatalogComponent) DdlResult
    #CreateFailureResult(message : string) DdlResult
}

class CreateSchemaCommand {
    <<Concrete Command>>
    -receiver : IDatabaseService
    -database : Database
    -schemaName : string
    +Execute() DdlResult
}

class CreateTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -schema : Schema
    -definition : TableDefinition
    +Execute() DdlResult
}

class AlterTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -table : Table
    -operation : TableAlterOperation
    +Execute() DdlResult
}

class DropTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -schema : Schema
    -tableName : string
    -cascade : bool
    +Execute() DdlResult
}

class IDdlCommandExecutor {
    <<Invoker Interface>>
    +Execute(command : IDdlCommand) DdlResult
}

class DdlCommandExecutor {
    <<Invoker>>
    +Execute(command : IDdlCommand) DdlResult
}

class DdlResult {
    <<Result>>
    +Success : bool
    +Message : string
    +AffectedObject : ICatalogComponent
}

IDdlCommand <|.. DdlCommandTemplate

DdlCommandTemplate <|-- CreateSchemaCommand
DdlCommandTemplate <|-- CreateTableCommand
DdlCommandTemplate <|-- AlterTableCommand
DdlCommandTemplate <|-- DropTableCommand

IDdlCommandExecutor <|.. DdlCommandExecutor
IDdlCommandExecutor --> IDdlCommand : invokes
IDdlRequestSource --> IDdlCommandExecutor : submits

DdlCommandTemplate --> ICatalogManager : persists metadata
DdlCommandTemplate --> IMetadataTransactionPort : controls transaction
DdlCommandTemplate --> IMetadataEventCollector : records events
DdlCommandTemplate --> MetadataEventCommitDispatcher : dispatches after commit
DdlCommandTemplate --> MetadataChangeContext : uses
DdlCommandTemplate --> MetadataEvent : creates
DdlCommandTemplate --> ICatalogComponent : processes
DdlCommandTemplate --> DdlResult : returns

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

DdlResult --> ICatalogComponent : affected object

%% =====================================================
%% 10. CATALOG COORDINATION
%% Không biểu diễn Repository Pattern hoặc persistence implementation.
%% =====================================================

class ICatalogManager {
    <<Subsystem Interface>>
    +Register(component : ICatalogComponent, context : MetadataChangeContext)
    +Update(component : ICatalogComponent, context : MetadataChangeContext)
    +Remove(component : ICatalogComponent, context : MetadataChangeContext)
    +GetDatabase(name : string) Database
    +GetSchema(databaseId : int, name : string) Schema
    +GetTable(schemaId : int, name : string) Table
    +ObjectExists(parent : ICatalogComposite, name : string) bool
}

class CatalogManager {
    <<Subsystem>>
    +Register(component : ICatalogComponent, context : MetadataChangeContext)
    +Update(component : ICatalogComponent, context : MetadataChangeContext)
    +Remove(component : ICatalogComponent, context : MetadataChangeContext)
    +GetDatabase(name : string) Database
    +GetSchema(databaseId : int, name : string) Schema
    +GetTable(schemaId : int, name : string) Table
    +ObjectExists(parent : ICatalogComposite, name : string) bool
}

ICatalogManager <|.. CatalogManager
ICatalogManager --> ICatalogComponent : manages
ICatalogManager --> ICatalogComposite : searches

%% =====================================================
%% 11. OBSERVER — METADATA EVENTS
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
    +ObjectName : string
    +Context : MetadataChangeContext
    +PreviousSnapshot : MetadataSnapshot
    +CurrentSnapshot : MetadataSnapshot
}

class MetadataSnapshot {
    <<Event Data>>
    +Properties : IReadOnlyDictionary~string, object~
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
    -auditStore : IMetadataAuditStore
    +OnMetadataChanged(event : MetadataEvent)
}

class ICatalogCache {
    <<Cache Port>>
    +Get(name : string) ICatalogComponent
    +Set(component : ICatalogComponent)
    +Remove(name : string)
    +InvalidateChildren(parentName : string)
}

class IMetadataStatisticsStore {
    <<Statistics Port>>
    +IncrementObjectCount()
    +DecrementObjectCount()
    +RecordModification(timestamp : DateTime)
}

class IMetadataAuditStore {
    <<Audit Port>>
    +Append(event : MetadataEvent)
}

IMetadataEventCollector <|.. MetadataEventCollector
IMetadataEventPublisher <|.. MetadataEventPublisher
IMetadataObserver <|.. CatalogCacheObserver
IMetadataObserver <|.. MetadataStatisticsObserver
IMetadataObserver <|.. MetadataAuditObserver
MetadataEventCollector o-- "*" MetadataEvent : pending
MetadataEventCommitDispatcher --> IMetadataEventCollector : reads or clears
MetadataEventCommitDispatcher --> IMetadataEventPublisher : publishes after commit
MetadataEventPublisher o-- "*" IMetadataObserver : notifies
MetadataEventPublisher --> MetadataEvent : publishes
IMetadataObserver --> MetadataEvent : receives
CatalogCacheObserver --> ICatalogCache
MetadataStatisticsObserver --> IMetadataStatisticsStore
MetadataAuditObserver --> IMetadataAuditStore
MetadataEvent --> MetadataEventType
MetadataEvent --> MetadataChangeContext
MetadataEvent --> MetadataSnapshot
ICatalogCache --> ICatalogComponent : caches
IMetadataAuditStore --> MetadataEvent : stores

%% =====================================================
%% 12. ITERATOR — METADATA TRAVERSAL
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
    +MoveNext() bool
    +Current : T
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
%% 13. VISITOR — METADATA UTILITY
%% Adds metadata operations without modifying catalog objects.
%% =====================================================

class IMetadataVisitor {
    <<Visitor>>
    +VisitDatabase(database : Database)
    +VisitSchema(schema : Schema)
    +VisitTable(table : Table)
    +VisitColumn(column : Column)
    +VisitConstraint(constraint : Constraint)
    +VisitIndex(index : Index)
    +VisitPartition(partition : Partition)
    +VisitTrigger(trigger : Trigger)
    +VisitView(view : View)
    +VisitStoredProcedure(procedure : StoredProcedure)
    +VisitSequence(sequence : Sequence)
}

class DdlExportVisitor {
    <<Concrete Visitor>>
    -ddl : StringBuilder
    +VisitDatabase(database : Database)
    +VisitSchema(schema : Schema)
    +VisitTable(table : Table)
    +VisitColumn(column : Column)
    +VisitConstraint(constraint : Constraint)
    +VisitIndex(index : Index)
    +VisitPartition(partition : Partition)
    +VisitTrigger(trigger : Trigger)
    +VisitView(view : View)
    +VisitStoredProcedure(procedure : StoredProcedure)
    +VisitSequence(sequence : Sequence)
    +GetResult() string
}

class DependencyScanVisitor {
    <<Concrete Visitor>>
    -dependencies : List~MetadataDependency~
    +VisitDatabase(database : Database)
    +VisitSchema(schema : Schema)
    +VisitTable(table : Table)
    +VisitColumn(column : Column)
    +VisitConstraint(constraint : Constraint)
    +VisitIndex(index : Index)
    +VisitPartition(partition : Partition)
    +VisitTrigger(trigger : Trigger)
    +VisitView(view : View)
    +VisitStoredProcedure(procedure : StoredProcedure)
    +VisitSequence(sequence : Sequence)
    +GetDependencies() IReadOnlyCollection~MetadataDependency~
}

class MetadataDependency {
    <<Visitor Result>>
    +SourceName : string
    +TargetName : string
    +DependencyType : MetadataDependencyType
}

IMetadataVisitor <|.. DdlExportVisitor
IMetadataVisitor <|.. DependencyScanVisitor

ICatalogComponent ..> IMetadataVisitor : accepts
DdlExportVisitor --> ICatalogComponent : visits
DependencyScanVisitor --> ICatalogComponent : scans
DependencyScanVisitor --> MetadataDependency : creates
MetadataDependency --> MetadataDependencyType

%% =====================================================
%% 14. TEMPLATE METHOD — DDL SCRIPT GENERATION
%% Defines a fixed Generate() skeleton; subclasses customize
%% BuildHeader() and BuildBody() per DDL object type.
%% =====================================================

class DdlScriptGenerator {
    <<Abstract Template>>
    +Generate() string
    #BuildHeader() string*
    #BuildBody() string*
    #BuildFooter() string
}

class CreateTableScriptGenerator {
    <<Concrete Template>>
    -table : Table
    #BuildHeader() string
    #BuildBody() string
}

class AlterTableScriptGenerator {
    <<Concrete Template>>
    -table : Table
    -operation : TableAlterOperation
    #BuildHeader() string
    #BuildBody() string
}

class DropTableScriptGenerator {
    <<Concrete Template>>
    -tableName : string
    -cascade : bool
    #BuildHeader() string
    #BuildBody() string
}

class CreateSchemaScriptGenerator {
    <<Concrete Template>>
    -schema : Schema
    #BuildHeader() string
    #BuildBody() string
}

DdlScriptGenerator <|-- CreateTableScriptGenerator
DdlScriptGenerator <|-- AlterTableScriptGenerator
DdlScriptGenerator <|-- DropTableScriptGenerator
DdlScriptGenerator <|-- CreateSchemaScriptGenerator

CreateTableScriptGenerator --> Table : reads
AlterTableScriptGenerator --> Table : reads
AlterTableScriptGenerator --> TableAlterOperation : reads
DropTableScriptGenerator --> Schema : references

%% =====================================================
%% 15. SUPPORTING TYPES
%% =====================================================

class TableAlterOperation {
    <<Command Data>>
    +Type : TableAlterType
    +Payload : object
}

class ViewDefinition {
    <<DTO>>
    +Name : string
    +QueryDefinition : string
}

class ProcedureDefinition {
    <<DTO>>
    +Name : string
    +Body : string
}

class SequenceDefinition {
    <<DTO>>
    +Name : string
    +StartValue : long
    +Increment : long
}

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

class ValidationOperation {
    <<enumeration>>
    INSERT
    UPDATE
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

class MetadataEventType {
    <<enumeration>>
    CREATED
    UPDATED
    RENAMED
    REMOVED
}

class MetadataDependencyType {
    <<enumeration>>
    CONTAINS
    REFERENCES
    DEPENDS_ON
    INDEXES
    TRIGGERS
}

TableAlterOperation --> TableAlterType
SchemaService --> TableAlterOperation
SchemaService --> ViewDefinition
SchemaService --> ProcedureDefinition
SchemaService --> SequenceDefinition
```

## Visual Summary Database Server & Database Lifecycle

| Priority | Module | Main Feature | Main Classes | Application | Design Pattern | Progress |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
| 🔥 Critical | Server Management | Server Lifecycle | `DatabaseServer` | Provides a unified interface for starting, stopping, restarting, and recovering the database server. | **Facade** | Completed |
| 🔥 Critical | Server Management | Server State Management | `DatabaseServer`, `IServerState` | Encapsulates behaviors for Stopped, Running, Recovering, and Failed states. | **State** | Completed |
| 🔴 High | Database Management | Database Creation | `IDatabaseFactory`, `DatabaseFactory` | Centralizes the construction and initialization of database objects. | **Factory Method** | Completed |
| 🔴 High | Database Management | Global Database Management | `DatabaseManager` | Ensures that only one database manager coordinates database lifecycle operations, catalog metadata, and database connections within the server process. | **Singleton** | Completed |
| 🔴 High | Database Management | Database Operations | `IDatabaseCommand`, `CreateDatabaseCommand`, `DropDatabaseCommand`, `RenameDatabaseCommand` | Encapsulates database creation, deletion, and renaming requests as command objects, enabling centralized execution, auditing, logging, retrying, scheduling, and operation history. | **Command** | Completed |
| 🟡 Medium | Database Management | Database State | `Database`, `IDatabaseState` | Controls database behavior in Online, Offline, ReadOnly, and Restoring states. | **State** | Not Started |
| 🟡 Medium | Configuration | Configuration Loading | `ConfigurationManager`, `IConfigurationLoader` | Supports loading configuration from JSON, XML, environment variables, or command-line sources. | **Strategy** | Not Started |
| 🟡 Medium | Monitoring | Metrics Collection | `MonitoringManager`, `IMetricCollector` | Separates CPU, memory, query, transaction, and connection metric collection. | **Strategy** | Not Started |
| 🟡 Medium | Monitoring | Runtime Event Monitoring | `MonitoringManager`, event publishers | Receives query, transaction, connection, and error events from server components. | **Observer** | Not Started |
| 🟡 Medium | Configuration | Dynamic Configuration | `ConfigurationManager`, configuration observers | Notifies dependent components when configuration values change. | **Observer** | Not Started |
| 🟢 Low | Monitoring | Metrics Export | `PrometheusMetricsAdapter`, `OpenTelemetryAdapter` | Converts internal server metrics into external monitoring formats. | **Adapter** | Not Started |

## Visual Summary Query Processor
| Priority | Module | Main Feature | Main Classes | Application | Design Pattern | Progress |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
| 🔥 Critical | Query Processing | SQL Parsing | `SQLParser`, `Lexer`, `ASTNode` | Parses SQL grammar and represents SQL statements as an abstract syntax tree. | **Interpreter** | Completed |
| 🔥 Critical | Query Optimization | Optimization Algorithm | `QueryOptimizer`, `IOptimizationStrategy` | Allows rule-based, cost-based, and heuristic optimization algorithms to be selected independently. | **Strategy** | Not Started |
| 🔴 High | Query Optimization | Optimization Rules | `IOptimizationRule`, `OptimizationRulePipeline`, concrete rules | Applies predicate pushdown, projection pruning, constant folding, and other transformations sequentially until the logical plan reaches a stable form. | **Chain of Responsibility** | Completed |
| 🔴 High | Query Processing | Query Plan Structure | `PlanOperator`, logical and physical operators | Represents AST and query plans as tree structures where leaf and composite operators are treated uniformly. | **Composite** | Not Started |
| 🔴 High | Query Execution | Streaming Execution | `IPhysicalOperator`, `ResultCursor` | Produces rows incrementally through `Open`, `Next`, and `Close` operations. | **Iterator** | Not Started |
| 🔴 High | Query Processing | Unified Query API | `QueryProcessor` | Provides one interface for parsing, binding, optimizing, and executing SQL queries. | **Facade** | Not Started |
| 🔴 High | Query Processing | Plan Traversal | `IPlanVisitor`, AST and plan operators | Traverses AST and plan trees for binding, validation, cost calculation, and explain-plan generation. | **Visitor** | Not Started |
| 🟡 Medium | Query Planning | Physical Operator Creation | `PhysicalOperatorFactory` | Creates scan, join, filter, sort, and aggregate physical operators based on optimizer decisions. | **Factory Method** | Not Started |
| 🟡 Medium | Query Processing | Standard Query Workflow | `QueryProcessingTemplate` | Defines the fixed parsing, binding, optimization, execution, and cleanup workflow. | **Template Method** | Not Started |
| 🟡 Medium | Statistics | Statistics Invalidation | `StatisticsManager`, data-change publishers | Invalidates or refreshes table statistics when underlying data or indexes change. | **Observer** | Not Started |

## Visual Summary Storage Engine

| Priority | Module | Main Feature | Main Classes | Application | Design Pattern | Progress |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
| 🔥 Critical | Buffer Management | Cached Page Access | `IPageStore`, `BufferPoolProxy`, `DiskPageStore`, `BufferFrame` | Provides the same page-access interface as disk storage, serves memory-resident pages on cache hits, and delegates cache misses or flushes to the underlying disk page store. | **Proxy** | Completed |
| 🔥 Critical | Storage Engine | Unified Storage Access | `StorageEngine`, `BufferPool`, `FileManager`, `WALManager` | Provides a unified interface for page allocation, reading, writing, caching, logging, and recovery. | **Facade** | Not Started |
| 🔥 Critical | Buffer Management | Page Replacement | `BufferPool`, `IReplacementPolicy`, `LRUReplacementPolicy`, `ClockReplacementPolicy` | Allows the buffer pool to select and replace page eviction algorithms independently. | **Strategy** | Not Started |
| 🔴 High | Recovery | Recoverable Storage Operations | `LogRecord`, concrete log records, `WALManager`, `RecoveryManager` | Encapsulates storage changes as records that support redo and undo during recovery. | **Command** | Not Started |
| 🔴 High | Page Management | Page Creation | `PageFactory`, `DataPage`, `IndexPage`, `MetadataPage` | Creates page implementations based on their storage purpose and page type. | **Factory Method** | Not Start |
| 🔴 High | Page Management | Standard Page Modification Workflow | `Page`, concrete page types | Defines a common process for validating records, checking capacity, modifying slots, updating metadata, and marking pages dirty. | **Template Method** | Not Start |
| 🟡 Medium | Record Management | Page Record Traversal | `IRecordIterator`, `PageRecordIterator`, `SlottedPage` | Traverses valid records without exposing slot-array and record-offset details. | **Iterator** | Not Start |
| 🟡 Medium | File Management | Cross-Cutting I/O Features | `IFileManager`, file manager decorators | Adds logging, metrics, checksum, encryption, or tracing without modifying the base file manager. | **Decorator** | Not Start |


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
    +Reset(name : string)
    +AddColumn(column : Column)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
    +Build() Table
    -ValidateBeforeBuild()
}

class TableDirector {
    <<Director>>
    -builder : ITableBuilder
    -columnFactory : IColumnFactory
    -constraintFactory : IConstraintFactory
    -indexFactory : IIndexFactory
    +Construct(definition : TableDefinition) Table
    -CreatePartition(options : PartitionOptions) Partition
    -CreateTrigger(options : TriggerOptions) Trigger
}

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

class TableBuildContext {
    <<Build Context>>
    +TableName : string
    +Columns : IReadOnlyCollection~Column~
    +FindColumn(name : string) Column
}

class Table {
    <<Product>>
}

class Column
class Constraint
class Index
class Partition
class Trigger

class IColumnFactory {
    <<Collaborator>>
    +Create(definition : ColumnDefinition) Column
}

class IConstraintFactory {
    <<Collaborator>>
    +Create(options : ConstraintOptions, context : TableBuildContext) Constraint
}

class IIndexFactory {
    <<Collaborator>>
    +Create(options : IndexOptions, context : TableBuildContext) Index
}



class ColumnDefinition
class ConstraintOptions
class IndexOptions
class PartitionOptions
class TriggerOptions
class DefinitionValidationResult

ITableBuilder <|.. TableBuilder
TableBuilder --> Table : builds

TableDirector --> ITableBuilder : directs
TableDirector --> TableDefinition : reads
TableDirector ..> TableBuildContext : creates

TableDirector --> IColumnFactory
TableDirector --> IConstraintFactory
TableDirector --> IIndexFactory

TableDefinition *-- ColumnDefinition
TableDefinition *-- ConstraintOptions
TableDefinition *-- IndexOptions
TableDefinition *-- PartitionOptions
TableDefinition *-- TriggerOptions

Table "1" *-- "*" Column
Table "1" *-- "*" Constraint
Table "1" *-- "*" Index
Table "1" *-- "*" Partition
Table "1" *-- "*" Trigger
```

```mermaid
sequenceDiagram
    autonumber

    participant Caller
    participant Definition as TableDefinition
    participant Director as TableDirector
    participant Builder as ITableBuilder
    participant ColFactory as IColumnFactory
    participant CFactory as IConstraintFactory
    participant IFactory as IIndexFactory

    Caller->>Definition: Validate()
    Definition-->>Caller: validationResult

    alt Definition is invalid
        Caller-->>Caller: throw InvalidTableDefinitionException
    else Definition is valid
        Caller->>Director: Construct(definition)
        activate Director

        Director->>Builder: Reset(definition.Name)

        loop Each ColumnDefinition
            Director->>ColFactory: Create(columnDefinition)
            ColFactory-->>Director: Column
            Director->>Builder: AddColumn(column)
        end

        Director->>Director: Create TableBuildContext(columns)

        loop Each ConstraintOptions
            Director->>CFactory: Create(options, context)
            CFactory-->>Director: Constraint
            Director->>Builder: AddConstraint(constraint)
        end

        loop Each IndexOptions
            Director->>IFactory: Create(options, context)
            IFactory-->>Director: Index
            Director->>Builder: AddIndex(index)
        end

        loop Each PartitionOptions
            Director->>Director: CreatePartition(options)
            Director->>Builder: AddPartition(partition)
        end

        loop Each TriggerOptions
            Director->>Director: CreateTrigger(options)
            Director->>Builder: AddTrigger(trigger)
        end

        Director->>Builder: Build()
        Builder->>Builder: ValidateBeforeBuild()
        Builder-->>Director: Table

        Director-->>Caller: Table
        deactivate Director
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
}

class SchemaService {
    <<Facade>>

    -catalog : ICatalogManager
    -dependencyService : ICatalogDependencyService
    -tableDirector : TableDirector
    -storagePort : IStorageObjectPort
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
DatabaseService --> Database

SchemaService --> ICatalogManager
SchemaService --> ICatalogDependencyService
SchemaService --> TableDirector
SchemaService --> IStorageObjectPort

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

### 8. Metadata Events (Observer Pattern)

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

### 9. DDL Script Generation (Template Pattern)

**Purpose:**
Define the skeleton of an algorithm in a base class while allowing subclasses to customize specific steps without changing the overall process.

**Example:**
A document generator follows the same process for creating reports, but PDF and HTML reports implement formatting differently.

#### Class Diagram

```mermaid
classDiagram
direction LR

class ReportGenerator {
    <<Abstract Class>>
    +GenerateReport()
    #LoadData()
    #FormatData()*
    #ExportReport()*
}

class PdfReportGenerator {
    <<Concrete Class>>
    #FormatData()
    #ExportReport()
}

class HtmlReportGenerator {
    <<Concrete Class>>
    #FormatData()
    #ExportReport()
}

ReportGenerator <|-- PdfReportGenerator
ReportGenerator <|-- HtmlReportGenerator
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant Generator as PdfReportGenerator

    Client->>Generator: GenerateReport()
    Generator->>Generator: LoadData()
    Generator->>Generator: FormatData()
    Generator->>Generator: ExportReport()
    Generator-->>Client: Report completed
```

#### Simplified Code

```csharp
public abstract class ReportGenerator
{
    // Template Method: define the fixed algorithm sequence
    public void GenerateReport()
    {
        LoadData();
        FormatData();
        ExportReport();
    }

    // Common step shared by all report types
    protected void LoadData()
    {
        // Load report data from the data source
    }

    // Allow subclasses to define how data is formatted
    protected abstract void FormatData();

    // Allow subclasses to define how the report is exported
    protected abstract void ExportReport();
}

public class PdfReportGenerator : ReportGenerator
{
    protected override void FormatData()
    {
        // Format data for a PDF document
    }

    protected override void ExportReport()
    {
        // Export the report as a PDF file
    }
}

public class HtmlReportGenerator : ReportGenerator
{
    protected override void FormatData()
    {
        // Format data as HTML content
    }

    protected override void ExportReport()
    {
        // Export the report as an HTML file
    }
}
```

**Benefits**

* Reuses common algorithm steps in the base class.
* Keeps the overall workflow consistent.
* Allows subclasses to customize specific steps.
* Reduces duplicated code between similar processes.
* Applies the Hollywood Principle: the base class controls when subclass methods are called.

**Application:** `DDLCommandTemplate` and concrete classes like `CreateTableCommand`, `AlterTableCommand`, and `DropTableCommand`.

**Why apply?** The Template Method Pattern provides a fixed execution workflow for DDL operations. The base template class defines the overarching skeleton—such as validating permissions, starting a transaction, applying the specific metadata change, persisting the catalog, and publishing events. Concrete command subclasses implement only the specific metadata logic (e.g., how to create or alter a table), ensuring that the complex, overarching lifecycle is consistently enforced without duplicating code across every DDL command.

```mermaid
classDiagram
direction TB

class DdlScriptGenerator {
    <<abstract Template>>
    +Generate() string
    #BuildHeader() string*
    #BuildBody() string*
    #BuildFooter() string
}

class CreateTableScriptGenerator {
    <<Concrete Template>>
    -table : Table
    +CreateTableScriptGenerator(table : Table)
    #BuildHeader() string
    #BuildBody() string
}

class AlterTableScriptGenerator {
    <<Concrete Template>>
    -table : Table
    -operation : TableAlterOperation
    +AlterTableScriptGenerator(table : Table, operation : TableAlterOperation)
    #BuildHeader() string
    #BuildBody() string
}

class DropTableScriptGenerator {
    <<Concrete Template>>
    -tableName : string
    -cascade : bool
    +DropTableScriptGenerator(tableName : string, cascade : bool)
    #BuildHeader() string
    #BuildBody() string
}

class CreateSchemaScriptGenerator {
    <<Concrete Template>>
    -schema : Schema
    +CreateSchemaScriptGenerator(schema : Schema)
    #BuildHeader() string
    #BuildBody() string
}

class Table {
    +TableId : int
    +Name : string
    +Parent : Schema
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
}

class TableAlterOperation {
    <<Command Data>>
    +Type : TableAlterType
    +Definition : object
}

DdlScriptGenerator <|-- CreateTableScriptGenerator
DdlScriptGenerator <|-- AlterTableScriptGenerator
DdlScriptGenerator <|-- DropTableScriptGenerator
DdlScriptGenerator <|-- CreateSchemaScriptGenerator

CreateTableScriptGenerator --> Table : reads
AlterTableScriptGenerator --> Table : reads
AlterTableScriptGenerator --> TableAlterOperation : reads
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Generator as CreateTableScriptGenerator
    participant Table
    participant Column
    participant Constraint
    participant Index

    Client->>Generator: new CreateTableScriptGenerator(table)
    Client->>Generator: Generate()

    Note over Generator: Template Method starts

    Generator->>Generator: BuildHeader()
    Generator->>Table: Name, Parent.Name
    Table-->>Generator: schema and table name

    Generator->>Generator: BuildBody()

    loop Each Column
        Generator->>Column: Name, DataType, Nullable, DefaultValue
        Column-->>Generator: column definition fragment
    end

    loop Each Constraint
        Generator->>Constraint: Name, Type, Columns
        Constraint-->>Generator: constraint definition fragment
    end

    loop Each Index
        Generator->>Index: Name, Columns, Unique
        Index-->>Generator: index definition fragment
    end

    Generator->>Generator: BuildFooter()
    Note over Generator: Shared base returns

    Generator-->>Client: Complete CREATE TABLE DDL script
```

### 10. Metadata Utility (Visitor Pattern)

```mermaid
classDiagram
direction LR

class IMetadataElement {
    <<Element>>
    +Accept(visitor : IMetadataVisitor) void
}

class IMetadataVisitor {
    <<Visitor>>
    +VisitDatabase(database : Database) void
    +VisitSchema(schema : Schema) void
    +VisitTable(table : Table) void
    +VisitColumn(column : Column) void
    +VisitConstraint(constraint : Constraint) void
    +VisitIndex(index : Index) void
}

class Database {
    <<Concrete Element>>
    +Accept(visitor : IMetadataVisitor) void
}

class Schema {
    <<Concrete Element>>
    +Accept(visitor : IMetadataVisitor) void
}

class Table {
    <<Concrete Element>>
    +Accept(visitor : IMetadataVisitor) void
}

class Column {
    <<Concrete Element>>
    +Accept(visitor : IMetadataVisitor) void
}

class Constraint {
    <<Concrete Element>>
    +Accept(visitor : IMetadataVisitor) void
}

class Index {
    <<Concrete Element>>
    +Accept(visitor : IMetadataVisitor) void
}

class DDLExportVisitor {
    <<Concrete Visitor>>
    -ddl : StringBuilder
    +VisitDatabase(database : Database) void
    +VisitSchema(schema : Schema) void
    +VisitTable(table : Table) void
    +VisitColumn(column : Column) void
    +VisitConstraint(constraint : Constraint) void
    +VisitIndex(index : Index) void
    +GetResult() string
}

class DependencyScanVisitor {
    <<Concrete Visitor>>
    -dependencies : List~MetadataDependency~
    +VisitDatabase(database : Database) void
    +VisitSchema(schema : Schema) void
    +VisitTable(table : Table) void
    +VisitColumn(column : Column) void
    +VisitConstraint(constraint : Constraint) void
    +VisitIndex(index : Index) void
    +GetDependencies() IReadOnlyCollection~MetadataDependency~
}

class MetadataDependency {
    +SourceName : string
    +TargetName : string
    +DependencyType : string
}

IMetadataElement <|.. Database
IMetadataElement <|.. Schema
IMetadataElement <|.. Table
IMetadataElement <|.. Column
IMetadataElement <|.. Constraint
IMetadataElement <|.. Index

IMetadataVisitor <|.. DDLExportVisitor
IMetadataVisitor <|.. DependencyScanVisitor

Database ..> IMetadataVisitor : accepts
Schema ..> IMetadataVisitor : accepts
Table ..> IMetadataVisitor : accepts
Column ..> IMetadataVisitor : accepts
Constraint ..> IMetadataVisitor : accepts
Index ..> IMetadataVisitor : accepts

DependencyScanVisitor --> MetadataDependency : creates
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Table as Table
    participant Visitor as DDLExportVisitor
    participant Column as Column
    participant Constraint as Constraint
    participant Index as Index

    Client->>Visitor: new DDLExportVisitor()
    Client->>Table: Accept(visitor)

    Table->>Visitor: VisitTable(this)
    Visitor->>Visitor: Append CREATE TABLE

    loop Each Column
        Table->>Column: Accept(visitor)
        Column->>Visitor: VisitColumn(this)
        Visitor->>Visitor: Append column definition
    end

    loop Each Constraint
        Table->>Constraint: Accept(visitor)
        Constraint->>Visitor: VisitConstraint(this)
        Visitor->>Visitor: Append constraint definition
    end

    loop Each Index
        Table->>Index: Accept(visitor)
        Index->>Visitor: VisitIndex(this)
        Visitor->>Visitor: Append index definition
    end

    Client->>Visitor: GetResult()
    Visitor-->>Client: CREATE TABLE DDL
```

### 11. System Initialization (Facade Pattern)

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

### 12. Server State Management (State Pattern)

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

### 13. Global Database Management (Singleton Pattern)

**Purpose:**  
Ensures that only one database manager coordinates database lifecycle operations, catalog metadata, and database connections within the server process.

**Example:**  
A simple configuration manager that ensures only one instance loads and holds global application settings throughout the process lifetime — any component that requests it gets the exact same object.

#### Class Diagram

```mermaid
classDiagram
direction LR

class Singleton {
    <<Singleton>>
    -_instance : Singleton$
    -_lock : object$
    -Singleton()
    +GetInstance()$ Singleton
    +Operation()
}

class Client {
    +DoWork()
}

Client ..> Singleton : GetInstance()
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    autonumber

    actor Client1 as Client 1
    actor Client2 as Client 2
    participant S as Singleton

    Note over S: _instance = null

    Client1->>S: GetInstance()
    S->>S: _instance == null? → true
    S->>S: lock acquired
    S->>S: new Singleton()
    S->>S: _instance = instance
    S->>S: lock released
    S-->>Client1: Singleton instance

    Client2->>S: GetInstance()
    S->>S: _instance == null? → false (fast path)
    S-->>Client2: same Singleton instance

    Note over Client1,Client2: Both share the identical instance
```

#### Simplified Code

```csharp
public sealed class Singleton
{
    private static volatile Singleton? _instance;
    private static readonly object _lock = new();

    // Private constructor prevents external instantiation
    private Singleton() { }

    // Double-checked locking — thread-safe lazy initialization
    public static Singleton GetInstance()
    {
        if (_instance is null)
        {
            lock (_lock)
            {
                if (_instance is null)
                    _instance = new Singleton();
            }
        }
        return _instance;
    }

    public void Operation() { /* ... */ }
}
```

**Benefits**

- Prevents concurrent database access issues by serializing operations through a single point of control.
- Simplifies configuration management by providing a global access point for database settings.
- Enables thread-safe access to shared resources like the catalog and connection pool.

**Application:** `DatabaseManager` is implemented as a Singleton so the `DatabaseServer` and all subsystems (StorageEngine, QueryProcessor, TransactionManager) always resolve to the same coordinating instance.

**Why apply?** In a DBMS, every component that needs to open, close, create, or drop a database must go through exactly the same coordinator. If multiple `DatabaseManager` instances could exist, two concurrent `CreateDatabase("orders")` calls issued through different instances would bypass each other's duplicate-name check, and two simultaneous `DropDatabase` calls could race to delete the same catalog entry. Implementing `DatabaseManager` as a Singleton guarantees a single authoritative source of truth for database lifecycle state, eliminates split-brain catalog views, and lets the `IConnectionPool` it holds be shared safely across every session handler in the server process.

```mermaid
classDiagram
direction LR

class DatabaseManager {
    <<Singleton>>
    -_instance : DatabaseManager$
    -_lock : object$
    -_catalog : ICatalogManager
    -_connectionPool : IConnectionPool
    -_databaseFactory : IDatabaseFactory
    -DatabaseManager(catalog, connectionPool, factory)
    +Initialize(catalog, connectionPool, factory)$ DatabaseManager
    +Instance : DatabaseManager$
    +CreateDatabase(name : string) Database
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

class DatabaseServer {
    <<Context>>
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

class ConfigurationManager {
    -configData : Map~string, string~
    +LoadConfiguration(filePath : string)
    +UpdateConfiguration(key : string, value : string)
    +GetConfiguration(key : string) string
}

class MonitoringManager {
    -metrics : ServerMetrics
    +CollectMetrics()
    +GetMetrics() ServerMetrics
}

class ICatalogManager {
    <<Catalog Port>>
    +RegisterDatabase(database : Database)
    +RemoveDatabase(name : string)
    +GetDatabase(name : string) Database
    +ListDatabases() IEnumerable~Database~
    +CheckExists(name : string) bool
}

class IConnectionPool {
    <<Connection Port>>
    +AcquireConnection(dbName : string) IDbConnection
    +ReleaseConnection(connection : IDbConnection)
    +CloseAll(dbName : string)
}

class IDatabaseFactory {
    <<Creator>>
    +Create(name : string) Database
    +Attach(name : string, filePath : string) Database
}

class Database {
    +DatabaseId : int
    +Name : string
    +State : DatabaseState
    +Open()
    +Close()
    +Rename(newName : string)
    +ChangeState(state : DatabaseState)
}

DatabaseServer --> DatabaseManager : Initialize() / Instance
DatabaseServer --> ConfigurationManager
DatabaseServer --> MonitoringManager

DatabaseManager --> ICatalogManager : manages metadata
DatabaseManager --> IConnectionPool : manages sessions
DatabaseManager --> IDatabaseFactory : creates databases
DatabaseManager --> Database : lifecycle
```

```mermaid
sequenceDiagram
    autonumber

    actor Admin
    participant Server as DatabaseServer
    participant DBM as DatabaseManager
    participant Lock as _lock (Monitor)
    participant Factory as IDatabaseFactory
    participant Catalog as ICatalogManager

    Note over DBM: _instance = null (process start)

    Admin->>Server: Start(safeMode)
    Server->>DBM: Initialize(catalog, pool, factory)

    DBM->>Lock: Acquire lock
    Lock-->>DBM: acquired
    DBM->>DBM: _instance == null? → true
    DBM->>DBM: new DatabaseManager(catalog, pool, factory)
    DBM->>DBM: _instance = new instance
    DBM->>Lock: Release lock
    DBM-->>Server: DatabaseManager singleton

    Note over Server,DBM: Server is Running — singleton in use

    Admin->>DBM: CreateDatabase("shop_db")
    DBM->>Catalog: CheckExists("shop_db")
    Catalog-->>DBM: false

    DBM->>Factory: Create("shop_db")
    Factory-->>DBM: Database

    DBM->>Catalog: RegisterDatabase(database)
    Catalog-->>DBM: registered
    DBM-->>Admin: Database created

    Note over DBM: Later — QueryProcessor also requests the manager

    participant QP as QueryProcessor
    QP->>DBM: Instance
    DBM->>DBM: _instance == null? → false (fast path — no lock)
    DBM-->>QP: same DatabaseManager instance

    QP->>DBM: GetDatabase("shop_db")
    DBM->>Catalog: GetDatabase("shop_db")
    Catalog-->>DBM: Database
    DBM-->>QP: Database
```

### 14. Database Operations (Command Pattern)

**Purpose:**  
Encapsulate a request as an object, thereby allowing you to parameterize clients with different requests, queue or log requests, and support undoable operations.

**Example:**  
A simple text editor where user actions (type, delete, bold) are wrapped as command objects so the editor can undo/redo each step without knowing the implementation details of each action.

#### Class Diagram

```mermaid
classDiagram
direction LR

class ICommand {
    <<Command>>
    +Execute()
    +Undo()
}

class TypeTextCommand {
    <<ConcreteCommand>>
    -_editor : TextEditor
    -_text : string
    +Execute()
    +Undo()
}

class DeleteTextCommand {
    <<ConcreteCommand>>
    -_editor : TextEditor
    -_count : int
    +Execute()
    +Undo()
}

class TextEditor {
    <<Receiver>>
    +Type(text : string)
    +Delete(count : int)
}

class CommandInvoker {
    <<Invoker>>
    -_history : Stack~ICommand~
    +Execute(command : ICommand)
    +Undo()
}

ICommand <|.. TypeTextCommand
ICommand <|.. DeleteTextCommand

TypeTextCommand --> TextEditor : receiver
DeleteTextCommand --> TextEditor : receiver

CommandInvoker --> ICommand : executes
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    autonumber

    actor User
    participant Invoker as CommandInvoker
    participant Cmd as TypeTextCommand
    participant Editor as TextEditor

    User->>Invoker: Execute(new TypeTextCommand("Hello"))
    Invoker->>Cmd: Execute()
    Cmd->>Editor: Type("Hello")
    Editor-->>Cmd: done
    Invoker->>Invoker: Push command to history stack
    Invoker-->>User: Success

    User->>Invoker: Undo()
    Invoker->>Invoker: Pop command from history stack
    Invoker->>Cmd: Undo()
    Cmd->>Editor: Delete(5)
    Editor-->>Cmd: done
    Invoker-->>User: Undone
```

#### Simplified Code

```csharp
public interface ICommand
{
    void Execute();
    void Undo();
}

public class TypeTextCommand : ICommand
{
    private readonly TextEditor _editor;
    private readonly string _text;

    public TypeTextCommand(TextEditor editor, string text)
    {
        _editor = editor;
        _text = text;
    }

    public void Execute() => _editor.Type(_text);
    public void Undo() => _editor.Delete(_text.Length);
}

public class CommandInvoker
{
    private readonly Stack<ICommand> _history = new();

    public void Execute(ICommand command)
    {
        command.Execute();
        _history.Push(command);
    }

    public void Undo()
    {
        if (_history.TryPop(out var command))
            command.Undo();
    }
}
```

**Benefits**

- Decouples the object that issues a request from the object that knows how to handle it.
- Supports undoable operations by storing command objects in a history stack.
- Enables logging, auditing, retrying, and scheduling of requests without modifying callers.
- Supports building macro-commands (composites of multiple commands).
- Follows Open/Closed Principle — new operations are added as new command classes.

**Application:** `IDatabaseCommand`, `CreateDatabaseCommand`, `DropDatabaseCommand`, `RenameDatabaseCommand`, and `DatabaseCommandExecutor` encapsulate database lifecycle operations as first-class objects within `DatabaseServer`.

**Why apply?** In a DBMS, admin operations such as `CREATE DATABASE`, `DROP DATABASE`, and `RENAME DATABASE` share a common lifecycle: permission check → validation → execution → audit log. Without the Command Pattern, this cross-cutting logic gets duplicated inside `DatabaseManager` or scattered across the API layer. Wrapping each operation as an `IDatabaseCommand` lets `DatabaseCommandExecutor` enforce uniform pre/post-processing, record a command history for auditing, and retry failed commands — all without changing `DatabaseManager`. It also separates *what* to do (the command) from *when* and *how often* to do it (the invoker or a scheduler), enabling features like deferred execution and operation replay.

```mermaid
classDiagram
direction LR

class IDatabaseCommand {
    <<Command>>
    +Execute() DatabaseCommandResult
}

class CreateDatabaseCommand {
    <<ConcreteCommand>>
    -_manager : DatabaseManager
    -_databaseName : string
    +CreateDatabaseCommand(manager : DatabaseManager, databaseName : string)
    +Execute() DatabaseCommandResult
}

class DropDatabaseCommand {
    <<ConcreteCommand>>
    -_manager : DatabaseManager
    -_databaseName : string
    -_cascade : bool
    +DropDatabaseCommand(manager : DatabaseManager, databaseName : string, cascade : bool)
    +Execute() DatabaseCommandResult
}

class RenameDatabaseCommand {
    <<ConcreteCommand>>
    -_manager : DatabaseManager
    -_oldName : string
    -_newName : string
    +RenameDatabaseCommand(manager : DatabaseManager, oldName : string, newName : string)
    +Execute() DatabaseCommandResult
}

class DatabaseCommandExecutor {
    <<Invoker>>
    -_history : List~IDatabaseCommand~
    +Execute(command : IDatabaseCommand) DatabaseCommandResult
    +GetHistory() IReadOnlyList~IDatabaseCommand~
}

class DatabaseCommandResult {
    <<Value Object>>
    +IsSuccess : bool
    +Message : string
    +Database : Database
    +ExecutedAt : DateTime
}

class DatabaseManager {
    <<Receiver / Singleton>>
    -_instance : DatabaseManager$
    -_catalog : ICatalogManager
    -_connectionPool : IConnectionPool
    -_databaseFactory : IDatabaseFactory
    +Instance : DatabaseManager$
    +Initialize(catalog, connectionPool, factory)$ DatabaseManager
    +CreateDatabase(name : string) Database
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

class DatabaseServer {
    <<Context>>
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

class ICatalogManager {
    <<Catalog Port>>
    +RegisterDatabase(database : Database)
    +RemoveDatabase(name : string)
    +GetDatabase(name : string) Database
    +CheckExists(name : string) bool
}

class IConnectionPool {
    <<Connection Port>>
    +CloseAll(dbName : string)
}

class IDatabaseFactory {
    <<Creator>>
    +Create(name : string) Database
}

class ConfigurationManager {
    -configData : Map~string, string~
    +LoadConfiguration(filePath : string)
    +GetConfiguration(key : string) string
}

class MonitoringManager {
    -metrics : ServerMetrics
    +CollectMetrics()
    +GetMetrics() ServerMetrics
}

IDatabaseCommand <|.. CreateDatabaseCommand
IDatabaseCommand <|.. DropDatabaseCommand
IDatabaseCommand <|.. RenameDatabaseCommand

CreateDatabaseCommand --> DatabaseManager : receiver
DropDatabaseCommand --> DatabaseManager : receiver
RenameDatabaseCommand --> DatabaseManager : receiver

DatabaseCommandExecutor --> IDatabaseCommand : executes
DatabaseCommandExecutor --> DatabaseCommandResult : returns

DatabaseServer --> DatabaseCommandExecutor : invokes via
DatabaseServer --> DatabaseManager : Initialize() / Instance
DatabaseServer --> ConfigurationManager
DatabaseServer --> MonitoringManager

DatabaseManager --> ICatalogManager : queries & updates
DatabaseManager --> IConnectionPool : manages sessions
DatabaseManager --> IDatabaseFactory : delegates construction
```

#### CreateDatabaseCommand

```mermaid
sequenceDiagram
    autonumber

    actor Admin
    participant Server as DatabaseServer
    participant Executor as DatabaseCommandExecutor
    participant Cmd as CreateDatabaseCommand
    participant Manager as DatabaseManager
    participant Catalog as ICatalogManager
    participant Factory as IDatabaseFactory

    Admin->>Server: CreateDatabase("SalesDB")

    Server->>Cmd: new CreateDatabaseCommand(DatabaseManager.Instance, "SalesDB")
    Server->>Executor: Execute(command)

    Executor->>Cmd: Execute()

    Cmd->>Manager: CreateDatabase("SalesDB")
    Manager->>Catalog: CheckExists("SalesDB")
    Catalog-->>Manager: false

    Manager->>Factory: Create("SalesDB")
    Factory-->>Manager: Database

    Manager->>Catalog: RegisterDatabase(database)
    Catalog-->>Manager: registered

    Manager-->>Cmd: Database
    Cmd-->>Executor: DatabaseCommandResult(IsSuccess=true)

    Executor->>Executor: Add command to history
    Executor-->>Server: DatabaseCommandResult
    Server-->>Admin: Database created successfully
```

#### DropDatabaseCommand

```mermaid
sequenceDiagram
    autonumber

    actor Admin
    participant Server as DatabaseServer
    participant Executor as DatabaseCommandExecutor
    participant Cmd as DropDatabaseCommand
    participant Manager as DatabaseManager
    participant Catalog as ICatalogManager
    participant Pool as IConnectionPool

    Admin->>Server: DropDatabase("SalesDB", cascade: true)

    Server->>Cmd: new DropDatabaseCommand(DatabaseManager.Instance, "SalesDB", cascade: true)
    Server->>Executor: Execute(command)

    Executor->>Cmd: Execute()
    Cmd->>Manager: DropDatabase("SalesDB", cascade: true)

    Manager->>Catalog: CheckExists("SalesDB")
    Catalog-->>Manager: true

    Manager->>Pool: CloseAll("SalesDB")
    Pool-->>Manager: connections closed

    Manager->>Catalog: RemoveDatabase("SalesDB")
    Catalog-->>Manager: removed

    Manager-->>Cmd: completed
    Cmd-->>Executor: DatabaseCommandResult(IsSuccess=true)

    Executor->>Executor: Add command to history
    Executor-->>Server: DatabaseCommandResult
    Server-->>Admin: Database dropped successfully
```

### 15. Database Creation (Factory Method Pattern)

**Purpose:**  
Define an interface for creating a `Database` object, but let subclasses (or concrete factory implementations) decide which class to instantiate. The Factory Method centralizes the complex initialization logic—allocating storage, registering the catalog entry, creating the default schema, and setting up permissions—so that `DatabaseManager` never has to know the construction details.

**Benefits**

- Decouples object creation from the client that uses the object.
- Follows the Open/Closed Principle: adding a new product only requires a new concrete factory.
- Centralizes construction logic — complex initialization stays inside the factory, not scattered across callers.
- Supports dependency injection and testability by programming to interfaces.

**Application:** `IDatabaseFactory` and `DatabaseFactory` centralize the construction and initialization of `Database` objects inside `DatabaseManager`.

**Why apply?** Creating a new database in a DBMS is not a simple `new Database()` call. It involves: allocating on-disk storage, registering the catalog entry, creating the default `public` schema, assigning ownership, and initializing access control. Without a factory, this multi-step logic would leak into `DatabaseManager.CreateDatabase()`, `AttachDatabase()`, and any other entry point that needs a `Database`. The Factory Method pattern moves all that initialization into `IDatabaseFactory.Create()`, giving `DatabaseManager` a single, clean call while remaining open to new database kinds (e.g., in-memory, read-only, or template databases) without modifying existing code.

```mermaid
classDiagram
direction LR

class IDatabaseFactory {
    <<Creator>>
    +Create(options : DatabaseCreationOptions) Database
}

class DatabaseFactory {
    <<ConcreteCreator>>
    -_catalog : ICatalogManager
    -_storageEngine : IStorageEngine
    -_securityManager : ISecurityManager
    +DatabaseFactory(catalog, storage, security)
    +Create(options : DatabaseCreationOptions) Database
}

class DatabaseCreationOptions {
    <<Options / Parameter Object>>
    +Name : string
    +Owner : string
    +Encoding : string
    +CollationName : string
    +IsTemplate : bool
}

class Database {
    <<Product>>
    +DatabaseId : int
    +Name : string
    +Owner : string
    +State : DatabaseState
    +Encoding : string
    +CollationName : string
    +Schemas : IReadOnlyCollection~Schema~
    +AddSchema(schema : Schema)
    +GetSchema(name : string) Schema
}

class DatabaseState {
    <<enumeration>>
    Online
    Offline
    ReadOnly
    Restoring
    Recovering
}

class ICatalogManager {
    <<Catalog Port>>
    +RegisterDatabase(name : string)
    +GetDatabase(name : string) Database
    +CheckExists(name : string) bool
}

class IStorageEngine {
    <<Storage Port>>
    +AllocateDatabase(name : string)
    +DeallocateDatabase(name : string)
}

class ISecurityManager {
    <<Security Port>>
    +CheckPermission(resource, userId, action) bool
    +GrantOwnership(dbName, owner)
}

class DatabaseManager {
    <<Client>>
    -_factory : IDatabaseFactory
    -_catalog : ICatalogManager
    -_connectionPool : IConnectionPool
    +DatabaseManager(factory, catalog, connectionPool)
    +CreateDatabase(name : string)
    +DropDatabase(name : string, cascade : bool)
    +GetDatabase(name : string) Database
    +ListDatabases() IEnumerable~Database~
    +AttachDatabase(name : string, filePath : string)
    +DetachDatabase(name : string)
}

IDatabaseFactory <|.. DatabaseFactory

DatabaseFactory ..> Database : creates
DatabaseFactory ..> DatabaseCreationOptions : reads
DatabaseFactory --> ICatalogManager : registers
DatabaseFactory --> IStorageEngine : allocates storage
DatabaseFactory --> ISecurityManager : grants ownership

Database --> DatabaseState
Database *-- Schema : default public schema

DatabaseManager --> IDatabaseFactory : uses
DatabaseManager --> ICatalogManager : queries
```

```mermaid
sequenceDiagram
    autonumber

    actor Admin
    participant DBManager as DatabaseManager
    participant Factory as DatabaseFactory
    participant Security as ISecurityManager
    participant Storage as IStorageEngine
    participant Catalog as ICatalogManager
    participant DB as Database

    Admin->>DBManager: CreateDatabase("shop_db")

    DBManager->>Security: CheckPermission("system", adminId, "CREATE_DATABASE")
    Security-->>DBManager: Permitted

    DBManager->>Catalog: CheckExists("shop_db")
    Catalog-->>DBManager: false

    Note over DBManager: Build DatabaseCreationOptions
    DBManager->>Factory: Create(options)

    Factory->>Storage: AllocateDatabase("shop_db")
    Storage-->>Factory: storage allocated

    Factory->>DB: new Database(id, "shop_db", owner, Online)
    DB-->>Factory: database instance

    Factory->>DB: AddSchema(new Schema("public"))
    DB-->>Factory: default schema added

    Factory->>Catalog: RegisterDatabase("shop_db")
    Catalog-->>Factory: registered

    Factory->>Security: GrantOwnership("shop_db", owner)
    Security-->>Factory: ownership granted

    Factory-->>DBManager: Database

    DBManager-->>Admin: Database created successfully
```

---

## Query Processor

### 1. SQL Parsing (Interpreter Pattern)

**Purpose:**  
Define a grammar for a language and provide an interpreter that uses the grammar to parse sentences in that language. Each grammar rule is mapped to a class, and the interpreter recursively evaluates an Abstract Syntax Tree (AST) built from those rules.

**Example:**  
A simple arithmetic expression evaluator that parses and evaluates expressions like `3 + 5 * 2` using a grammar of terminal and non-terminal expressions.

#### Class Diagram

```mermaid
classDiagram
direction LR

class IExpression {
    <<AbstractExpression>>
    +Interpret(context : Context) int
}

class NumberExpression {
    <<TerminalExpression>>
    -value : int
    +Interpret(context : Context) int
}

class AddExpression {
    <<NonTerminalExpression>>
    -left : IExpression
    -right : IExpression
    +Interpret(context : Context) int
}

class MultiplyExpression {
    <<NonTerminalExpression>>
    -left : IExpression
    -right : IExpression
    +Interpret(context : Context) int
}

class Context {
    <<Context>>
    +Variables : Dictionary~string, int~
}

class ExpressionParser {
    <<Client>>
    +Parse(expression : string) IExpression
}

IExpression <|.. NumberExpression
IExpression <|.. AddExpression
IExpression <|.. MultiplyExpression

AddExpression --> IExpression : left
AddExpression --> IExpression : right
MultiplyExpression --> IExpression : left
MultiplyExpression --> IExpression : right
ExpressionParser --> IExpression : builds
IExpression --> Context : uses
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant Parser as ExpressionParser
    participant Add as AddExpression
    participant Mul as MultiplyExpression
    participant Num as NumberExpression
    participant Ctx as Context

    Client->>Parser: Parse("3 + 5 * 2")
    Parser->>Num: new NumberExpression(3)
    Parser->>Num: new NumberExpression(5)
    Parser->>Num: new NumberExpression(2)
    Parser->>Mul: new MultiplyExpression(5, 2)
    Parser->>Add: new AddExpression(3, Mul)
    Parser-->>Client: IExpression (AST root)

    Client->>Add: Interpret(context)
    Add->>Num: Interpret(context) → 3
    Add->>Mul: Interpret(context)
    Mul->>Num: Interpret(context) → 5
    Mul->>Num: Interpret(context) → 2
    Mul-->>Add: 10
    Add-->>Client: 13
```

#### Simplified Code

```csharp
public interface IExpression
{
    // Evaluate this expression node and return its integer result
    int Interpret(Context context);
}

public class NumberExpression : IExpression
{
    private readonly int _value;

    public NumberExpression(int value) => _value = value;

    public int Interpret(Context context)
    {
        // Terminal: return the literal number value
        return _value;
    }
}

public class AddExpression : IExpression
{
    private readonly IExpression _left;
    private readonly IExpression _right;

    public AddExpression(IExpression left, IExpression right)
    {
        _left = left;
        _right = right;
    }

    public int Interpret(Context context)
    {
        // Non-terminal: recursively evaluate both operands and add them
        return _left.Interpret(context) + _right.Interpret(context);
    }
}

public class MultiplyExpression : IExpression
{
    private readonly IExpression _left;
    private readonly IExpression _right;

    public MultiplyExpression(IExpression left, IExpression right)
    {
        _left = left;
        _right = right;
    }

    public int Interpret(Context context)
    {
        // Non-terminal: recursively evaluate both operands and multiply them
        return _left.Interpret(context) * _right.Interpret(context);
    }
}

public class ExpressionParser
{
    public IExpression Parse(string expression)
    {
        // Tokenize the string and build the AST from grammar rules
        // Returns the root IExpression node for caller to Interpret()
        return new NumberExpression(0); // placeholder
    }
}
```

**Benefits**

- Separates grammar rules from evaluation logic — each rule maps to exactly one class.
- Easy to extend: adding a new operator only requires adding a new `IExpression` class.
- The AST makes the structure of the parsed input explicit and inspectable.
- Recursive evaluation naturally mirrors the recursive structure of the grammar.

**Application:** `SQLParser` tokenizes SQL text via `Lexer`, builds an `AST` of `ASTNode` nodes, and hands the tree to `SemanticAnalyzer` for binding into a `LogicalPlan` that `QueryOptimizer` transforms into a `PhysicalPlan` for `QueryExecutor`.

**Why apply?** SQL is a formal language with a well-defined grammar. The Interpreter pattern maps each grammar production rule (SELECT, WHERE clause, expression, literal, identifier …) to a concrete `ASTNode` subclass. `SQLParser` acts as the *Client* that drives `Lexer` to tokenize the input string and then walks the token stream to construct the AST. The AST root can then be traversed — by `SemanticAnalyzer` to resolve names against the catalog, by `QueryOptimizer` to derive logical and physical plans, and by `QueryExecutor` to stream result rows — without any component needing to re-parse raw SQL text. This gives the engine a clean separation between *syntax* (what was written) and *semantics* (what it means).

```mermaid
classDiagram
direction LR

%% =====================================================
%% LEXER — Tokenization
%% =====================================================

class Lexer {
    <<Terminal Producer>>
    +Tokenize(sql : string) Token[]
}

class Token {
    <<Terminal Symbol>>
    +Kind : TokenKind
    +Value : string
    +Position : int
}

class TokenKind {
    <<enumeration>>
    Keyword
    Identifier
    Literal
    Operator
    Punctuation
    EOF
}

%% =====================================================
%% AST — Abstract Syntax Tree
%% =====================================================

class ASTNode {
    <<AbstractExpression>>
    +NodeType : ASTNodeType
    +Children : IReadOnlyList~ASTNode~
    +Accept(visitor : IASTVisitor)
}

class SelectNode {
    <<NonTerminalExpression>>
    +Columns : IReadOnlyList~ASTNode~
    +FromClause : ASTNode
    +WhereClause : ASTNode
    +Accept(visitor : IASTVisitor)
}

class IdentifierNode {
    <<TerminalExpression>>
    +Name : string
    +Accept(visitor : IASTVisitor)
}

class LiteralNode {
    <<TerminalExpression>>
    +Value : object
    +DataType : DataType
    +Accept(visitor : IASTVisitor)
}

class BinaryExpressionNode {
    <<NonTerminalExpression>>
    +Operator : string
    +Left : ASTNode
    +Right : ASTNode
    +Accept(visitor : IASTVisitor)
}

class AST {
    <<AST Root Wrapper>>
    +Root : ASTNode
}

%% =====================================================
%% SQL PARSER — Client / Grammar Interpreter Driver
%% =====================================================

class SQLParser {
    <<Client / Interpreter Driver>>
    +Parse(sql : string) AST
    -Tokenize(sql : string) Token[]
    -BuildAST(tokens : Token[]) ASTNode
    -ParseSelect(tokens : Token[]) SelectNode
    -ParseExpression(tokens : Token[]) ASTNode
}

note for SQLParser "Parse() throws SqlSyntaxException\non invalid input"

%% =====================================================
%% SEMANTIC ANALYZER — Name Binding
%% =====================================================

class SemanticAnalyzer {
    <<Interpreter / Visitor>>
    -catalog : ICatalogManager
    +Bind(ast : AST) LogicalPlan
    -ResolveIdentifier(node : IdentifierNode) Column
    -BindExpression(node : ASTNode) ASTNode
}

note for SemanticAnalyzer "Bind() throws\nObjectNotFoundException if table or column is invalid"

%% =====================================================
%% LOGICAL & PHYSICAL PLANS
%% =====================================================

class LogicalPlan {
    <<Interpreted Result>>
    +Operators : List~Operator~
    +Root : Operator
}

class PhysicalPlan {
    <<Execution Plan>>
    +Operators : List~Operator~
    +Root : IPhysicalOperator
}

class Operator {
    <<abstract>>
    +OperatorType : OperatorType
    +Children : IReadOnlyList~Operator~
}

%% =====================================================
%% QUERY OPTIMIZER
%% =====================================================

class QueryOptimizer {
    <<Optimizer>>
    -costModel : CostModel
    -catalog : ICatalogManager
    +Optimize(plan : LogicalPlan) PhysicalPlan
}

class StatisticsManager {
    <<Statistics Provider>>
    +Collect(table : Table)
    +GetStats(tableId : int) TableStats
}

class TableStats {
    <<Statistics Data>>
    +TableId : int
    +RowCount : long
    +PageCount : int
    +ColumnHistograms : IReadOnlyDictionary~int, Histogram~
}

class CostModel {
    <<Cost Estimator>>
    +EstimateCost(plan : LogicalPlan, stats : TableStats) double
}

%% =====================================================
%% QUERY EXECUTOR
%% =====================================================

class QueryExecutor {
    <<Executor>>
    +Execute(plan : PhysicalPlan, ctx : RuntimeContext) ResultCursor
}

class RuntimeContext {
    <<Execution Context>>
    +TransactionId : int
    +SessionId : string
    +IsolationLevel : IsolationLevel
}

class ResultCursor {
    <<Iterator Result>>
    +MoveNext() bool
    +Current : Row
    +Close()
}

%% =====================================================
%% RELATIONSHIPS
%% =====================================================

SQLParser --> Lexer : tokenizes via
SQLParser --> AST : builds

Lexer --> Token : produces
Token --> TokenKind

ASTNode <|-- SelectNode
ASTNode <|-- IdentifierNode
ASTNode <|-- LiteralNode
ASTNode <|-- BinaryExpressionNode

SelectNode --> ASTNode : children
BinaryExpressionNode --> ASTNode : left / right

AST --> ASTNode : root
AST --> LogicalPlan : converts to

SemanticAnalyzer --> ASTNode : binds
SemanticAnalyzer --> ICatalogManager : resolves names
SemanticAnalyzer --> LogicalPlan : produces

QueryOptimizer --> LogicalPlan : reads
QueryOptimizer --> PhysicalPlan : produces
QueryOptimizer --> StatisticsManager : fetches stats
QueryOptimizer --> CostModel : estimates cost

StatisticsManager --> TableStats : returns
CostModel --> TableStats : reads

LogicalPlan --> Operator : contains
PhysicalPlan --> Operator : contains

QueryExecutor --> PhysicalPlan : executes
QueryExecutor --> RuntimeContext : uses
QueryExecutor --> ResultCursor : returns
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Parser as SQLParser
    participant Lexer as Lexer
    participant AST as AST
    participant Analyzer as SemanticAnalyzer
    participant Catalog as ICatalogManager
    participant Optimizer as QueryOptimizer
    participant Stats as StatisticsManager
    participant Executor as QueryExecutor
    participant Cursor as ResultCursor

    Client->>Parser: Parse("SELECT id, name FROM users WHERE age > 18")

    Parser->>Lexer: Tokenize(sql)
    Lexer-->>Parser: Token[] (SELECT, id, name, FROM, users, WHERE, age, >, 18)

    Note over Parser: BuildAST(tokens)
    Parser->>Parser: ParseSelect(tokens) → SelectNode
    Parser->>Parser: ParseExpression(WHERE tokens) → BinaryExpressionNode(age > 18)

    Parser-->>Client: AST { Root: SelectNode }

    Client->>Analyzer: Bind(ast.Root)

    Analyzer->>Catalog: GetTable("users")
    Catalog-->>Analyzer: Table (users)

    Analyzer->>Catalog: GetColumn("users", "id")
    Catalog-->>Analyzer: Column (id)

    Analyzer->>Catalog: GetColumn("users", "name")
    Catalog-->>Analyzer: Column (name)

    Analyzer->>Catalog: GetColumn("users", "age")
    Catalog-->>Analyzer: Column (age)

    Analyzer-->>Client: LogicalPlan { Project[id,name] → Filter[age>18] → Scan[users] }

    Client->>Optimizer: Optimize(logicalPlan)

    Optimizer->>Stats: GetStats(tableId: users)
    Stats-->>Optimizer: TableStats { RowCount: 50000, ... }

    Note over Optimizer: Apply predicate pushdown,\nselect best join order,\nestimate I/O cost

    Optimizer-->>Client: PhysicalPlan { IndexScan[age] → Filter → Project }

    Client->>Executor: Execute(physicalPlan, runtimeContext)

    Executor-->>Client: ResultCursor

    loop Fetch rows
        Client->>Cursor: MoveNext()
        Cursor-->>Client: true / false
        Client->>Cursor: Current
        Cursor-->>Client: Row { id, name }
    end

    Client->>Cursor: Close()
```

### 2. Optimization Rules (Chain of Responsibility Pattern)

**Purpose:**  
Avoid coupling the sender of a request to its receiver by giving more than one object a chance to handle the request. Chain the receiving objects and pass the request along the chain until an object handles it. Each handler decides either to process the request or to forward it to the next handler in the chain.

**Example:**  
A customer support ticket routing system where tickets are passed through a chain of handlers — `L1SupportHandler` → `L2SupportHandler` → `ManagerHandler` — and each handler either resolves the ticket or escalates it to the next level.

#### Class Diagram

```mermaid
classDiagram
direction LR

class ITicketHandler {
    <<Handler>>
    +SetNext(next : ITicketHandler) ITicketHandler
    +Handle(ticket : SupportTicket) string
}

class BaseTicketHandler {
    <<AbstractHandler>>
    -next : ITicketHandler
    +SetNext(next : ITicketHandler) ITicketHandler
    +Handle(ticket : SupportTicket) string
}

class L1SupportHandler {
    <<ConcreteHandler>>
    +Handle(ticket : SupportTicket) string
}

class L2SupportHandler {
    <<ConcreteHandler>>
    +Handle(ticket : SupportTicket) string
}

class ManagerHandler {
    <<ConcreteHandler>>
    +Handle(ticket : SupportTicket) string
}

class SupportTicket {
    <<Request>>
    +Priority : int
    +Description : string
}

ITicketHandler <|.. BaseTicketHandler
BaseTicketHandler <|-- L1SupportHandler
BaseTicketHandler <|-- L2SupportHandler
BaseTicketHandler <|-- ManagerHandler

ITicketHandler --> ITicketHandler : next
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant L1 as L1SupportHandler
    participant L2 as L2SupportHandler
    participant Mgr as ManagerHandler

    Client->>L1: Handle(ticket priority=3)
    Note over L1: priority > 1 — cannot handle
    L1->>L2: Handle(ticket priority=3)
    Note over L2: priority > 2 — cannot handle
    L2->>Mgr: Handle(ticket priority=3)
    Note over Mgr: handles all remaining tickets
    Mgr-->>L2: "Escalated to Manager"
    L2-->>L1: "Escalated to Manager"
    L1-->>Client: "Escalated to Manager"
```

#### Simplified Code

```csharp
public interface ITicketHandler
{
    // Link this handler to the next one in the chain
    ITicketHandler SetNext(ITicketHandler next);
    // Process the ticket or forward it to the next handler
    string Handle(SupportTicket ticket);
}

public abstract class BaseTicketHandler : ITicketHandler
{
    private ITicketHandler? _next;

    public ITicketHandler SetNext(ITicketHandler next)
    {
        _next = next;
        return next; // allows fluent chaining: h1.SetNext(h2).SetNext(h3)
    }

    public virtual string Handle(SupportTicket ticket)
    {
        // Default: forward to next handler if one exists
        return _next?.Handle(ticket) ?? "No handler could process the ticket";
    }
}

public class L1SupportHandler : BaseTicketHandler
{
    public override string Handle(SupportTicket ticket)
    {
        if (ticket.Priority == 1)
            return $"L1 resolved: {ticket.Description}";

        // Priority too high — pass to next handler
        return base.Handle(ticket);
    }
}

public class L2SupportHandler : BaseTicketHandler
{
    public override string Handle(SupportTicket ticket)
    {
        if (ticket.Priority <= 2)
            return $"L2 resolved: {ticket.Description}";

        return base.Handle(ticket);
    }
}

public class ManagerHandler : BaseTicketHandler
{
    public override string Handle(SupportTicket ticket)
    {
        // Manager handles all tickets regardless of priority
        return $"Manager resolved: {ticket.Description}";
    }
}

// Usage: build chain and send a ticket
var l1 = new L1SupportHandler();
var l2 = new L2SupportHandler();
var mgr = new ManagerHandler();
l1.SetNext(l2).SetNext(mgr);

var ticket = new SupportTicket { Priority = 3, Description = "System outage" };
string result = l1.Handle(ticket); // → "Manager resolved: System outage"
```

**Benefits**

- Decouples the sender from receiver — the client does not know which handler will ultimately process the request.
- Responsibilities can be assigned dynamically at runtime by assembling different handler chains.
- New handlers can be added or removed without modifying existing handlers (Open/Closed Principle).
- Each handler is focused on a single concern, keeping individual classes small and testable.
- The chain can terminate early when any handler fully handles the request, avoiding unnecessary processing.

**Application:** `OptimizationRulePipeline` assembles a chain of `IOptimizationRule` handlers — `ConstantFoldingRule` → `PredicatePushdownRule` → `ProjectionPruningRule` → `JoinReorderingRule`. Each rule either transforms the `LogicalPlan` or passes it to the next. The pipeline repeats the full chain until no rule reports a change (`OptimizeUntilStable`), guaranteeing a fixed point.

**Why apply?** A SQL query optimizer must apply many independent, composable transformations — constant folding, predicate pushdown, projection pruning, join reordering, subquery unnesting — in a well-defined sequence. Hardcoding all these transformations into a single `Optimize()` method creates a monolithic, brittle class. The Chain of Responsibility pattern allows each transformation to be encapsulated in its own handler class, enables new rules to be registered without touching existing logic, and makes the ordering and enablement of rules fully configurable at startup. The `OptimizationRulePipeline` acts as the chain coordinator, iterating passes until the plan is stable, which naturally models fixed-point optimization used by production DBMS engines such as PostgreSQL and SQL Server.

```mermaid
classDiagram
direction LR

%% =====================================================
%% LOGICAL PLAN — REQUEST
%% =====================================================

class LogicalPlan {
    <<Request>>
    +Root : LogicalOperator
    +Clone() LogicalPlan
}

class LogicalOperator {
    <<abstract>>
    +OperatorType : LogicalOperatorType
    +Children : IReadOnlyList~LogicalOperator~
    +ReplaceChild(oldChild : LogicalOperator, newChild : LogicalOperator)
}

class LogicalScan {
    +TableId : int
    +Alias : string
}

class LogicalFilter {
    +Predicate : Expression
    +Child : LogicalOperator
}

class LogicalProject {
    +Expressions : IReadOnlyList~Expression~
    +Child : LogicalOperator
}

class LogicalJoin {
    +JoinType : JoinType
    +Condition : Expression
    +Left : LogicalOperator
    +Right : LogicalOperator
}

LogicalOperator <|-- LogicalScan
LogicalOperator <|-- LogicalFilter
LogicalOperator <|-- LogicalProject
LogicalOperator <|-- LogicalJoin

LogicalPlan *-- LogicalOperator : root
LogicalOperator --> LogicalOperator : children

%% =====================================================
%% OPTIMIZATION CONTEXT
%% =====================================================

class OptimizationContext {
    <<Context>>
    +Catalog : ICatalogManager
    +Statistics : StatisticsManager
    +CostModel : CostModel
    +MaxPasses : int
}

class OptimizationResult {
    <<Result>>
    +Plan : LogicalPlan
    +Changed : bool
    +AppliedRules : IReadOnlyList~string~
}

%% =====================================================
%% CHAIN OF RESPONSIBILITY — HANDLER
%% =====================================================

class IOptimizationRule {
    <<Handler>>
    +SetNext(next : IOptimizationRule) IOptimizationRule
    +Handle(plan : LogicalPlan, ctx : OptimizationContext) OptimizationResult
}

class OptimizationRuleBase {
    <<AbstractHandler>>
    -next : IOptimizationRule
    +SetNext(next : IOptimizationRule) IOptimizationRule
    +Handle(plan : LogicalPlan, ctx : OptimizationContext) OptimizationResult
    #CanApply(plan : LogicalPlan, ctx : OptimizationContext) bool
    #Apply(plan : LogicalPlan, ctx : OptimizationContext) OptimizationResult
    #PassToNext(result : OptimizationResult, ctx : OptimizationContext) OptimizationResult
}

IOptimizationRule <|.. OptimizationRuleBase

%% =====================================================
%% CONCRETE HANDLERS
%% =====================================================

class ConstantFoldingRule {
    <<ConcreteHandler>>
    #CanApply(plan : LogicalPlan, ctx : OptimizationContext) bool
    #Apply(plan : LogicalPlan, ctx : OptimizationContext) OptimizationResult
}

class PredicatePushdownRule {
    <<ConcreteHandler>>
    #CanApply(plan : LogicalPlan, ctx : OptimizationContext) bool
    #Apply(plan : LogicalPlan, ctx : OptimizationContext) OptimizationResult
}

class ProjectionPruningRule {
    <<ConcreteHandler>>
    #CanApply(plan : LogicalPlan, ctx : OptimizationContext) bool
    #Apply(plan : LogicalPlan, ctx : OptimizationContext) OptimizationResult
}

class JoinReorderingRule {
    <<ConcreteHandler>>
    #CanApply(plan : LogicalPlan, ctx : OptimizationContext) bool
    #Apply(plan : LogicalPlan, ctx : OptimizationContext) OptimizationResult
}

OptimizationRuleBase <|-- ConstantFoldingRule
OptimizationRuleBase <|-- PredicatePushdownRule
OptimizationRuleBase <|-- ProjectionPruningRule
OptimizationRuleBase <|-- JoinReorderingRule

%% =====================================================
%% RULE PIPELINE
%% =====================================================

class OptimizationRulePipeline {
    <<Chain Coordinator>>
    -firstRule : IOptimizationRule
    -rules : List~IOptimizationRule~
    -maxPasses : int

    +AddRule(rule : IOptimizationRule) OptimizationRulePipeline
    +BuildChain()
    +OptimizeUntilStable(plan : LogicalPlan, ctx : OptimizationContext) LogicalPlan
}

OptimizationRulePipeline o-- IOptimizationRule : contains
IOptimizationRule --> IOptimizationRule : next handler

%% =====================================================
%% QUERY OPTIMIZER
%% =====================================================

class QueryOptimizer {
    <<Client>>
    -rulePipeline : OptimizationRulePipeline
    -physicalPlanGenerator : PhysicalPlanGenerator
    -statisticsManager : StatisticsManager
    -costModel : CostModel

    +Optimize(plan : LogicalPlan) PhysicalPlan
}

class PhysicalPlanGenerator {
    +Generate(plan : LogicalPlan, ctx : OptimizationContext) PhysicalPlan
}

class PhysicalPlan {
    +Root : IPhysicalOperator
}

QueryOptimizer --> OptimizationRulePipeline : optimizes logical plan
QueryOptimizer --> PhysicalPlanGenerator : generates physical plan
QueryOptimizer --> StatisticsManager : obtains statistics
QueryOptimizer --> CostModel : estimates alternatives

OptimizationRulePipeline --> LogicalPlan : transforms
OptimizationRulePipeline --> OptimizationContext : uses

JoinReorderingRule --> StatisticsManager : estimates cardinality
JoinReorderingRule --> CostModel : compares join orders

PhysicalPlanGenerator --> LogicalPlan : reads
PhysicalPlanGenerator --> PhysicalPlan : creates
```

```mermaid
sequenceDiagram
    autonumber

    participant QP as QueryProcessor
    participant Parser as SQLParser
    participant Analyzer as SemanticAnalyzer
    participant Optimizer as QueryOptimizer
    participant Pipeline as OptimizationRulePipeline
    participant Generator as PhysicalPlanGenerator

    QP->>Parser: Parse(sql)
    Parser-->>QP: AST

    QP->>Analyzer: Bind(AST)
    Analyzer-->>QP: LogicalPlan

    QP->>Optimizer: Optimize(LogicalPlan)
    Optimizer->>Pipeline: OptimizeUntilStable(LogicalPlan, context)
    Pipeline-->>Optimizer: Optimized LogicalPlan

    Optimizer->>Generator: Generate(Optimized LogicalPlan)
    Generator-->>Optimizer: PhysicalPlan

    Optimizer-->>QP: PhysicalPlan
```

---

## Storage Engine

### 1. Buffer Management (Proxy Pattern)

**Purpose:**  
Provide a surrogate or placeholder for another object to control access to it. The Proxy intercepts requests to the real object and can add behaviors such as lazy initialization, access control, caching, or logging before forwarding the call.

**Example:**  
A `CachedImage` proxy that holds a reference to an expensive `RealImage`. On the first `Display()` call the proxy loads the image from disk; on subsequent calls it serves the cached copy without touching the file system.

#### Class Diagram

```mermaid
classDiagram
direction LR

class IImage {
    <<Subject>>
    +Display()
}

class RealImage {
    <<RealSubject>>
    -filename : string
    +Load()
    +Display()
}

class CachedImage {
    <<Proxy>>
    -realImage : RealImage
    -filename : string
    +Display()
}

class Client {
    <<Client>>
    +ShowImage()
}

IImage <|.. RealImage
IImage <|.. CachedImage
CachedImage --> RealImage : delegates to (lazy)
Client --> IImage : uses
```

#### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client
    participant Proxy as CachedImage
    participant Real as RealImage

    Client->>Proxy: Display()
    alt image not cached
        Proxy->>Real: new RealImage(filename)
        Real->>Real: Load() — read from disk
        Real-->>Proxy: loaded
    end
    Proxy->>Real: Display()
    Real-->>Proxy: rendered
    Proxy-->>Client: done

    Client->>Proxy: Display()
    Note over Proxy: image already cached
    Proxy->>Real: Display()
    Real-->>Proxy: rendered
    Proxy-->>Client: done
```

#### Simplified Code

```csharp
public interface IImage
{
    // Render the image to the screen
    void Display();
}

public class RealImage : IImage
{
    private readonly string _filename;

    public RealImage(string filename)
    {
        _filename = filename;
        // Expensive operation: load bytes from disk immediately on creation
        Load();
    }

    private void Load()
    {
        // Read image file from disk into memory
        Console.WriteLine($"Loading image from disk: {_filename}");
    }

    public void Display()
    {
        // Render the already-loaded image
        Console.WriteLine($"Displaying image: {_filename}");
    }
}

public class CachedImage : IImage
{
    private readonly string _filename;
    private RealImage? _realImage;   // null until first Display() call

    public CachedImage(string filename)
    {
        _filename = filename;
        // Proxy is cheap to construct — no disk I/O yet
    }

    public void Display()
    {
        // On cache miss: create and load the real object
        _realImage ??= new RealImage(_filename);

        // Forward the call to the real object
        _realImage.Display();
    }
}
```

**Benefits**

- Eliminates redundant I/O by serving cached results on subsequent access.
- Decouples the caller from the real subject; the client only depends on the interface.
- Lazy initialization defers expensive operations until they are actually needed.
- Cache invalidation and miss-handling logic are encapsulated inside the proxy, invisible to callers.

**Application:** `BufferPool` acts as a transparent proxy for `FileManager`. When `StorageEngine` calls `FetchPage(pageId)`, `BufferPool` first looks for the page in its in-memory frame pool. If found (cache hit), the pinned `Page` is returned instantly. On a cache miss, `BufferPool` calls `FileManager.Read(pageId)` to load the page from disk, places it in a free or evicted frame, and then returns the page — all transparently behind the same interface that `StorageEngine` uses.

**Why apply?** Disk I/O is several orders of magnitude slower than memory access. Without a caching layer every `ReadPage` call would hit disk, making query execution prohibitively slow. The Proxy pattern lets `BufferPool` intercept every page-access request, serve hot pages from RAM, and only fall through to `FileManager` on a cache miss. The `StorageEngine` never needs to know whether a page came from memory or disk — it always calls the same `FetchPage` interface. Replacement policies (`LRU`, `Clock`) and dirty-page flushing are also hidden inside the proxy, keeping the rest of the engine clean.

```mermaid
classDiagram
direction LR

%% =====================================================
%% CLIENT / FACADE
%% =====================================================

class StorageEngine {
    <<Client / Facade>>
    -pageStore : IPageStore
    -walManager : WALManager

    +ReadPage(id : PageId) Byte[]
    +WritePage(id : PageId, data : Byte[])
    +AllocatePage(tableId : int) PageId
    +FlushPage(id : PageId)
}

%% =====================================================
%% PROXY SUBJECT
%% =====================================================

class IPageStore {
    <<Subject>>
    +FetchPage(id : PageId) Page
    +FlushPage(id : PageId)
    +AllocatePage(tableId : int) PageId
}

%% =====================================================
%% BUFFER POOL — CACHING PROXY
%% =====================================================

class BufferPoolProxy {
    <<Proxy>>
    -frames : Dictionary~PageId, BufferFrame~
    -realStore : IPageStore
    -replacementPolicy : IReplacementPolicy

    +FetchPage(id : PageId) Page
    +FlushPage(id : PageId)
    +AllocatePage(tableId : int) PageId

    +UnpinPage(id : PageId)
    +MarkDirty(id : PageId)
    -EvictFrame() BufferFrame
}

note for BufferPoolProxy "FetchPage:\n1. Cache hit → pin and return page\n2. Cache miss → delegate to realStore\n3. Cache loaded page in a frame"

%% =====================================================
%% REAL SUBJECT
%% =====================================================

class DiskPageStore {
    <<RealSubject>>
    -fileManager : IFileManager

    +FetchPage(id : PageId) Page
    +FlushPage(id : PageId)
    +AllocatePage(tableId : int) PageId
}

class IFileManager {
    <<interface>>
    +Read(pageId : PageId) Byte[]
    +Write(pageId : PageId, data : Byte[])
    +AllocatePage(tableId : int) PageId
}

class FileManager {
    -dataDir : string

    +Read(pageId : PageId) Byte[]
    +Write(pageId : PageId, data : Byte[])
    +AllocatePage(tableId : int) PageId
    +CreateFile(path : string) int
}

%% =====================================================
%% BUFFER FRAME AND PAGE
%% =====================================================

class BufferFrame {
    +FrameId : int
    +Page : Page
    +PinCount : int
    +IsDirty : bool
}

class Page {
    +PageId : PageId
    +Data : Byte[]

    +InsertRecord(record : Byte[])
    +DeleteRecord(rid : RID)
    +Compact()
}

note for Page "InsertRecord() throws\nPageFullException if full"

class PageId {
    <<Value Object>>
    +TableId : int
    +PageNumber : int
}

class RID {
    <<Value Object>>
    +PageId : PageId
    +SlotNumber : int
}

%% =====================================================
%% REPLACEMENT STRATEGY
%% =====================================================

class IReplacementPolicy {
    <<interface>>
    +SelectVictim() PageId
    +OnAccess(id : PageId)
    +SetEvictable(id : PageId, evictable : bool)
}

class LruPolicy {
    -accessOrder : LinkedList~PageId~

    +SelectVictim() PageId
    +OnAccess(id : PageId)
    +SetEvictable(id : PageId, evictable : bool)
}

class ClockPolicy {
    -hand : int
    -referenceBits : Dictionary~PageId, bool~

    +SelectVictim() PageId
    +OnAccess(id : PageId)
    +SetEvictable(id : PageId, evictable : bool)
}

%% =====================================================
%% WAL AND RECOVERY
%% =====================================================

class WALManager {
    +WriteLog(record : LogRecord) long
    +Flush(lsn : long)
    +ReadFrom(lsn : long) IEnumerable~LogRecord~
}

class RecoveryManager {
    -walManager : WALManager
    -pageStore : IPageStore

    +Recover(checkpoint : long)
    +Redo(record : LogRecord)
    +Undo(record : LogRecord)
}

class LogRecord {
    <<Value Object>>
    +LSN : long
    +TransactionId : int
    +Type : LogRecordType
    +PageId : PageId
    +BeforeImage : Byte[]
    +AfterImage : Byte[]
}

%% =====================================================
%% RELATIONSHIPS
%% =====================================================

IPageStore <|.. BufferPoolProxy
IPageStore <|.. DiskPageStore

IFileManager <|.. FileManager

StorageEngine --> IPageStore : uses
StorageEngine --> WALManager : logs writes

BufferPoolProxy --> IPageStore : delegates cache misses
BufferPoolProxy --> BufferFrame : manages
BufferPoolProxy --> IReplacementPolicy : eviction strategy

DiskPageStore --> IFileManager : performs disk I/O

BufferFrame *-- Page
Page --> PageId
Page --> RID

IReplacementPolicy <|.. LruPolicy
IReplacementPolicy <|.. ClockPolicy

RecoveryManager --> WALManager : reads log
RecoveryManager --> IPageStore : applies redo/undo
WALManager --> LogRecord : stores
```

```mermaid
sequenceDiagram
    autonumber

    actor Client as RecordManager
    participant SE as StorageEngine
    participant BP as BufferPoolProxy
    participant Policy as IReplacementPolicy
    participant Disk as DiskPageStore
    participant FM as FileManager
    participant WAL as WALManager
    participant Page as Page

    Client->>SE: ReadPage(pageId)
    SE->>BP: FetchPage(pageId)

    alt Cache hit
        BP->>Page: PinCount++
        BP->>Policy: OnAccess(pageId)
        BP-->>SE: Page
    else Cache miss
        BP->>BP: FindFreeFrame()

        alt No free frame
            BP->>Policy: SelectVictim()
            Policy-->>BP: victimPageId

            alt Victim is dirty
                BP->>WAL: Flush(victim.PageLSN)
                WAL-->>BP: WAL durable
                BP->>Disk: FlushPage(victimPageId)
                Disk->>FM: Write(victimPageId, data)
                FM-->>Disk: completed
                Disk-->>BP: completed
            end
        end

        BP->>Disk: FetchPage(pageId)
        Disk->>FM: Read(pageId)
        FM-->>Disk: Byte[]
        Disk-->>BP: Page

        BP->>Page: PinCount = 1
        BP->>Policy: SetEvictable(pageId, false)
        BP-->>SE: Page
    end

    SE-->>Client: Page.Data

    Client->>SE: WritePage(pageId, newData, txId)
    SE->>WAL: WriteLog(UpdateRecord)
    WAL-->>SE: LSN

    SE->>Page: ApplyUpdate(newData)
    SE->>Page: PageLSN = LSN
    SE->>BP: MarkDirty(pageId)

    Client->>SE: UnpinPage(pageId)
    SE->>BP: UnpinPage(pageId)
    BP->>Page: PinCount--

    alt PinCount == 0
        BP->>Policy: SetEvictable(pageId, true)
    end

    Client->>SE: FlushPage(pageId)
    SE->>BP: FlushPage(pageId)

    alt Page is dirty
        BP->>WAL: Flush(Page.PageLSN)
        WAL-->>BP: WAL durable
        BP->>Disk: FlushPage(pageId)
        Disk->>FM: Write(pageId, Page.Data)
        FM-->>Disk: completed
        Disk-->>BP: completed
        BP->>Page: IsDirty = false
    end
```