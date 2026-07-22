# Design Patterns in DBMS

This document outlines the Design Patterns implemented within various core components of the BBV-DBMS.

## Visual Summary

| Module | Feature | Pattern | Application |
| :--- | :--- | :--- | :--- |
| **Database & Metadata** | Hierarchy Management | **Composite** | Manages hierarchical architecture: `DatabaseComposite` → `SchemaComposite` → `TableComposite` → `ColumnLeaf`. |
| **Database & Metadata** | Metadata Initialization | **Builder** | Uses `TableDefBuilder` to set each table property instead of a long constructor. |
| **Database & Metadata** | Constraint Validation | **Strategy** | `PrimaryKeyConstraint`, `UniqueConstraint`, `ForeignKeyConstraint` implement the same interface. |
| **Database & Metadata** | Dynamic Allocation | **Factory Method** | Dynamically initializes Indexes and Constraints via `ObjectFactoryProvider` during DDL execution. |
| **Database & Metadata** | Hierarchy Traversal | **Iterator** | Traverse Schema, Table, Column. |
| **Database & Metadata** | Trigger Execution | **Command** | Trigger executes DDL actions. |
| **Database & Metadata** | System Utilities | **Visitor** | Backup, Export DDL, Metadata Scan, Statistics. |
| **Database & Metadata** | Data Change Reactions | **Observer** | Trigger, Index, Statistics react when data changes. |
| **Database & Metadata** | DDL Coordination | **Facade / Application Service**| `SchemaService` coordinates DDL. |
| **Database Manager** | System Initialization | **Facade** | `DbEngineFacade` groups complex startup steps for Disk, Storage, and Catalog. |
| **Database Manager** | DDL Operations | **Command** | Encapsulates Database create/drop commands into `CreateDatabaseAction` for easy undo/redo or logging. |

---

