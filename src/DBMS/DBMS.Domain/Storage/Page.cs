using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class Page
{
    public int PageId { get; set; }
    public Byte[] Data { get; set; }
    public bool IsDirty { get; set; }
    public int PinCount { get; set; }
}
