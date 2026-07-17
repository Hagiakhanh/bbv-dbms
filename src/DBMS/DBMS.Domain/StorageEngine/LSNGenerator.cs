namespace DBMS.Domain.StorageEngine;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class LSNGenerator
{
    private LSN currentLSN;

    public LSNGenerator(LSN startLSN)
    {
        this.currentLSN = startLSN;
    }
    public LSN GetNextLSN()
    {
        throw new System.NotImplementedException();
    }
    public LSN GetCurrentLSN()
    {
        throw new System.NotImplementedException();
    }
}