```mermaid
classDiagram
direction LR

%% =====================================================
%% Composite Hierarchy & DB Objects
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

class ICatalogIterator {
    <<Iterator>>
    +GetNext() ICatalogComponent
    +HasMore() bool
}

class CatalogIterator {
    <<Concrete Iterator>>
    -collection : ICatalogComposite
    -iterationState
    +CatalogIterator(collection : ICatalogComposite)
    +GetNext() ICatalogComponent
    +HasMore() bool
}

class IIterableCatalog {
    <<Iterable Collection>>
    +CreateIterator() ICatalogIterator
}

class Database {
    +DatabaseId : int
    +Name : string
    +Owner : string
    +Schemas : IReadOnlyList~Schema~
    +Database(id : int, name : string, owner : string)
    +GetSchema(name : string) Schema
    +GetSchemas() IReadOnlyList~Schema~
    +AddSchema(schema : Schema)
    +RemoveSchema(name : string)
    +Backup(path : string, fileManager : IFileManager)
    +Restore(path : string, fileManager : IFileManager)
    +CreateIterator() ICatalogIterator
}

class Schema {
    +SchemaId : int
    +Name : string
    +Parent : Database
    +Tables : IReadOnlyCollection~Table~
    +Views : IReadOnlyCollection~View~
    +Procedures : IReadOnlyCollection~StoredProcedure~
    +Sequences : IReadOnlyCollection~Sequence~
    +Schema(name : string)
    +AddTable(table : Table)
    +RemoveTable(name : string)
    +GetTable(name : string) Table
    +GetTables() IReadOnlyCollection~Table~
    +AddView(view : View)
    +RemoveView(name : string)
    +AddProcedure(proc : StoredProcedure)
    +RemoveProcedure(name : string)
    +AddSequence(seq : Sequence)
    +RemoveSequence(name : string)
    +CreateIterator() ICatalogIterator
}

class Table {
    +TableId : int
    +Name : string
    +Parent : Schema
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~
    +Table(name : string)
    +AddColumn(col : Column)
    +RemoveColumn(name : string)
    +GetColumn(name : string) Column
    +GetColumns() IReadOnlyCollection~Column~
    +AddConstraint(constraint : Constraint)
    +RemoveConstraint(name : string)
    +AddIndex(index : Index)
    +RemoveIndex(name : string)
    +AddPartition(partition : Partition)
    +RemovePartition(name : string)
    +AddTrigger(trigger : Trigger)
    +RemoveTrigger(name : string)
    +CreateIterator() ICatalogIterator
}

class Column {
    +ColumnId : int
    +Name : string
    +Parent : Table
    +DataType : DataType
    +Nullable : bool
    +DefaultValue : object
    +SetDataType(type : DataType)
    +SetNullable(nullable : bool)
    +SetDefaultValue(value : object)
    +Rename(newName : string)
    +ValidateValue(value : object) bool
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

class Constraint {
    <<abstract>>
    +Name : string
    +Validate(row : Row) bool
}

class IRowKeyExtractor {
    <<interface>>
    +ExtractKey(row : Row, columns : List~Column~) object
    +HasNullValue(row : Row, columns : List~Column~) bool
}

class PrimaryKey {
    +Columns : List~Column~
    -Index _index
    -IRowKeyExtractor _extractor
}

class ForeignKey {
    +ReferenceTable : Table
    +ReferenceColumns : List~Column~
}

class UniqueConstraint {
    +Columns : List~Column~
    -Index _index
    -IRowKeyExtractor _extractor
}

class CheckConstraint {
    +Expression : string
}

class View {
    +ViewId : int
    +Name : string
    +QueryDefinition : string
    +View(name : string)
    +Compile() ExecutionPlan
    +Execute() ResultCursor
}

class StoredProcedure {
    +Name : string
    +Parameters : IReadOnlyCollection~Column~
    +Body : string
    +StoredProcedure(name : string)
    +Compile()
    +Execute(args : object[]) ResultCursor
}

class Sequence {
    +Name : string
    +CurrentValue : long
    +Increment : long
    +Sequence(name : string)
    +NextValue() long
    +Reset()
}

class Partition {
    +PartitionKey : string
    +PartitionType : string
    +InsertRecord(row : Row)
    +DropPartition(name : string)
    +GetPartition(key : object) Partition
}

class Trigger {
    +Name : string
    +Event : TriggerEvent
    +Timing : TriggerTiming
    +Body : string
    +Execute(context : TriggerContext)
}

class Row {
    +RowId : RID
    +Data : RecordData
    +Version : long
    +GetValue(colId : int) object
    +SetValue(colId : int, value : object)
    +UpdateValue(colId : int, value : object)
}

class RecordData {
    <<value object>>
    +Bytes : Byte[]
    +Length : int
    +Serialize() Byte[]
    +Deserialize(bytes : Byte[])
    +GetLength() int
}

class RID {
    <<value object>>
    +PageId : int
    +SlotNumber : int
    +Equals(other : RID) bool
}

class Index {
    <<abstract>>
    +Name : string
    +Insert(key, rid)
    +Delete(key)
    +Search(key)
}

class BTreeIndex
class HashIndex
class BitmapIndex

Database "1" *-- "*" Schema
Schema "1" *-- "*" Table
Table "1" *-- "*" Column
Table "1" *-- "*" Constraint
Table "1" *-- "*" Index
Table "1" *-- "*" Partition
Table "1" *-- "*" Trigger
Schema "1" *-- "*" View
Schema "1" *-- "*" StoredProcedure
Schema "1" *-- "*" Sequence
Table "1" *-- "*" Row

Row *-- RID
Row *-- RecordData
Column --> DataType
Constraint <|-- PrimaryKey
Constraint <|-- ForeignKey
Constraint <|-- UniqueConstraint
Constraint <|-- CheckConstraint
ForeignKey --> Table
PrimaryKey --> IRowKeyExtractor
UniqueConstraint --> IRowKeyExtractor
PrimaryKey --> Index
UniqueConstraint --> Index
Index <|-- BTreeIndex
Index <|-- HashIndex
Index <|-- BitmapIndex

  class TableDefinition {
      <<Command Data>>
      +Name : string
      +Columns : IReadOnlyCollection~Column~
      +Constraints : IReadOnlyCollection~ConstraintOptions~
      +Indexes : IReadOnlyCollection~IndexOptions~
      +Partitions : IReadOnlyCollection~Partition~
      +Triggers : IReadOnlyCollection~Trigger~
  }

  class ConstraintOptions {
      <<DTO>>
      +Columns : List~Column~
      +ReferenceTable : Table
      +ReferenceColumns : List~Column~
      +Expression : string
  }

  class IndexOptions {
      <<DTO>>
      +Name : string
      +Columns : List~Column~
      +Unique : bool
  }

  class DdlResult {
      <<Result>>
      +Success : bool
      +Message : string
      +AffectedObject : ICatalogComponent
  }

  TableDefinition --> Column
  TableDefinition --> ConstraintOptions
  TableDefinition --> IndexOptions
  TableDefinition --> Partition
  TableDefinition --> Trigger

  %% =====================================================
  %% Services, Builders, Factories & Commands
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
  }

  class TableDirector {
      <<Director>>
      +Construct(definition : TableDefinition) Table
  }

  class IConstraintFactory {
      <<Factory>>
      +Create(options : ConstraintOptions) Constraint
  }

  class ConstraintFactory {
      <<Concrete Factory>>
  }

  class ConstraintType {
      <<enumeration>>
      PRIMARY_KEY
      FOREIGN_KEY
      UNIQUE
      CHECK
  }

  class IIndexFactory {
      <<Factory>>
      +Create(options : IndexOptions) Index
  }

  class IndexFactory {
      <<Concrete Factory>>
  }

  class IndexType {
      <<enumeration>>
      BTREE
      HASH
      BITMAP
  }

  class IDdlCommand {
      <<Command>>
      +Execute() DdlResult
  }

  class CreateTableCommand {
      <<Concrete Command>>
  }

  class CreateSchemaCommand {
      <<Concrete Command>>
  }

  class DdlCommandExecutor {
      <<Invoker>>
      +Execute(command : IDdlCommand) DdlResult
  }


  class IDatabaseService {
      <<Service>>
      +CreateSchema(database : Database, name : string) Schema
  }

  class DatabaseService {
      <<Service>>
  }

  class ISchemaService {
      <<Service>>
      +CreateTable(schema : Schema, definition : TableDefinition) Table
  }

  class SchemaService {
      <<Service>>
  }

  class CatalogManager {
      <<Manager>>
  }

  class StorageEngine {
      <<Manager>>
  }
  
  class RecordManager {
      <<Manager>>
  }
  
  class IndexManager {
      <<Manager>>
  }

  ITableBuilder <|.. TableBuilder
  IConstraintFactory <|.. ConstraintFactory
  IIndexFactory <|.. IndexFactory
  IDdlCommand <|.. CreateTableCommand
  IDdlCommand <|.. CreateSchemaCommand
  IDatabaseService <|.. DatabaseService
  ISchemaService <|.. SchemaService
  
  TableDirector --> ITableBuilder
  TableDirector --> IConstraintFactory
  TableDirector --> IIndexFactory
  TableDirector --> TableDefinition
  IConstraintFactory --> ConstraintOptions
  IIndexFactory --> IndexOptions
  IConstraintFactory --> Constraint
  IIndexFactory --> Index
  DdlCommandExecutor --> IDdlCommand
  CreateTableCommand --> ISchemaService
  CreateSchemaCommand --> IDatabaseService
  CreateTableCommand --> Schema
  CreateTableCommand --> TableDefinition
  CreateSchemaCommand --> Database
  SchemaService --> TableDirector
  SchemaService --> Schema
  SchemaService --> CatalogManager
  SchemaService --> StorageEngine
  DatabaseService --> Database
  DatabaseService --> CatalogManager
  IDdlCommand --> DdlResult


%% =====================================================
%% Builder Pattern
%% =====================================================

class ITableBuilder {
    <<interface>>
    +Reset(name : string)
    +AddColumn(column : Column)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
    +Build() Table
}

class TableBuilder {
    -currentTable : Table
    +Reset(name : string)
    +AddColumn(column : Column)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
    +Build() Table
}


%% =====================================================
%% Factory Method Patterns
%% =====================================================

class IConstraintFactory {
    <<interface>>
    +Create(type : ConstraintType, options : ConstraintOptions) Constraint
}

class ConstraintFactory {
    +Create(type : ConstraintType, options : ConstraintOptions) Constraint
}

class ConstraintType{
    <<enumeration>>
    PRIMARY_KEY
    UNIQUE
    FOREIGN_KEY
    CHECK
}

class ConstraintOptions{
    +Columns : List~Column~
    +ReferenceTable : Table
    +ReferenceColumns : List~Column~
    +Expression : string
}

class IIndexFactory {
    <<interface>>
    +Create(type : IndexType, options : IndexOptions) Index
}

class IndexFactory {
    +Create(type : IndexType, options : IndexOptions) Index
}

class IndexType{
    <<enumeration>>
    BTREE
    HASH
    BITMAP
}

class IndexOptions{
    +Name : string
    +Columns : List~Column~
    +Unique : bool
}

ConstraintFactory --> ConstraintType
ConstraintFactory --> ConstraintOptions

IndexFactory --> IndexType
IndexFactory --> IndexOptions


%% =====================================================
%% Domain Services
%% =====================================================

class SchemaService {
    -catalog : CatalogManager
    -storage : StorageEngine
    -builder : ITableBuilder
    -constraintFactory : IConstraintFactory
    -indexFactory : IIndexFactory
    +CreateTable(schema : Schema, name : string) Table
    +DropTable(schema : Schema, name : string)
    +CreateView(schema : Schema, name : string, query : string) View
    +DropView(schema : Schema, name : string)
}

class RecordManager {
    -storage : StorageEngine
    -catalog : CatalogManager
    +Insert(table : Table, row : Row) RID
    +Update(table : Table, rid : RID, row : Row)
    +Delete(table : Table, rid : RID)
    +Read(table : Table, rid : RID) Row
    +Scan(table : Table) List~Row~
}

class IndexManager {
    -indexes : Dictionary~string, Index~
    +Register(index : Index)
    +Drop(name : string)
    +Find(name : string) Index
    +FindBestIndex(query : Query) Index
    +Rebuild(index : Index)
}

class Query {
}

SchemaService --> ITableBuilder : uses
SchemaService --> IConstraintFactory : uses
SchemaService --> IIndexFactory : uses
SchemaService --> Schema : manages DDL
SchemaService --> Table : creates/drops

ITableBuilder <|.. TableBuilder
IConstraintFactory <|.. ConstraintFactory
IIndexFactory <|.. IndexFactory

TableBuilder --> Table : builds
ConstraintFactory --> Constraint : creates
IndexFactory --> Index : creates

RecordManager --> Table : reads schema from
RecordManager --> Row : reads/writes
IndexManager --> Index : manages

ICatalogIterator <|.. CatalogIterator

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

IIterableCatalog <|.. Database
IIterableCatalog <|.. Schema
IIterableCatalog <|.. Table

CatalogIterator o-- ICatalogComposite : collection
CatalogIterator --> ICatalogComponent : returns

IIterableCatalog --> ICatalogIterator : creates
SchemaService --> IIterableCatalog : requests iterator
SchemaService --> ICatalogIterator : traverses
```

