namespace DBMS.Domain.Transactions;

public interface ITransactionManager
{
    void Initialize();
    void Shutdown();
}
