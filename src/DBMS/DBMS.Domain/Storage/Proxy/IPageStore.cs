namespace DBMS.Domain.Storage.Proxy;

public interface IPageStore
{
    Page FetchPage(int pageId);
    void FlushPage(int pageId);
    int AllocatePage(int tableId);
}
