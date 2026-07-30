namespace DBMS.Domain.Storage.Policies;

public interface IReplacementPolicy
{
    int SelectVictim();
    void OnAccess(int pageId);
    void SetEvictable(int pageId, bool evictable);
}
