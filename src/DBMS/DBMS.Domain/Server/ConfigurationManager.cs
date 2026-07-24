using System;
using System.Collections.Generic;

namespace DBMS.Domain.Server;

public interface IConfigurationManager
{
    void Configure(string key, string value);
    string Get(string key);
    void LoadConfiguration(string filePath);
}

public class ConfigurationManager : IConfigurationManager
{
    public void Configure(string key, string value)
    {
        throw new NotImplementedException();
    }

    public string Get(string key)
    {
        throw new NotImplementedException();
    }

    public void LoadConfiguration(string filePath)
    {
        throw new NotImplementedException();
    }
}
