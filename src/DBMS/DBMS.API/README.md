# API Important in DBMS

🔗 **Live Interactive Swagger Documentation**: [https://hagiakhanh.github.io/bbv-dbms/](https://hagiakhanh.github.io/bbv-dbms/)

## 1. API Overview

The DBMS REST API provides administrative, security, session, database object, query processing, transaction, recovery, monitoring, and audit operations for the database management system.

### Base URL

```text
/api/v1
```

### Main Modules

| Module                      | Description                                                                         |
| --------------------------- | ----------------------------------------------------------------------------------- |
| Server Administration       | Manages the DBMS server lifecycle and runtime configuration                         |
| Authentication and Security | Manages authentication, users, roles, and permissions                               |
| Session and Connection      | Manages client sessions and active database contexts                                |
| Backup and Recovery         | Creates backups, restores data, and monitors recovery operations                    |
| Database Object APIs        | Manages databases, schemas, tables, columns, constraints, and indexes               |
| Catalog and Metadata        | Provides metadata lookup, dependency analysis, and DDL generation                   |
| Query Processing            | Parses, validates, explains, executes, and cancels SQL queries                      |
| Transaction Management      | Manages transactions, isolation levels, commits, rollbacks, and savepoints          |
| Monitoring and Audit        | Provides health checks, metrics, query monitoring, lock information, and audit logs |

---

# 2. Server Administration APIs

| No. | API Name                    |  Method | Endpoint                | Description                                       | Request                        | Success Response                  |
| --: | --------------------------- | :-----: | ----------------------- | ------------------------------------------------- | ------------------------------ | --------------------------------- |
|   1 | Get Server Status           |  `GET`  | `/server/status`        | Retrieves the current status of the DBMS server   | None                           | `200 OK` – Server status          |
|   2 | Start Server                |  `POST` | `/server/start`         | Starts the DBMS server                            | Optional startup configuration | `200 OK` – Server started         |
|   3 | Stop Server                 |  `POST` | `/server/stop`          | Stops the DBMS server                             | Optional force flag            | `200 OK` – Server stopped         |
|   4 | Restart Server              |  `POST` | `/server/restart`       | Restarts the DBMS server                          | Optional restart configuration | `200 OK` – Server restarted       |
|   5 | Recover Server              |  `POST` | `/server/recover`       | Starts server recovery using logs or checkpoints  | Recovery options               | `202 Accepted` – Recovery started |
|   6 | Get Server Configuration    |  `GET`  | `/server/configuration` | Retrieves the active server configuration         | None                           | `200 OK` – Configuration          |
|   7 | Update Server Configuration | `PATCH` | `/server/configuration` | Updates one or more server configuration settings | Configuration values           | `200 OK` – Updated configuration  |

---

# 3. Authentication APIs

| No. | API Name         | Method | Endpoint        | Description                                                | Request                          | Success Response                    |
| --: | ---------------- | :----: | --------------- | ---------------------------------------------------------- | -------------------------------- | ----------------------------------- |
|   1 | Register         | `POST` | `/auth/register` | Registers a new user account                               | Username, password, and email    | `201 Created` – Newly registered user |
|   2 | Login            | `POST` | `/auth/login`   | Authenticates a user and returns access and refresh tokens | Username and password            | `200 OK` – Authentication tokens    |
|   3 | Refresh Token    | `POST` | `/auth/refresh` | Generates a new access token using a refresh token         | Refresh token                    | `200 OK` – New access token         |
|   4 | Logout           | `POST` | `/auth/logout`  | Invalidates the current refresh token or session           | Refresh token or session context | `204 No Content`                    |
|   5 | Get Current User |  `GET` | `/auth/me`      | Retrieves the currently authenticated user                 | Bearer token                     | `200 OK` – Current user information |

---

# 4. User Management APIs

| No. | API Name    |  Method  | Endpoint          | Description                                 | Request                                   | Success Response                   |
| --: | ----------- | :------: | ----------------- | ------------------------------------------- | ----------------------------------------- | ---------------------------------- |
|   1 | Create User |  `POST`  | `/users`          | Creates a new database user                 | Username, password, and user properties   | `201 Created` – Newly created user |
|   2 | List Users  |   `GET`  | `/users`          | Retrieves users in the system               | Optional pagination and search parameters | `200 OK` – User list               |
|   3 | Get User    |   `GET`  | `/users/{userId}` | Retrieves detailed information about a user | User ID                                   | `200 OK` – User details            |
|   4 | Update User |  `PATCH` | `/users/{userId}` | Updates user information or account status  | Properties to update                      | `200 OK` – Updated user            |
|   5 | Delete User | `DELETE` | `/users/{userId}` | Deletes or disables a user account          | User ID                                   | `204 No Content`                   |

---

# 5. Role and Permission APIs

| No. | API Name          |  Method  | Endpoint                                     | Description                         | Request                            | Success Response                    |
| --: | ----------------- | :------: | -------------------------------------------- | ----------------------------------- | ---------------------------------- | ----------------------------------- |
|   1 | Create Role       |  `POST`  | `/roles`                                     | Creates a new security role         | Role name and description          | `201 Created` – Newly created role  |
|   2 | List Roles        |   `GET`  | `/roles`                                     | Retrieves all available roles       | Optional pagination and search     | `200 OK` – Role list                |
|   3 | Assign Role       |  `POST`  | `/users/{userId}/roles`                      | Assigns one or more roles to a user | Role IDs                           | `200 OK` – Updated role assignments |
|   4 | Remove Role       | `DELETE` | `/users/{userId}/roles/{roleId}`             | Removes a role from a user          | User ID and role ID                | `204 No Content`                    |
|   5 | Grant Permission  |  `POST`  | `/roles/{roleId}/permissions`                | Grants permissions to a role        | Permission IDs or permission names | `200 OK` – Updated permissions      |
|   6 | Revoke Permission | `DELETE` | `/roles/{roleId}/permissions/{permissionId}` | Revokes a permission from a role    | Role ID and permission ID          | `204 No Content`                    |

---

# 6. Session and Connection APIs

| No. | API Name                |  Method  | Endpoint                         | Description                               | Request                               | Success Response                    |
| --: | ----------------------- | :------: | -------------------------------- | ----------------------------------------- | ------------------------------------- | ----------------------------------- |
|   1 | Open Session            |  `POST`  | `/sessions`                      | Opens a new database session              | Optional database and session options | `201 Created` – Session information |
|   2 | Get Session             |   `GET`  | `/sessions/{sessionId}`          | Retrieves a specific session              | Session ID                            | `200 OK` – Session details          |
|   3 | List Sessions           |   `GET`  | `/sessions`                      | Retrieves active and recent sessions      | Optional status and user filters      | `200 OK` – Session list             |
|   4 | Change Session Database |  `PATCH` | `/sessions/{sessionId}/database` | Changes the active database for a session | Database name or ID                   | `200 OK` – Updated session          |
|   5 | Close Session           | `DELETE` | `/sessions/{sessionId}`          | Closes an active session                  | Session ID                            | `204 No Content`                    |

---

# 7. Backup and Recovery APIs

| No. | API Name            | Method | Endpoint                      | Description                                         | Request                                     | Success Response                    |
| --: | ------------------- | :----: | ----------------------------- | --------------------------------------------------- | ------------------------------------------- | ----------------------------------- |
|   1 | Create Backup       | `POST` | `/backups`                    | Creates a backup of a database or the entire server | Backup type and target database             | `202 Accepted` – Backup job started |
|   2 | List Backups        |  `GET` | `/backups`                    | Retrieves available backup records                  | Optional database, type, and status filters | `200 OK` – Backup list              |
|   3 | Get Backup          |  `GET` | `/backups/{backupId}`         | Retrieves details of a backup                       | Backup ID                                   | `200 OK` – Backup details           |
|   4 | Restore Backup      | `POST` | `/backups/{backupId}/restore` | Restores data from a backup                         | Restore options and target database         | `202 Accepted` – Restore started    |
|   5 | Get Recovery Status |  `GET` | `/recovery/status`            | Retrieves the current server recovery status        | None                                        | `200 OK` – Recovery information     |

---

# 8. Database APIs

| No. | API Name           |  Method  | Endpoint                 | Description                                         | Request                                            | Success Response                       |
| --: | ------------------ | :------: | ------------------------ | --------------------------------------------------- | -------------------------------------------------- | -------------------------------------- |
|   1 | Create Database    |  `POST`  | `/databases`             | Creates a new database                              | Database name, owner, character set, and collation | `201 Created` – Newly created database |
|   2 | List Databases     |   `GET`  | `/databases`             | Retrieves databases available to the user           | Optional pagination and search parameters          | `200 OK` – Database list               |
|   3 | Get Database       |   `GET`  | `/databases/{name}`      | Retrieves database details                          | Database name                                      | `200 OK` – Database details            |
|   4 | Update Database    |  `PATCH` | `/databases/{name}`      | Updates database properties or renames the database | Properties to update                               | `200 OK` – Updated database            |
|   5 | Drop Database      | `DELETE` | `/databases/{name}`      | Deletes a database                                  | Database name                                      | `204 No Content`                       |
|   6 | Set Database State |  `PATCH` | `/databases/{name}/state` | Sets state of a database (ONLINE/OFFLINE/READ_ONLY) | State value                                        | `200 OK` – Updated database state      |
|   7 | Attach Database    |  `POST`  | `/databases/attach`      | Attaches an existing database file                  | Database name and file path                        | `201 Created` – Attached database      |
|   8 | Detach Database    |  `POST`  | `/databases/{name}/detach`| Detaches a database from the server                 | Database name                                      | `204 No Content`                       |

---

# 9. Schema APIs

| No. | API Name      |  Method  | Endpoint                  | Description                               | Request               | Success Response                     |
| --: | ------------- | :------: | ------------------------- | ----------------------------------------- | --------------------- | ------------------------------------ |
|   1 | Create Schema |  `POST`  | `/databases/{db}/schemas` | Creates a schema inside a database        | Schema name and owner | `201 Created` – Newly created schema |
|   2 | List Schemas  |   `GET`  | `/databases/{db}/schemas` | Retrieves schemas belonging to a database | Database name or ID   | `200 OK` – Schema list               |
|   3 | Get Schema    |   `GET`  | `/schemas/{name}`         | Retrieves schema details                  | Schema name           | `200 OK` – Schema details            |
|   4 | Update Schema |  `PATCH` | `/schemas/{name}`         | Updates schema properties or renames it   | Properties to update  | `200 OK` – Updated schema            |
|   5 | Drop Schema   | `DELETE` | `/schemas/{name}`         | Deletes a schema                          | Schema name           | `204 No Content`                     |

---

# 10. Table APIs

| No. | API Name     |  Method  | Endpoint                   | Description                                 | Request              | Success Response                    |
| --: | ------------ | :------: | -------------------------- | ------------------------------------------- | -------------------- | ----------------------------------- |
|   1 | Create Table |  `POST`  | `/schemas/{schema}/tables` | Creates a table inside a schema             | Table definition     | `201 Created` – Newly created table |
|   2 | List Tables  |   `GET`  | `/schemas/{schema}/tables` | Retrieves tables belonging to a schema      | Schema name or ID    | `200 OK` – Table list               |
|   3 | Get Table    |   `GET`  | `/tables/{name}`           | Retrieves table structure and metadata      | Table name           | `200 OK` – Table details            |
|   4 | Update Table |  `PATCH` | `/tables/{name}`           | Renames a table or updates table properties | Properties to update | `200 OK` – Updated table            |
|   5 | Drop Table   | `DELETE` | `/tables/{name}`           | Deletes a table                             | Table name           | `204 No Content`                    |

---

# 11. Column APIs

| No. | API Name      |  Method  | Endpoint                                     | Description                                                     | Request              | Success Response                     |
| --: | ------------- | :------: | -------------------------------------------- | --------------------------------------------------------------- | -------------------- | ------------------------------------ |
|   1 | Add Column    |  `POST`  | `/tables/{tableName}/columns`                | Adds a column to a table                                        | Column definition    | `201 Created` – Newly created column |
|   2 | List Columns  |   `GET`  | `/tables/{tableName}/columns`                | Retrieves columns belonging to a table                          | Table name           | `200 OK` – Column list               |
|   3 | Update Column |  `PATCH` | `/tables/{tableName}/columns/{name}`         | Changes a column name, data type, default value, or nullability | Properties to update | `200 OK` – Updated column            |
|   4 | Drop Column   | `DELETE` | `/tables/{tableName}/columns/{name}`         | Removes a column from a table                                   | Column name          | `204 No Content`                     |

---

# 12. Constraint APIs

| No. | API Name         |  Method  | Endpoint                                     | Description                                | Request               | Success Response                         |
| --: | ---------------- | :------: | -------------------------------------------- | ------------------------------------------ | --------------------- | ---------------------------------------- |
|   1 | Add Constraint   |  `POST`  | `/tables/{tableName}/constraints`            | Adds a constraint to a table               | Constraint definition | `201 Created` – Newly created constraint |
|   2 | List Constraints |   `GET`  | `/tables/{tableName}/constraints`            | Retrieves constraints belonging to a table | Table name            | `200 OK` – Constraint list               |
|   3 | Get Constraint   |   `GET`  | `/tables/{tableName}/constraints/{name}`     | Retrieves constraint details               | Constraint name       | `200 OK` – Constraint details            |
|   4 | Drop Constraint  | `DELETE` | `/tables/{tableName}/constraints/{name}`     | Removes a constraint from a table          | Constraint name       | `204 No Content`                         |

---

# 13. Index APIs

| No. | API Name      |  Method  | Endpoint                                     | Description                                    | Request                  | Success Response                    |
| --: | ------------- | :------: | -------------------------------------------- | ---------------------------------------------- | ------------------------ | ----------------------------------- |
|   1 | Create Index  |  `POST`  | `/tables/{tableName}/indexes`                | Creates an index for one or more table columns | Index definition         | `201 Created` – Newly created index |
|   2 | List Indexes  |   `GET`  | `/tables/{tableName}/indexes`                | Retrieves indexes belonging to a table         | Table name               | `200 OK` – Index list               |
|   3 | Get Index     |   `GET`  | `/tables/{tableName}/indexes/{name}`         | Retrieves index details                        | Index name               | `200 OK` – Index details            |
|   4 | Rebuild Index |  `POST`  | `/tables/{tableName}/indexes/{name}/rebuild` | Rebuilds an existing index                     | Optional rebuild options | `202 Accepted` – Rebuild started    |
|   5 | Enable Index  |  `POST`  | `/tables/{tableName}/indexes/{name}/enable`  | Enables an existing index                      | Table name and index name| `200 OK` – Index enabled            |
|   6 | Disable Index |  `POST`  | `/tables/{tableName}/indexes/{name}/disable` | Disables an existing index                     | Table name and index name| `200 OK` – Index disabled           |
|   7 | Drop Index    | `DELETE` | `/tables/{tableName}/indexes/{name}`         | Removes an index                               | Index name               | `204 No Content`                    |

---

# 14. Phase 2 Database Objects

The following database object types are planned for Phase 2.

| Object Type      | Planned Functionality                               |
| ---------------- | --------------------------------------------------- |
| View             | Create, retrieve, update, and drop database views   |
| Sequence         | Create and manage numeric sequences                 |
| Stored Procedure | Create, execute, update, and drop stored procedures |

## Proposed View Endpoints

|  Method  | Endpoint                  | Description   |
| :------: | ------------------------- | ------------- |
|  `POST`  | `/schemas/{schema}/views` | Create a view |
|   `GET`  | `/schemas/{schema}/views` | List views    |
|   `GET`  | `/views/{view}`           | Get a view    |
|  `PATCH` | `/views/{view}`           | Update a view |
| `DELETE` | `/views/{view}`           | Drop a view   |

## Proposed Sequence Endpoints

|  Method  | Endpoint                           | Description             |
| :------: | ---------------------------------- | ----------------------- |
|  `POST`  | `/schemas/{schema}/sequences`      | Create a sequence       |
|   `GET`  | `/schemas/{schema}/sequences`      | List sequences          |
|   `GET`  | `/sequences/{sequence}`            | Get a sequence          |
|  `POST`  | `/sequences/{sequence}/next-value` | Retrieve the next value |
| `DELETE` | `/sequences/{sequence}`            | Drop a sequence         |

## Proposed Stored Procedure Endpoints

|  Method  | Endpoint                          | Description                |
| :------: | --------------------------------- | -------------------------- |
|  `POST`  | `/schemas/{schema}/procedures`    | Create a stored procedure  |
|   `GET`  | `/schemas/{schema}/procedures`    | List stored procedures     |
|   `GET`  | `/procedures/{procedure}`         | Get procedure metadata     |
|  `POST`  | `/procedures/{procedure}/execute` | Execute a stored procedure |
| `DELETE` | `/procedures/{procedure}`         | Drop a stored procedure    |

---

# 15. Catalog and Metadata APIs

| No. | API Name                | Method | Endpoint                                   | Description                                                     | Request                             | Success Response                 |
| --: | ----------------------- | :----: | ------------------------------------------ | --------------------------------------------------------------- | ----------------------------------- | -------------------------------- |
|   1 | Get Catalog Tree        |  `GET` | `/catalog/tree`                            | Retrieves the hierarchical catalog structure                    | Optional database and depth filters | `200 OK` – Catalog tree          |
|   2 | Get Database Metadata   |  `GET` | `/catalog/databases/{db}`                  | Retrieves metadata for a database                               | Database name or ID                 | `200 OK` – Database metadata     |
|   3 | Get Schema Metadata     |  `GET` | `/catalog/schemas/{schema}`                | Retrieves metadata for a schema                                 | Schema name or ID                   | `200 OK` – Schema metadata       |
|   4 | Get Table Metadata      |  `GET` | `/catalog/tables/{table}`                  | Retrieves complete table metadata                               | Table name or ID                    | `200 OK` – Table metadata        |
|   5 | Search Metadata         |  `GET` | `/catalog/search?keyword={keyword}`        | Searches catalog objects by name or type                        | Keyword and optional filters        | `200 OK` – Search results        |
|   6 | Get Object Dependencies |  `GET` | `/catalog/objects/{objectId}/dependencies` | Retrieves objects that depend on or are referenced by an object | Object ID                           | `200 OK` – Dependency graph      |
|   7 | Generate DDL            |  `GET` | `/catalog/objects/{objectId}/ddl`          | Generates a DDL script for an object                            | Object ID                           | `200 OK` – DDL script            |
|   8 | Refresh Catalog         | `POST` | `/catalog/refresh`                         | Reloads or rebuilds catalog metadata                            | Optional database or object scope   | `202 Accepted` – Refresh started |

---

# 16. Query Processing APIs

| No. | API Name         | Method | Endpoint                    | Description                                                          | Request                                      | Success Response                        |
| --: | ---------------- | :----: | --------------------------- | -------------------------------------------------------------------- | -------------------------------------------- | --------------------------------------- |
|   1 | Parse Query      | `POST` | `/queries/parse`            | Parses SQL and returns an Abstract Syntax Tree                       | SQL string and session context               | `200 OK` – Parsed query                 |
|   2 | Validate Query   | `POST` | `/queries/validate`         | Validates SQL syntax, semantics, permissions, and referenced objects | SQL string and session context               | `200 OK` – Validation result            |
|   3 | Explain Query    | `POST` | `/queries/explain`          | Generates a logical and physical execution plan                      | SQL string and explain options               | `200 OK` – Execution plan               |
|   4 | Execute Query    | `POST` | `/queries/execute`          | Executes DDL, DML, or SELECT statements                              | SQL, session ID, and optional transaction ID | `200 OK` or `202 Accepted`              |
|   5 | Cancel Query     | `POST` | `/queries/{queryId}/cancel` | Requests cancellation of an active query                             | Query ID                                     | `202 Accepted` – Cancellation requested |
|   6 | Get Query Status |  `GET` | `/queries/{queryId}/status` | Retrieves the current status of a query                              | Query ID                                     | `200 OK` – Query status                 |
|   7 | Get Query Result |  `GET` | `/queries/{queryId}/result` | Retrieves the result of a completed query                            | Query ID and optional pagination             | `200 OK` – Query result                 |

---

# 17. Transaction APIs

| No. | API Name              |  Method  | Endpoint                                                        | Description                                | Request                                    | Success Response                        |
| --: | --------------------- | :------: | --------------------------------------------------------------- | ------------------------------------------ | ------------------------------------------ | --------------------------------------- |
|   1 | Begin Transaction     |  `POST`  | `/transactions`                                                 | Starts a new transaction                   | Session ID, database, and isolation level  | `201 Created` – Transaction information |
|   2 | Get Transaction       |   `GET`  | `/transactions/{transactionId}`                                 | Retrieves transaction details              | Transaction ID                             | `200 OK` – Transaction details          |
|   3 | List Transactions     |   `GET`  | `/transactions`                                                 | Retrieves active or completed transactions | Optional session, user, and status filters | `200 OK` – Transaction list             |
|   4 | Commit Transaction    |  `POST`  | `/transactions/{transactionId}/commit`                          | Commits an active transaction              | Transaction ID                             | `200 OK` – Transaction committed        |
|   5 | Rollback Transaction  |  `POST`  | `/transactions/{transactionId}/rollback`                        | Rolls back an active transaction           | Transaction ID                             | `200 OK` – Transaction rolled back      |
|   6 | Create Savepoint      |  `POST`  | `/transactions/{transactionId}/savepoints`                      | Creates a named savepoint                  | Savepoint name                             | `201 Created` – Savepoint created       |
|   7 | Rollback to Savepoint |  `POST`  | `/transactions/{transactionId}/savepoints/{savepoint}/rollback` | Rolls back changes after a savepoint       | Transaction ID and savepoint name          | `200 OK` – Rolled back to savepoint     |
|   8 | Release Savepoint     | `DELETE` | `/transactions/{transactionId}/savepoints/{savepoint}`          | Removes a savepoint                        | Transaction ID and savepoint name          | `204 No Content`                        |

---

# 18. Monitoring APIs

| No. | API Name           | Method | Endpoint                   | Description                                           | Request                                      | Success Response                      |
| --: | ------------------ | :----: | -------------------------- | ----------------------------------------------------- | -------------------------------------------- | ------------------------------------- |
|   1 | Health Check       |  `GET` | `/health`                  | Retrieves the overall health status of the DBMS API   | None                                         | `200 OK` or `503 Service Unavailable` |
|   2 | Readiness Check    |  `GET` | `/health/ready`            | Indicates whether the DBMS is ready to accept traffic | None                                         | `200 OK` or `503 Service Unavailable` |
|   3 | Liveness Check     |  `GET` | `/health/live`             | Indicates whether the DBMS process is alive           | None                                         | `200 OK` or `503 Service Unavailable` |
|   4 | Get Metrics        |  `GET` | `/monitoring/metrics`      | Retrieves system and database performance metrics     | Optional metric filters                      | `200 OK` – Metrics                    |
|   5 | Get Active Queries |  `GET` | `/monitoring/queries`      | Retrieves currently executing queries                 | Optional session, user, and duration filters | `200 OK` – Active query list          |
|   6 | Get Slow Queries   |  `GET` | `/monitoring/slow-queries` | Retrieves queries exceeding the slow-query threshold  | Time range and duration threshold            | `200 OK` – Slow query list            |
|   7 | Get Active Locks   |  `GET` | `/monitoring/locks`        | Retrieves active locks and waiting transactions       | Optional transaction and object filters      | `200 OK` – Lock list                  |
|   8 | Get Deadlocks      |  `GET` | `/monitoring/deadlocks`    | Retrieves detected deadlock records                   | Optional time range                          | `200 OK` – Deadlock list              |

---

# 19. Audit Log APIs

| No. | API Name       | Method | Endpoint      | Description                                                              | Request                                         | Success Response          |
| --: | -------------- | :----: | ------------- | ------------------------------------------------------------------------ | ----------------------------------------------- | ------------------------- |
|   1 | Get Audit Logs |  `GET` | `/audit-logs` | Retrieves security, administration, and database operation audit records | Optional user, action, object, and time filters | `200 OK` – Audit log list |