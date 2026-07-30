namespace DBMS.Domain.Storage.Proxies;

public class BufferFrame
{
    public int FrameId { get; set; }
    public Page? Page { get; set; }
    public int PinCount { get; set; }
    public bool IsDirty { get; set; }
}
