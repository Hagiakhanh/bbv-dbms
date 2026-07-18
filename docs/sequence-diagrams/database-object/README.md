# Database Objects

## 1. Updated Class Diagram

*(Đã bổ sung các thuộc tính và phương thức còn thiếu để hỗ trợ đầy đủ các Unit Test được định nghĩa)*

```mermaid
classDiagram
direction LR
class Database {
    +DatabaseId : int
    +Name : string
    +Owner : string
    +Schemas : List~Schema~
    +CreateSchema(name : string) Schema
    +DropSchema(name : string)
    +GetSchema(name : string) Schema
    +GetSchemas() List~Schema~
    +Backup(path : string)
    +Restore(path : string)
}
class Schema {
    +SchemaId : int
    +Name : string
    +Tables : List~Table~
    +Views : List~View~
    +Procedures : List~StoredProcedure~
    +Sequences : List~Sequence~
    +AddTable(table : Table)
    +DropTable(name : string)
    +GetTable(name : string) Table
    +GetTables() List~Table~
    +CreateView(view : View)
    +DropView(name : string)
    +CreateProcedure(proc : StoredProcedure)
    +DropProcedure(name : string)
    +CreateSequence(seq : Sequence)
}
class Table {
    +TableId : int
    +Name : string
    +Columns : List~Column~
    +Constraints : List~Constraint~
    +Indexes : List~Index~
    +Partitions : List~Partition~
    +Triggers : List~Trigger~
    +AddColumn(col : Column)
    +RemoveColumn(name : string)
    +GetColumn(name : string) Column
    +GetColumns() List~Column~
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
    +DataType : DataType
    +Nullable : bool
    +DefaultValue : object
    +SetDataType(type : DataType)
    +SetNullable(nullable : bool)
    +SetDefaultValue(value : object)
    +Rename(newName : string)
    +ValidateValue(value : object) bool
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
class PrimaryKey {
    +Columns : List~Column~
}
class ForeignKey {
    +ReferenceTable : Table
    +ReferenceColumns : List~Column~
}
class UniqueConstraint
class CheckConstraint {
    +Expression : string
}

class View {
    +ViewId : int
    +Name : string
    +QueryDefinition : string
    +Compile() ExecutionPlan
    +Execute() ResultCursor
}

class StoredProcedure {
    +Name : string
    +Parameters : List~Column~
    +Body : string
    +Compile()
    +Execute(args : object[]) ResultCursor
}

class Sequence {
    +Name : string
    +CurrentValue : long
    +Increment : long
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

Database "1" *-- "many" Schema
Schema "1" *-- "many" Table
Schema "1" *-- "many" View
Schema "1" *-- "many" StoredProcedure
Schema "1" *-- "many" Sequence
Table "1" *-- "many" Column
Table "1" *-- "many" Constraint
Table "1" *-- "many" Row
Table "1" *-- "many" Partition
Table "1" *-- "many" Trigger
Row *-- RID
Row *-- RecordData
Column --> DataType
Constraint <|-- PrimaryKey
Constraint <|-- ForeignKey
Constraint <|-- UniqueConstraint
Constraint <|-- CheckConstraint
ForeignKey --> Table

class SchemaService {
    -catalog : CatalogManager
    -storage : StorageEngine
    +CreateTable(schema : Schema, def : TableDef) Table
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
SchemaService --> Schema : manages DDL
SchemaService --> Table : creates/drops
RecordManager --> Table : reads schema from
RecordManager --> Row : reads/writes
```

---

## 2. Sequence Diagrams

### 2.1. Database & Schema Operations

#### Database Lifecycle (Create/Drop Schema, Backup/Restore)
Covers: `CreateSchema`, `DropSchema`, `GetSchema`, `GetSchemas`, `Backup`, `Restore`

