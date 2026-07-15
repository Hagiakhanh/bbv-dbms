using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using DBMS.Domain.DataFile;

namespace DBMS.Tests.DataFile;

public class DiskManagerTests : IDisposable
{
    private readonly DiskManager _diskMgr;
    private readonly string _tempPath;
    private const int PAGE_SIZE = 4096;

    public DiskManagerTests()
    {
        _diskMgr = new DiskManager();
        _tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".db");
    }

    public void Dispose()
    {
        if (File.Exists(_tempPath))
        {
            File.Delete(_tempPath);
        }
    }

    [Fact]
    public void CreateFile_ValidPath_CreatesPhysicalFile()
    {
        _diskMgr.CreateFile(_tempPath);
        File.Exists(_tempPath).Should().BeTrue();
    }

    [Fact]
    public void CreateFile_ValidPath_ReturnsNonNegativeFileId()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        fileId.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void CreateFile_NullPath_ThrowsArgumentNullException()
    {
        Action act = () => _diskMgr.CreateFile(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateFile_InvalidPath_ThrowsIOException()
    {
        Action act = () => _diskMgr.CreateFile(@"Z:\invalid\nonexist\a.db");
        act.Should().Throw<IOException>();
    }

    [Fact]
    public void CreateFile_AlreadyExists_ThrowsIOException()
    {
        _diskMgr.CreateFile(_tempPath);
        Action act = () => _diskMgr.CreateFile(_tempPath);
        act.Should().Throw<IOException>();
    }

    [Fact]
    public void WritePage_ThenReadPage_DataIsIdentical()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        byte[] writeBuffer = new byte[PAGE_SIZE];
        Array.Fill(writeBuffer, (byte)0xAB);
        
        _diskMgr.WritePage(fileId, 0, writeBuffer);
        
        byte[] readBuffer = new byte[PAGE_SIZE];
        _diskMgr.ReadPage(fileId, 0, readBuffer);
        
        readBuffer.Should().Equal(writeBuffer);
    }

    [Fact]
    public void WritePage_PageN_WritesAtCorrectOffset()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        byte[] writeBuffer = new byte[PAGE_SIZE];
        Array.Fill(writeBuffer, (byte)0xCD);
        
        _diskMgr.WritePage(fileId, 5, writeBuffer);
        
        byte[] readBuffer = new byte[PAGE_SIZE];
        _diskMgr.ReadPage(fileId, 5, readBuffer);
        readBuffer.Should().Equal(writeBuffer);
        
        new FileInfo(_tempPath).Length.Should().BeGreaterThanOrEqualTo(6 * PAGE_SIZE);
    }

    [Fact]
    public void ReadPage_UnwrittenPage_ReturnsZeroBytes()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        byte[] readBuffer = new byte[PAGE_SIZE];
        _diskMgr.ReadPage(fileId, 5, readBuffer);
        
        readBuffer.Should().OnlyContain(b => b == 0);
    }

    [Fact]
    public void WritePage_Page2_DoesNotCorruptPage0()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        
        byte[] write0 = new byte[PAGE_SIZE];
        Array.Fill(write0, (byte)0xAA);
        _diskMgr.WritePage(fileId, 0, write0);
        
        byte[] write2 = new byte[PAGE_SIZE];
        Array.Fill(write2, (byte)0xBB);
        _diskMgr.WritePage(fileId, 2, write2);
        
        byte[] read0 = new byte[PAGE_SIZE];
        _diskMgr.ReadPage(fileId, 0, read0);
        
        read0.Should().Equal(write0);
    }

    [Fact]
    public void WritePage_Page2_DoesNotCorruptPage1()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        
        byte[] buf0 = new byte[PAGE_SIZE];
        byte[] buf1 = new byte[PAGE_SIZE]; Array.Fill(buf1, (byte)0x11);
        byte[] buf2 = new byte[PAGE_SIZE];
        
        _diskMgr.WritePage(fileId, 0, buf0);
        _diskMgr.WritePage(fileId, 1, buf1);
        _diskMgr.WritePage(fileId, 2, buf2);
        
        byte[] read1 = new byte[PAGE_SIZE];
        _diskMgr.ReadPage(fileId, 1, read1);
        
        read1.Should().Equal(buf1);
    }

    [Fact]
    public void OverwritePage_SamePage_LastWriteWins()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        
        byte[] buf1 = new byte[PAGE_SIZE]; Array.Fill(buf1, (byte)0xAA);
        byte[] buf2 = new byte[PAGE_SIZE]; Array.Fill(buf2, (byte)0xBB);
        
        _diskMgr.WritePage(fileId, 0, buf1);
        _diskMgr.WritePage(fileId, 0, buf2);
        
        byte[] read = new byte[PAGE_SIZE];
        _diskMgr.ReadPage(fileId, 0, read);
        
        read.Should().Equal(buf2);
    }

    [Fact]
    public void MultiplePages_AllDataConsistent()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        var buffers = Enumerable.Range(0, 10).Select(i => 
        {
            var b = new byte[PAGE_SIZE];
            Array.Fill(b, (byte)i);
            return b;
        }).ToList();
        
        for (int i = 0; i < 10; i++)
        {
            _diskMgr.WritePage(fileId, i, buffers[i]);
        }
        
        for (int i = 0; i < 10; i++)
        {
            var read = new byte[PAGE_SIZE];
            _diskMgr.ReadPage(fileId, i, read);
            read.Should().Equal(buffers[i]);
        }
    }

    [Fact]
    public void WritePage_NullData_ThrowsArgumentNullException()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        Action act = () => _diskMgr.WritePage(fileId, 0, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WritePage_WrongBufferSize_ThrowsArgumentException()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        Action act = () => _diskMgr.WritePage(fileId, 0, new byte[100]);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ReadPage_NullBuffer_ThrowsArgumentNullException()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        Action act = () => _diskMgr.ReadPage(fileId, 0, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReadPage_WrongBufferSize_ThrowsArgumentException()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        Action act = () => _diskMgr.ReadPage(fileId, 0, new byte[100]);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ReadPage_NegativePageId_ThrowsArgumentOutOfRangeException()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        Action act = () => _diskMgr.ReadPage(fileId, -1, new byte[PAGE_SIZE]);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void WritePage_NegativePageId_ThrowsArgumentOutOfRangeException()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        Action act = () => _diskMgr.WritePage(fileId, -1, new byte[PAGE_SIZE]);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void WritePage_InvalidFileId_ThrowsArgumentException()
    {
        byte[] data = new byte[PAGE_SIZE];
        Action act = () => _diskMgr.WritePage(999, 0, data);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ReadPage_InvalidFileId_ThrowsArgumentException()
    {
        byte[] data = new byte[PAGE_SIZE];
        Action act = () => _diskMgr.ReadPage(999, 0, data);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WritePage_PageZero_WritesAtStartOfFile()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        byte[] writeBuf = new byte[PAGE_SIZE];
        Array.Fill(writeBuf, (byte)0x11);
        
        _diskMgr.WritePage(fileId, 0, writeBuf);
        
        byte[] rawBytes = File.ReadAllBytes(_tempPath).Take(PAGE_SIZE).ToArray();
        rawBytes.Should().Equal(writeBuf);
    }

    [Fact]
    public void WritePage_FullPageSize_NoDataLoss()
    {
        int fileId = _diskMgr.CreateFile(_tempPath);
        byte[] writeBuf = new byte[PAGE_SIZE];
        for (int i = 0; i < PAGE_SIZE; i++) writeBuf[i] = (byte)(i % 256);
        
        _diskMgr.WritePage(fileId, 0, writeBuf);
        
        byte[] readBuf = new byte[PAGE_SIZE];
        _diskMgr.ReadPage(fileId, 0, readBuf);
        
        readBuf.Should().Equal(writeBuf);
    }
}
