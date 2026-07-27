namespace DBMS.Domain.Storage.Strategy;

public interface IReplacementPolicy
{
    int SelectVictim();
    void OnAccess(int pageId);
    void SetEvictable(int pageId, bool evictable);
}