## Sequence Diagrams (Database Manager & Metadata)

### 1. Hierarchy Management (Composite Pattern)

**Application:** Models the metadata tree: Database → Schema → Table → Column.

**Why apply?** Composite Pattern structures data into a tree form, providing uniform Add/Remove functions. The diagram below shows assigning objects together to form a parent-child structure, making it easy to access the entire branch (e.g., `GetSchemas()`, `GetTables()`).

```mermaid
classDiagram
direction TB

class Database {
    +DatabaseId : int
    +Name : string
    +Owner : string
    +Schemas : IReadOnlyList~Schema~
    +Database(id : int, name : string, owner : string)
    +CreateSchema(name : string) Schema
    +DropSchema(name : string)
    +GetSchema(name : string) Schema
    +GetSchemas() IReadOnlyList~Schema~
    +AddSchema(schema : Schema)
    +RemoveSchema(schema : Schema)
    +Backup(path : string, fileManager : IFileManager)
    +Restore(path : string, fileManager : IFileManager)
}

class Schema {
    +SchemaId : int
    +Name : string
    +Parent : Database
    +Tables : IReadOnlyCollection~Table~
    +Views : IReadOnlyCollection~View~
    +Procedures : IReadOnlyCollection~StoredProcedure~
    +Sequences : IReadOnlyCollection~Sequence~
    +Schema(name : string)
    +AddTable(table : Table)
    +DropTable(name : string)
    +GetTable(name : string) Table
    +GetTables() IReadOnlyCollection~Table~
    +CreateView(view : View)
    +DropView(name : string)
    +CreateProcedure(proc : StoredProcedure)
    +DropProcedure(name : string)
    +CreateSequence(seq : Sequence)
}

class Table {
    +TableId : int
    +Name : string
    +Parent : Schema
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~
    +Table(name : string)
    +AddColumn(col : Column)
    +RemoveColumn(name : string)
    +GetColumn(name : string) Column
    +GetColumns() IReadOnlyCollection~Column~
    +AddConstraint(constraint : Constraint)
    +RemoveConstraint(name : string)
    +AddIndex(index : Index)
    +RemoveIndex(name : string)
    +AddPartition(partition : Partition)
    +DropPartition(name : string)
    +AddTrigger(trigger : Trigger)
    +RemoveTrigger(name : string)
}

class Column {
    +ColumnId : int
    +Name : string
    +Parent : Table
    +DataType : DataType
    +Nullable : bool
    +DefaultValue : object
}
class Constraint
class Index
class Partition
class Trigger
class View
class StoredProcedure
class Sequence

Database "1" *-- "*" Schema : Composite
Schema "1" *-- "*" Table : Composite
Schema "1" *-- "*" View : Leaf
Schema "1" *-- "*" StoredProcedure : Leaf
Schema "1" *-- "*" Sequence : Leaf
Table "1" *-- "*" Column : Leaf
Table "1" *-- "*" Constraint : Leaf
Table "1" *-- "*" Index : Leaf
Table "1" *-- "*" Partition : Leaf
Table "1" *-- "*" Trigger : Leaf
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

**Application:** Initializes tables via `TableBuilder` from DDL syntax.

**Why apply?** Initializing a Table object requires many properties. `TableBuilder` helps gather parameters gradually (Columns, Primary Keys) and only creates the `TableMetadata` object in the final step, making the code coherent and readable.

```mermaid
classDiagram
direction LR

