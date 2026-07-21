# Design Patterns in DBMS

This document outlines the Design Patterns implemented within various core components of the BBV-DBMS.

## Visual Summary

| Module | Feature | Pattern | Application |
| :--- | :--- | :--- | :--- |
| **Database & Metadata** | Hierarchy Management | **Composite** | Quản lý kiến trúc phân cấp: `DatabaseComposite` → `SchemaComposite` → `TableComposite` → `ColumnLeaf`. |
| **Database & Metadata** | Metadata Initialization | **Builder** | Sử dụng `TableDefBuilder` để thiết lập từng thuộc tính của bảng thay vì dùng constructor dài. |
| **Database & Metadata** | Constraint Validation | **Strategy** | `PrimaryKeyConstraint`, `UniqueConstraint`, `ForeignKeyConstraint` triển khai cùng interface. |
| **Database & Metadata** | Dynamic Allocation | **Factory Method** | Khởi tạo động các Index, Constraint thông qua `ObjectFactoryProvider` trong lúc chạy DDL. |
| **Database & Metadata** | Metadata Persistence | **Repository** | Cung cấp interface `ICatalogRepository` để lấy cấu trúc bảng mà không dính dáng đến Storage vật lý. |
| **Database & Metadata** | Fast Duplication | **Prototype** | Hỗ trợ nhân bản bảng (lệnh `CREATE TABLE LIKE`) thông qua hàm `DeepCopy()`. |
| **Database & Metadata** | Cache Invalidation | **Observer** | Dùng `CatalogEventBroker` báo cho `PlanCacheManager` biết khi có thay đổi cấu trúc để xóa cache. |
| **Database Manager** | System Initialization | **Facade** | `DbEngineFacade` gom nhóm các bước khởi động phức tạp của Disk, Storage, và Catalog. |
| **Database Manager** | DDL Operations | **Command** | Đóng gói các lệnh tạo/xóa Database thành `CreateDatabaseAction` để dễ dàng undo/redo hoặc log. |
---

```mermaid
classDiagram
direction LR

%% =====================================================
%% Composite Hierarchy & DB Objects
%% =====================================================

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
```

## Sequence Diagrams (Database Manager & Metadata)

### 1. Hierarchy Management (Composite Pattern)

**Application:** Mô hình hóa cây siêu dữ liệu: Database → Schema → Table → Column.

**Tại sao áp dụng?** Composite Pattern cấu trúc hoá dữ liệu thành dạng cây, cung cấp các hàm Add/Remove đồng nhất. Biểu đồ dưới đây thể hiện việc gán ghép các object lại với nhau để hình thành cấu trúc cha-con, giúp dễ dàng truy xuất toàn bộ nhánh (ví dụ: `GetSchemas()`, `GetTables()`).

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

**Application:** Khởi tạo bảng qua `TableBuilder` từ cú pháp DDL.

**Tại sao áp dụng?** Khởi tạo một đối tượng Table cần rất nhiều thuộc tính. `TableBuilder` giúp thu thập dần dần các thông số (Cột, Khóa chính) và chỉ tạo ra object `TableMetadata` ở bước cuối cùng, giúp code mạch lạc và dễ đọc hơn.

```mermaid
classDiagram
direction LR

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

class Table {
    +TableId : int
    +Name : string
}

class SchemaService {
    -builder : ITableBuilder
    +CreateTable(schema : Schema, name : string) Table
}
class Column
class Constraint
class Index
class Partition
class Trigger

SchemaService --> ITableBuilder : Director
ITableBuilder <|.. TableBuilder : ConcreteBuilder
TableBuilder --> Table : creates Product
TableBuilder --> Column : uses
TableBuilder --> Constraint : uses
TableBuilder --> Index : uses
TableBuilder --> Partition : uses
TableBuilder --> Trigger : uses
```

```mermaid
sequenceDiagram
    autonumber

    actor Client
    participant Service as SchemaService
    participant Builder as TableBuilder
    participant Table as Table
    participant Column as Column
    participant PK as PrimaryKey
    participant Index as BTreeIndex
    participant Schema as Schema

    Client->>Service: CreateTable(schema, tableDef)

    Service->>Builder: Create(tableDef)

    activate Builder

    loop Build Columns
        Builder->>Column: new Column()
        Column-->>Builder: Column
    end

    loop Build Constraints
        Builder->>PK: new PrimaryKey()
        PK-->>Builder: Constraint
    end

    loop Build Indexes
        Builder->>Index: new BTreeIndex()
        Index-->>Builder: Index
    end

    Builder->>Table: Build()

    Table-->>Builder: Table

    deactivate Builder

    Builder-->>Service: Table

    Service->>Schema: AddTable(Table)

    Schema-->>Client: Success
```

### 3. Constraint Validation (Strategy Pattern)

**Application:** Đánh giá tính hợp lệ của Row dựa trên nhiều loại Constraint khác nhau.

**Tại sao áp dụng?** Bằng cách áp dụng Strategy Pattern thông qua interface `IConstraint`, RecordManager không cần quan tâm chi tiết logic bên trong (Primary Key kiểm tra trùng lặp, Check kiểm tra biểu thức, Foreign Key kiểm tra bảng tham chiếu). Nó chỉ cần gọi `Validate(row)` và xử lý kết quả trả về đa hình.

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

**Application:** Phân bổ các object như Index, Constraint tự động lúc thi hành DDL.

**Tại sao áp dụng?** Giao phó việc tạo Index cụ thể (BTree hay Hash) cho `IndexFactory`. Client không cần biết logic khởi tạo bên trong, chỉ cần truyền vào loại Index mong muốn và nhận lại một interface `IIndex` chung.

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

