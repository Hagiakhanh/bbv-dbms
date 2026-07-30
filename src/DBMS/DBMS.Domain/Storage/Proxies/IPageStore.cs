namespace DBMS.Domain.Storage.Proxies;

public interface IPageStore
{
    Page FetchPage(int pageId);
    void FlushPage(int pageId);
    int AllocatePage(int tableId);
}