%% =====================================================
%% Builder Pattern
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
    +Reset(name : string)
    +AddColumn(column : Column)
    +AddConstraint(constraint : Constraint)
    +AddIndex(index : Index)
    +AddPartition(partition : Partition)
    +AddTrigger(trigger : Trigger)
    +Build() Table
}

class TableDirector {
    <<Director>>
    -builder : ITableBuilder
    -constraintFactory : IConstraintFactory
    -indexFactory : IIndexFactory
    +Construct(definition : TableDefinition) Table
}

class TableDefinition {
    <<Construction Data>>
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~ConstraintOptions~
    +Indexes : IReadOnlyCollection~IndexOptions~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~
}

class Table {
    <<Product>>
    +TableId : int
    +Name : string
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~
}

class Column
class Constraint
class Index
class Partition
class Trigger

class IConstraintFactory {
    <<Factory>>
    +Create(options : ConstraintOptions) Constraint
}

class IIndexFactory {
    <<Factory>>
    +Create(options : IndexOptions) Index
}

class ConstraintOptions
class IndexOptions

ITableBuilder <|.. TableBuilder
TableDirector --> ITableBuilder : directs
TableDirector --> IConstraintFactory : creates constraints
TableDirector --> IIndexFactory : creates indexes
TableDirector --> TableDefinition : reads