### 5. Metadata Persistence (Repository Pattern)

**Application:** Tập trung logic lưu và truy xuất cấu trúc dữ liệu.

**Tại sao áp dụng?** Cô lập các lớp Domain khỏi thư viện đọc/ghi file. Các tầng xử lý truy vấn chỉ cần yêu cầu `ICatalogRepository` trả về thông tin bảng (`TableMetadata`), mà không cần quan tâm nó được load từ hệ thống phân trang bộ nhớ bên dưới như thế nào.

```mermaid
sequenceDiagram
    actor QueryPlanner
    participant Repo as ICatalogRepository
    participant Engine as StorageSubsystem

    QueryPlanner->>Repo: FetchTableDefinition("Employees")
    activate Repo
    Repo->>Engine: ReadRecord(SysTables_Id, "Employees")
    Engine-->>Repo: Byte Array
    Repo->>Repo: Deserialize to TableMetadata
    Repo-->>QueryPlanner: TableMetadata Instance
    deactivate Repo
```

### 6. Fast Duplication (Prototype Pattern)

**Application:** Sao chép nguyên mẫu Schema hoặc Bảng.

**Tại sao áp dụng?** Trong các tác vụ như `CREATE TABLE AS SELECT...`, việc clone lại toàn bộ cấu hình của một TableMetadata hiện có qua hàm `DeepCopy()` sẽ nhanh và ít rủi ro hơn nhiều so với việc trích xuất và gán lại từng tham số thông qua Builder.

```mermaid
sequenceDiagram
    actor Engine
    participant Src as TableMetadata (Source)
    participant Dest as TableMetadata (Cloned)

    Engine->>Src: DeepCopy()
    activate Src
    Src->>Dest: new TableMetadata()
    Src->>Dest: Copy Columns, Constraints, Indexes
    Src-->>Engine: Cloned Instance
    deactivate Src
```

### 7. Cache Invalidation (Observer Pattern)

**Application:** Thông báo thay đổi hệ thống cấu trúc bảng.

**Tại sao áp dụng?** Thay vì service thay đổi bảng phải gọi một đống hàm clear cache, Observer Pattern cho phép các Broker phát đi event `SchemaChangedEvent`. Bất kì module nào đăng ký (như Plan Cache hay Module thống kê dữ liệu) sẽ tự bắt sự kiện và xử lý bộ đệm của nó.

```mermaid
sequenceDiagram
    actor ExecEngine
    participant Broker as CatalogEventBroker
    participant PlanCache as PlanCacheManager
    participant StatTracker as DbStatisticsTracker

    %% Initialization
    PlanCache->>Broker: RegisterListener(SchemaChangedEvent)
    StatTracker->>Broker: RegisterListener(SchemaChangedEvent)

    %% Alter Table
    ExecEngine->>Broker: PublishSchemaChange(TableModifiedInfo)
    activate Broker
    Broker->>PlanCache: OnSchemaChanged(TableModifiedInfo)
    PlanCache->>PlanCache: EvictRelatedQueries()
    Broker->>StatTracker: OnSchemaChanged(TableModifiedInfo)
    StatTracker->>StatTracker: FlagStatsForRecalculation()
    Broker-->>ExecEngine: Events Dispatched
    deactivate Broker
```

### 8. System Initialization (Facade Pattern)

**Application:** `DbEngineFacade` đóng vai trò là cửa ngõ duy nhất để khởi động hệ thống.

**Tại sao áp dụng?** Việc bật hoặc tắt một instance cơ sở dữ liệu đòi hỏi phải gọi tuần tự rất nhiều module bên dưới. Facade cung cấp một điểm truy cập duy nhất, giúp code ở client (như CLI hoặc giao diện) trở nên cực kỳ đơn giản và không bị phụ thuộc vào các module cấp thấp.

```mermaid
sequenceDiagram
    actor App
    participant Engine as DbEngineFacade
    participant Disk as IDiskManager
    participant Store as IStorageSubsystem
    participant Cat as IDatabaseCatalog
    
    App->>Engine: MountDatabase("UserDB")
    activate Engine
    Engine->>Disk: CheckDatabaseFilesExist("UserDB")
    Engine->>Store: BootStorageEngine("UserDB")
    Engine->>Cat: LoadSystemTables()
    Engine-->>App: DbConnection
    deactivate Engine
```

### 9. DDL Operations (Command Pattern)

**Application:** Đóng gói các lệnh thực thi cấu trúc thành các Action object.

**Tại sao áp dụng?** Thay vì viết thẳng logic tạo database trong controller, hệ thống đóng gói chúng thành `CreateDatabaseAction`. Việc này chia tách trách nhiệm rõ ràng giữa nơi nhận lệnh (Processor) và nơi thi hành, hỗ trợ tốt cho việc ghi log (WAL) hoặc khôi phục khi có lỗi.

```mermaid
sequenceDiagram
    actor App
    participant Processor as DdlCommandProcessor
    participant Cmd as CreateDatabaseAction
    participant Cat as IDatabaseCatalog
    
    App->>Processor: SubmitCommand(new CreateDatabaseAction("UserDB"))
    activate Processor
    Processor->>Cmd: ExecuteAction()
    activate Cmd
    Cmd->>Cat: AllocateNewDatabaseRecord()
    Cat-->>Cmd: true
    Cmd-->>Processor: true
    deactivate Cmd
    Processor-->>App: Success
    deactivate Processor
```