```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant DB as Database
    participant Disk as FileSystem

    %% Create Schema
    Admin->>DB: CreateSchema("dbo")
    alt Schema exists
        DB-->>Admin: Throw DuplicateSchemaException
    else
        DB->>DB: Schemas.Add(new Schema)
        DB-->>Admin: Schema Created Successfully
    end

    %% Drop Schema
    Admin->>DB: DropSchema("temp")
    alt Schema not found
        DB-->>Admin: Throw SchemaNotFoundException
    else
        DB->>DB: Schemas.Remove(Schema)
        DB-->>Admin: Schema Dropped Successfully
    end

    %% Backup & Restore
    Admin->>DB: Backup("/path/to/backup.bak")
    DB->>Disk: Write Data to File
    DB-->>Admin: Backup Successful

    Admin->>DB: Restore("/path/to/backup.bak")
    DB->>Disk: Read Data from File
    DB->>DB: Overwrite Internal State
    DB-->>Admin: Restore Successful
```

#### Schema Operations (Add Table, View, Procedure, Sequence)
Covers: `AddTable`, `DropTable`, `GetTable`, `GetTables`, `CreateView`, `DropView`, `CreateProcedure`, `DropProcedure`, `CreateSequence`

```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant Sch as Schema
    
    %% Table Operations
    Admin->>Sch: AddTable(Table "Users")
    alt Duplicate Name
        Sch-->>Admin: Throw DuplicateTableException
    else
        Sch->>Sch: Tables.Add(Table)
        Sch-->>Admin: Table Added
    end
    
    Admin->>Sch: DropTable("OldTable")
    alt Not exists
        Sch-->>Admin: Throw TableNotFoundException
    else
        Sch->>Sch: Tables.Remove(Table)
        Sch-->>Admin: Table Dropped
    end

    %% Other Schema Objects
    Admin->>Sch: CreateView(View)
    Sch->>Sch: Views.Add(View)

    Admin->>Sch: CreateProcedure(StoredProcedure)
    Sch->>Sch: Procedures.Add(StoredProcedure)

    Admin->>Sch: CreateSequence(Sequence)
    Sch->>Sch: Sequences.Add(Sequence)
```

### 2.2. Table & Column Operations

#### Table Schema Adjustments
Covers: `AddColumn`, `RemoveColumn`, `AddConstraint`, `RemoveConstraint`, `AddIndex`, `RemoveIndex`, `AddPartition`, `DropPartition`, `AddTrigger`, `RemoveTrigger`

```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant Tbl as Table

    Admin->>Tbl: AddColumn(Column "Age")
    alt Duplicate Column
        Tbl-->>Admin: Throw DuplicateColumnException
    else
        Tbl->>Tbl: Columns.Add(Column)
    end

    Admin->>Tbl: AddConstraint(Constraint)
    Tbl->>Tbl: Constraints.Add(Constraint)

    Admin->>Tbl: AddIndex(Index)
    Tbl->>Tbl: Indexes.Add(Index)

    Admin->>Tbl: AddPartition(Partition)
    Tbl->>Tbl: Partitions.Add(Partition)

    Admin->>Tbl: AddTrigger(Trigger)
    Tbl->>Tbl: Triggers.Add(Trigger)
```

#### Column Attribute Management & Validation
Covers: `SetDataType`, `SetNullable`, `SetDefaultValue`, `Rename`, `ValidateValue`

```mermaid
sequenceDiagram
    autonumber
    participant System
    participant Col as Column

    System->>Col: SetDataType(DataType.INT)
    Col->>Col: DataType = INT
    System->>Col: SetNullable(false)
    System->>Col: SetDefaultValue(0)
    System->>Col: Rename("UserAge")

    %% Validation
    System->>Col: ValidateValue(100)
    alt Value matches INT & constraints
        Col-->>System: return true
    else Invalid value
        Col-->>System: Throw InvalidValueException
    end
```

### 2.3. Data Structures: Row, RecordData & RID

#### Row & RecordData Manipulation
Covers: `GetValue`, `SetValue`, `UpdateValue`, `Serialize`, `Deserialize`, `RID.Equals`