TableBuilder --> Table : builds

Table *-- Column
Table *-- Constraint
Table *-- Index
Table *-- Partition
Table *-- Trigger

TableDefinition --> Column
TableDefinition --> ConstraintOptions
TableDefinition --> IndexOptions
TableDefinition --> Partition
TableDefinition --> Trigger
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Service as SchemaService
    participant Director as TableDirector
    participant Builder as ITableBuilder
    participant CFactory as IConstraintFactory
    participant IFactory as IIndexFactory
    participant Schema

    Client->>Service: CreateTable(schema, definition)

    Service->>Director: Construct(definition)
    activate Director

    Director->>Builder: Reset(definition.Name)

    loop Mỗi Column
        Director->>Builder: AddColumn(column)
    end

    loop Mỗi ConstraintOptions
        Director->>CFactory: Create(options)
        CFactory-->>Director: Constraint
        Director->>Builder: AddConstraint(constraint)
    end

    loop Mỗi IndexOptions
        Director->>IFactory: Create(options)
        IFactory-->>Director: Index
        Director->>Builder: AddIndex(index)
    end

    loop Mỗi Partition
        Director->>Builder: AddPartition(partition)
    end

    loop Mỗi Trigger
        Director->>Builder: AddTrigger(trigger)
    end

    Director->>Builder: Build()
    Builder-->>Director: Table

    deactivate Director
    Director-->>Service: Table

    Service->>Schema: AddTable(table)
    Schema-->>Service: Success

    Service-->>Client: Table
```

### 3. Constraint Validation (Strategy Pattern)

**Application:** Evaluates Row validity based on various types of Constraints.

**Why apply?** By applying the Strategy Pattern via the `IConstraint` interface, `RecordManager` doesn't need to care about internal detailed logic (Primary Key checks for duplicates, Check evaluates expressions, Foreign Key checks reference table). It just calls `Validate(row)` and handles the polymorphic result.

```mermaid
classDiagram
direction TB

class RecordManager {
    +Insert(table : Table, row : Row)
    +Update(table : Table, rid : RID, row : Row)
}

class Table {
    +Constraints : IReadOnlyCollection~Constraint~
}

class Constraint {
    <<abstract>>
    +Name : string
    +Validate(row : Row) bool
}

class PrimaryKey {
    +Validate(row : Row) bool
}

class ForeignKey {
    +Validate(row : Row) bool
}

class UniqueConstraint {
    +Validate(row : Row) bool
}

class CheckConstraint {
    +Validate(row : Row) bool
}
class IRowKeyExtractor {
    <<interface>>
    +ExtractKey(row : Row, columns : List~Column~) object
}

class Index

