namespace DBMS.Domain.Storage.Proxy;

public class BufferFrame
{
    public int FrameId { get; set; }
    public Page? Page { get; set; }
    public int PinCount { get; set; }
    public bool IsDirty { get; set; }
}
