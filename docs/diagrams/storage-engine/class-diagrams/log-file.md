## Group 6 — Write-Ahead Log (Log File / WAL)
*Role in Sequence (Write Path): This step occurs immediately before the actual data is written. The WalWriter must call AppendLog() before AllocateSlotEntry() is invoked, adhering to the ACID principle.*

```mermaid
classDiagram
    namespace LogFile {
        class LogManager {
            +GetCurrentLsn() long
            +Recover() void
        }
        class IWalWriter {
            <<interface>>
            +AppendLog(record: LogRecord, payload: byte[]) long
            +FlushBuffer() void
            +EnforceWalRule(requiredLsn: long) void
        }
        class WalWriter {
            -_walBuffer: WalBuffer
            -_lsnGenerator: LSNGenerator
            -_diskManager: IDiskManager
        }
        class WalBuffer {
            -_buffer: byte[]
            -_bufferSize: int
            -_writePos: int
            -_currentLsn: long
            +Write(record: LogRecord, payload: byte[]) long
            +Read(lsn: long) LogRecord
            +Flush(stream: Stream) void
            +IsFull() bool
            +GetCurrentLsn() long
        }
        class LSNGenerator {
            -_currentLsn: long
            +NextLsn() long
            +GetCurrentLsn() long
        }
        class LogRecord {
            <<struct>>
            +Lsn: long
            +PrevLsn: long
            +TxId: int
            +Type: LogRecordType
        }
    }
    LogManager *-- IWalWriter
    LogManager *-- LSNGenerator
    IWalWriter <|-- WalWriter
    WalWriter --> WalBuffer : writes to
    WalWriter --> LSNGenerator : uses
    IWalWriter --> LogRecord : Appends
```