RecordManager --> Table : Context uses
Table "1" *-- "*" Constraint : holds Strategies
Constraint <|-- PrimaryKey : ConcreteStrategy
Constraint <|-- ForeignKey : ConcreteStrategy
Constraint <|-- UniqueConstraint : ConcreteStrategy
Constraint <|-- CheckConstraint : ConcreteStrategy
PrimaryKey --> IRowKeyExtractor
UniqueConstraint --> IRowKeyExtractor
PrimaryKey --> Index
UniqueConstraint --> Index
ForeignKey --> Table
```

```mermaid
sequenceDiagram
    autonumber

    participant RecordMgr as RecordManager
    participant Table as Table
    participant PK as PrimaryKey
    participant Unique as UniqueConstraint
    participant FK as ForeignKey
    participant Check as CheckConstraint

    RecordMgr->>Table: GetConstraints()

    loop For each Constraint

        alt Primary Key
            Table-->>RecordMgr: PrimaryKey
            RecordMgr->>PK: Validate(row)
            PK-->>RecordMgr: true
        else Unique
            Table-->>RecordMgr: UniqueConstraint
            RecordMgr->>Unique: Validate(row)
            Unique-->>RecordMgr: true
        else Foreign Key
            Table-->>RecordMgr: ForeignKey
            RecordMgr->>FK: Validate(row)
            FK-->>RecordMgr: true
        else Check
            Table-->>RecordMgr: CheckConstraint
            RecordMgr->>Check: Validate(row)
            Check-->>RecordMgr: true
        end

    end

    RecordMgr-->>RecordMgr: Insert Row
```

### 4. Dynamic Allocation (Factory Method Pattern)

**Application:** Allocates objects like Index and Constraint automatically during DDL execution.

**Why apply?** Delegates the creation of a specific Index (BTree or Hash) to `IndexFactory`. The client doesn't need to know the internal initialization logic, just passes in the desired Index type and receives a common `IIndex` interface back.

```mermaid
classDiagram
direction TB
class SchemaService {
    -constraintFactory : IConstraintFactory
    -indexFactory : IIndexFactory
}

class IConstraintFactory {
    <<interface>>
    +Create(type : ConstraintType, options : ConstraintOptions) Constraint
}

class ConstraintFactory {
    +Create(type : ConstraintType, options : ConstraintOptions) Constraint
}

class ConstraintType{
    <<enumeration>>
    PRIMARY_KEY
    UNIQUE
    FOREIGN_KEY
    CHECK
}

class ConstraintOptions{
    +Columns : List~Column~
    +ReferenceTable : Table
    +ReferenceColumns : List~Column~
    +Expression : string
}

class IIndexFactory {
    <<interface>>
    +Create(type : IndexType, options : IndexOptions) Index
}

class IndexFactory {
    +Create(type : IndexType, options : IndexOptions) Index
}

class IndexType{
    <<enumeration>>
    BTREE
    HASH
    BITMAP
}

class IndexOptions{
    +Name : string
    +Columns : List~Column~
    +Unique : bool
}

class Constraint
class PrimaryKey
class Index
class BTreeIndex

SchemaService --> IConstraintFactory : Creator
SchemaService --> IIndexFactory : Creator
IConstraintFactory <|.. ConstraintFactory
IIndexFactory <|.. IndexFactory
ConstraintFactory --> Constraint : creates
IndexFactory --> Index : creates
ConstraintFactory --> ConstraintType
ConstraintFactory --> ConstraintOptions
IndexFactory --> IndexType
IndexFactory --> IndexOptions
Constraint <|-- PrimaryKey
Index <|-- BTreeIndex
```

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant Service as SchemaService
    participant Factory as IndexFactory
    participant BTree as BTreeIndex
    participant Table as Table
    Client->>Service: CreateIndex(type="BTree")
    Service->>Factory: Create(BTree)
    activate Factory
    Factory->>BTree: new BTreeIndex()
    BTree-->>Factory: Index
    deactivate Factory
    Factory-->>Service: Index
    Service->>Table: AddIndex(Index)
    Table-->>Client: Success
```

### 5. Hierarchy Traversal (Iterator Pattern)

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

ICatalogComponent <|-- ICatalogComposite

class ICatalogIterator {
    <<Iterator>>
    +GetNext() ICatalogComponent
    +HasMore() bool
}

class CatalogIterator {
    <<Concrete Iterator>>
    -collection : ICatalogComposite
    -iterationState
    +CatalogIterator(collection : ICatalogComposite)
    +GetNext() ICatalogComponent
    +HasMore() bool
}

class IIterableCatalog {
    <<Iterable Collection>>
    +CreateIterator() ICatalogIterator
}

class Database {
    +DatabaseId : int
    +Name : string
    +Owner : string
    +Schemas : IReadOnlyList~Schema~
    +Database(id : int, name : string, owner : string)
    +CreateSchema(name : string) Schema
    +DropSchema(name : string)
    +GetSchema(name : string) Schema
    +GetSchemas() IReadOnlyList~Schema~
    +AddSchema(schema : Schema)
    +RemoveSchema(schema : Schema)
    +Backup(path : string, fileManager : IFileManager)
    +Restore(path : string, fileManager : IFileManager)
    +CreateIterator() ICatalogIterator
}

