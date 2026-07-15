using System;
using DBMS.Domain.Foundation;

namespace DBMS.Domain.DataFile;

public struct FileMetadata
{
    public int FileId { get; set; }
    public string FilePath { get; set; }
    public FileType Type { get; set; }
}
