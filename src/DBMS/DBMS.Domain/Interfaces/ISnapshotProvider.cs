namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface ISnapshotProvider
{
    long CreateSnapshot(TransactionId txId);
    void ReleaseSnapshot(long id);
    bool IsVisible(TransactionId txId, long snapshotId);
}