class Schema {
    +SchemaId : int
    +Name : string
    +Parent : Database
    +Tables : IReadOnlyCollection~Table~
    +Views : IReadOnlyCollection~View~
    +Procedures : IReadOnlyCollection~StoredProcedure~
    +Sequences : IReadOnlyCollection~Sequence~
    +Schema(name : string)
    +AddTable(table : Table)
    +DropTable(name : string)
    +GetTable(name : string) Table
    +GetTables() IReadOnlyCollection~Table~
    +CreateView(view : View)
    +DropView(name : string)
    +CreateProcedure(proc : StoredProcedure)
    +DropProcedure(name : string)
    +CreateSequence(seq : Sequence)
    +CreateIterator() ICatalogIterator
}

class Table {
    +TableId : int
    +Name : string
    +Parent : Schema
    +Columns : IReadOnlyCollection~Column~
    +Constraints : IReadOnlyCollection~Constraint~
    +Indexes : IReadOnlyCollection~Index~
    +Partitions : IReadOnlyCollection~Partition~
    +Triggers : IReadOnlyCollection~Trigger~
    +Table(name : string)
    +AddColumn(col : Column)
    +RemoveColumn(name : string)
    +GetColumn(name : string) Column
    +GetColumns() IReadOnlyCollection~Column~
    +AddConstraint(constraint : Constraint)
    +RemoveConstraint(name : string)
    +AddIndex(index : Index)
    +RemoveIndex(name : string)
    +AddPartition(partition : Partition)
    +DropPartition(name : string)
    +AddTrigger(trigger : Trigger)
    +RemoveTrigger(name : string)
    +CreateIterator() ICatalogIterator
}

class Column {
    +ColumnId : int
    +Name : string
    +Parent : Table
    +DataType : DataType
    +Nullable : bool
    +DefaultValue : object
    +SetDataType(type : DataType)
    +SetNullable(nullable : bool)
    +SetDefaultValue(value : object)
    +Rename(newName : string)
    +ValidateValue(value : object) bool
}

class Constraint {
    <<abstract>>
    +Name : string
    +Validate(row : Row) bool
}

class Index {
    <<abstract>>
    +Name : string
    +Insert(key, rid)
    +Delete(key)
    +Search(key)
}

class Partition {
    +PartitionKey : string
    +PartitionType : string
    +InsertRecord(row : Row)
    +DropPartition(name : string)
    +GetPartition(key : object) Partition
}

class Trigger {
    +Name : string
    +Event : TriggerEvent
    +Timing : TriggerTiming
    +Body : string
    +Execute(context : TriggerContext)
}

class View {
    +ViewId : int
    +Name : string
    +QueryDefinition : string
    +View(name : string)
    +Compile() ExecutionPlan
    +Execute() ResultCursor
}

class StoredProcedure {
    +Name : string
    +Parameters : IReadOnlyCollection~Column~
    +Body : string
    +StoredProcedure(name : string)
    +Compile()
    +Execute(args : object[]) ResultCursor
}

class Sequence {
    +Name : string
    +CurrentValue : long
    +Increment : long
    +Sequence(name : string)
    +NextValue() long
    +Reset()
}

class SchemaService {
    <<Client>>
}

ICatalogIterator <|.. CatalogIterator

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

IIterableCatalog <|.. Database
IIterableCatalog <|.. Schema
IIterableCatalog <|.. Table

CatalogIterator o-- ICatalogComposite : collection
CatalogIterator --> ICatalogComponent : returns

IIterableCatalog --> ICatalogIterator : creates
SchemaService --> IIterableCatalog : requests iterator
SchemaService --> ICatalogIterator : traverses
```

```mermaid
sequenceDiagram
    autonumber

    participant Client as SchemaService
    participant Composite as Database / Schema / Table
    participant Iterator as CatalogIterator
    participant Component as ICatalogComponent

    Client->>Composite: CreateIterator()
    Composite->>Iterator: Create(this)
    Iterator-->>Composite: ICatalogIterator
    Composite-->>Client: ICatalogIterator

    loop HasMore() = true
        Client->>Iterator: HasMore()
        Iterator-->>Client: true

        Client->>Iterator: GetNext()
        Iterator->>Component: Get next component
        Component-->>Iterator: Component
        Iterator-->>Client: ICatalogComponent

        Client->>Client: Process component
    end

    Client->>Iterator: HasMore()
    Iterator-->>Client: false
```

### 6. DDL Execution (Command Pattern)

**Application:** Encapsulates Data Definition Language (DDL) requests (like `CreateTable`, `CreateSchema`) as standalone objects that contain all information about the request.

**Why apply?** The Command Pattern allows the `QueryProcessor` to parameterize the `DdlCommandExecutor` with different requests, decouple the invoker from the receivers (`SchemaService`, `DatabaseService`), and supports future capabilities like queuing, logging, or undoing operations.

```mermaid
classDiagram
direction LR