```mermaid
sequenceDiagram
    autonumber
    participant TxMgr as TransactionManager
    participant Rw as Row
    participant Data as RecordData
    participant PageRID as RID

    %% Updating a Row
    TxMgr->>Rw: UpdateValue(colId, newValue)
    alt invalid column
        Rw-->>TxMgr: Throw InvalidColumnException
    else
        Rw->>Data: Update byte array at colId offset
        Rw->>Rw: Version++ (Increment Version)
        Rw-->>TxMgr: Success
    end

    %% Serialization
    TxMgr->>Data: Serialize()
    Data->>Data: Convert fields to Byte[]
    Data-->>TxMgr: Return Byte[]

    %% RID Equivalence
    TxMgr->>PageRID: Equals(otherRID)
    alt PageId and Slot matching
        PageRID-->>TxMgr: true
    else
        PageRID-->>TxMgr: false
    end
```

### 2.4. Programmability & Automations

#### View, Stored Procedure, & Sequence Execution
Covers: `View.Compile`, `View.Execute`, `StoredProcedure.Compile`, `StoredProcedure.Execute`, `Sequence.NextValue`, `Sequence.Reset`

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant Vw as View
    participant Proc as StoredProcedure
    participant Seq as Sequence
    participant Exec as QueryExecutor

    %% View Execution
    Client->>Vw: Execute()
    Vw->>Vw: Compile() (Generate Plan)
    Vw->>Exec: Run(ExecutionPlan)
    Exec-->>Vw: ResultCursor
    Vw-->>Client: Expected Results

    %% Procedure Execution
    Client->>Proc: Execute(args)
    Proc->>Proc: Compile()
    Proc->>Exec: Run(Compiled Statements)
    Exec-->>Proc: Success / Cursor
    Proc-->>Client: Procedure Completed

    %% Sequence Generation
    Client->>Seq: NextValue()
    alt Value exceeds limit
        Seq-->>Client: Throw OverflowException
    else
        Seq->>Seq: CurrentValue += Increment
        Seq-->>Client: CurrentValue
    end
```

#### Triggers & Partitions
Covers: `Trigger.Execute`, `Partition.InsertRecord`

```mermaid
sequenceDiagram
    autonumber
    participant RecordMgr as RecordManager
    participant Tbl as Table
    participant Part as Partition
    participant Trg as Trigger
    participant Tx as Transaction

    %% Partition Routing
    RecordMgr->>Tbl: InsertRecord(Row)
    Tbl->>Part: InsertRecord(Row)
    alt Key not matching PartitionType
        Part-->>Tbl: Throw InvalidPartitionKeyException
    else
        Part->>Part: Physical Insertion Logic
    end

    %% Trigger Execution
    RecordMgr->>Trg: Execute(TriggerContext)
    Trg->>Trg: Check Trigger Event & Timing
    alt Condition Fails
        Trg-->>RecordMgr: Throw ConditionFailedException
    else
        Trg->>Trg: Execute Body
        alt Execution fails
            Trg->>Tx: AbortTransaction_OnFailure()
        end
    end
```

### 2.5. Integration Flow

#### DDL Object Creation Workflow (Database -> Schema -> Table -> Column)
Covers: `Database_CreateSchema_ShouldRegisterSchema`, `Schema_AddTable_ShouldRegisterTable`, `Table_AddColumn_ShouldRegisterColumn`, `Table_AddIndex_ShouldRegisterIndex`

```mermaid
sequenceDiagram
    autonumber
    actor Dev
    participant DB as Database
    participant Sch as Schema
    participant Tbl as Table

    Dev->>DB: CreateSchema("Sales")
    DB->>DB: Register Schema
    DB-->>Dev: Return Schema "Sales"

    Dev->>Sch: AddTable(Table "Orders")
    Sch->>Sch: Register Table
    Sch-->>Dev: Success

    Dev->>Tbl: AddColumn(Column "TotalAmount")
    Tbl->>Tbl: Register Column
    
    Dev->>Tbl: AddIndex(Index "Idx_Total")
    Tbl->>Tbl: Register Index
```
