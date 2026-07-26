namespace DBMS.Domain.Storage;

public interface IPageStore
{
    Page FetchPage(int pageId);
    void FlushPage(int pageId);
}
