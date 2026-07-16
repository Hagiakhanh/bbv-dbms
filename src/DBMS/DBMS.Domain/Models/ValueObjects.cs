namespace DBMS.Domain.Models;

using System;

public class PageId : IEquatable<PageId>
{
    public FileId FileId { get; }
    public int PageNumber { get; }

    public PageId(FileId fileId, int pageNumber)
    {
        FileId = fileId;
        PageNumber = pageNumber;
    }

    public bool Equals(PageId? other) => throw new NotImplementedException();
    public override string ToString() => throw new NotImplementedException();
}

public class LSN
{
    public long Value { get; }

    public LSN(long value) => Value = value;

    public bool IsAfter(LSN other) => throw new NotImplementedException();
    public LSN Next() => throw new NotImplementedException();
}

public class TransactionId
{
    public long Id { get; }

    public TransactionId(long id) => Id = id;

    public bool IsValid() => throw new NotImplementedException();
}

public class RID : IEquatable<RID>
{
    public PageId PageId { get; }
    public int SlotNumber { get; }

    public RID(PageId pageId, int slotNumber)
    {
        PageId = pageId;
        SlotNumber = slotNumber;
    }

    public bool Equals(RID? other) => throw new NotImplementedException();
}

public class DatabaseName
{
    public string Value { get; }
    public DatabaseName(string value) => Value = value;
    public void Validate() => throw new NotImplementedException();
}

public class AbsolutePath
{
    public string Path { get; }
    public AbsolutePath(string path) => Path = path;

    public bool Exists() => throw new NotImplementedException();
    public AbsolutePath Resolve(string child) => throw new NotImplementedException();
}

public class PoolSize
{
    public int Value { get; }
    public PoolSize(int value) => Value = value;
    public void Validate() => throw new NotImplementedException();
}

public class BTreeDegree
{
    public int Value { get; }
    public BTreeDegree(int value) => Value = value;
    public void Validate() => throw new NotImplementedException();
}