%% =====================================================
%% Command Pattern
%% =====================================================

class IDdlCommand {
    <<Command>>
    +Execute() DdlResult
}

class CreateTableCommand {
    <<Concrete Command>>
    -receiver : ISchemaService
    -schema : Schema
    -definition : TableDefinition
    +CreateTableCommand(receiver, schema, definition)
    +Execute() DdlResult
}

class CreateSchemaCommand {
    <<Concrete Command>>
    -receiver : IDatabaseService
    -database : Database
    -schemaName : string
    +CreateSchemaCommand(receiver, database, schemaName)
    +Execute() DdlResult
}

class DdlCommandExecutor {
    <<Invoker>>
    +Execute(command : IDdlCommand) DdlResult
}

class QueryProcessor {
    <<Client>>
    +CreateCommand(query : Query) IDdlCommand
}

class DdlResult {
    <<Result>>
    +Success : bool
    +Message : string
    +AffectedObject : ICatalogComponent
}

%% =====================================================
%% Receivers
%% =====================================================

class IDatabaseService {
    <<Receiver>>
    +CreateSchema(database : Database, name : string) Schema
}

class DatabaseService {
    -catalog : CatalogManager
    +CreateSchema(database : Database, name : string) Schema
}

class ISchemaService {
    <<Receiver>>
    +CreateTable(schema : Schema, definition : TableDefinition) Table
}

class SchemaService {
    -director : TableDirector
    -catalog : CatalogManager
    -storage : StorageEngine
    +CreateTable(schema : Schema, definition : TableDefinition) Table
}

%% =====================================================
%% Related Domain Objects
%% =====================================================

class Database
class Schema
class TableDefinition
class TableDirector
class CatalogManager
class StorageEngine
class Query
class ICatalogComponent

IDdlCommand <|.. CreateTableCommand
IDdlCommand <|.. CreateSchemaCommand

IDatabaseService <|.. DatabaseService
ISchemaService <|.. SchemaService

DdlCommandExecutor --> IDdlCommand : executes

QueryProcessor --> IDdlCommand : creates
QueryProcessor --> DdlCommandExecutor : submits

CreateTableCommand --> ISchemaService : receiver
CreateTableCommand --> Schema : target
CreateTableCommand --> TableDefinition : carries

CreateSchemaCommand --> IDatabaseService : receiver
CreateSchemaCommand --> Database : target

SchemaService --> TableDirector : constructs table
SchemaService --> Schema : adds table
SchemaService --> CatalogManager : registers metadata
SchemaService --> StorageEngine : allocates storage

DatabaseService --> Database : adds schema
DatabaseService --> CatalogManager : registers metadata

IDdlCommand --> DdlResult : returns
DdlResult --> ICatalogComponent : contains
```

```mermaid
sequenceDiagram
    autonumber

    participant QP as QueryProcessor
    participant Executor as DdlCommandExecutor
    participant Command as CreateTableCommand
    participant Service as ISchemaService
    participant Director as TableDirector
    participant Builder as ITableBuilder
    participant CFactory as IConstraintFactory
    participant IFactory as IIndexFactory
    participant Storage as StorageEngine
    participant Schema
    participant Catalog as CatalogManager

    QP->>Command: new(receiver, schema, definition)
    QP->>Executor: Execute(command)

    Executor->>Command: Execute()
    activate Command

    Command->>Service: CreateTable(schema, definition)
    activate Service

    Service->>Director: Construct(definition)
    activate Director

    Director->>Builder: Reset(definition.Name)

    loop Columns
        Director->>Builder: AddColumn(column)
    end

    loop Constraint options
        Director->>CFactory: Create(options)
        CFactory-->>Director: Constraint
        Director->>Builder: AddConstraint(constraint)
    end

    loop Index options
        Director->>IFactory: Create(options)
        IFactory-->>Director: Index
        Director->>Builder: AddIndex(index)
    end

    loop Partitions
        Director->>Builder: AddPartition(partition)
    end

    loop Triggers
        Director->>Builder: AddTrigger(trigger)
    end

    Director->>Builder: Build()
    Builder-->>Director: Table

    deactivate Director
    Director-->>Service: Table

    Service->>Storage: Allocate(table)
    Storage-->>Service: Success

    Service->>Schema: AddTable(table)
    Schema-->>Service: Success

    Service->>Catalog: Register(table)
    Catalog-->>Service: Success

    Service-->>Command: Table
    deactivate Service

    Command-->>Executor: DdlResult
    deactivate Command

    Executor-->>QP: DdlResult
```