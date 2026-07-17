using System;
using System.Collections.Generic;

namespace DBMS.Domain.Security;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}
