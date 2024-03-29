# SqliteWasmHelper assembly

## SqliteWasmHelper namespace
| public type | description |
| --- | --- |
| class [BackupLink](./SqliteWasmHelper/BackupLink.md) |  |
| class [BrowserCache](./SqliteWasmHelper/BrowserCache.md) | Wrapper for JavaScript code to sychronize the database. |
| static class [Extensions](./SqliteWasmHelper/Extensions.md) | Extensions for ease of use. |
| interface [IBrowserCache](./SqliteWasmHelper/IBrowserCache.md) | Wrapper for JavaScript module functions that interact with the cache. |
| interface [ISqliteSwap](./SqliteWasmHelper/ISqliteSwap.md) | Encapsulates backup functionality for SQLite. |
| interface [ISqliteWasmDbContextFactory&lt;TContext&gt;](./SqliteWasmHelper/ISqliteWasmDbContextFactory-1.md) | Interface for custom factory. |
| class [SqliteSwap](./SqliteWasmHelper/SqliteSwap.md) | Performs the backup or restore. Override to inject your own functionality, such as sending the file to the server. |
| class [SqliteWasmDbContextFactory&lt;TContext&gt;](./SqliteWasmHelper/SqliteWasmDbContextFactory-1.md) | Defers sending back the context until the database is restored, and backs up on succcessful saves. |
| class [_Imports](./SqliteWasmHelper/_Imports.md) |  |
<!-- DO NOT EDIT: generated by xmldocmd for SqliteWasmHelper.dll -->


Version 1.1.0-beta+9209ba6c71 generated on 04/21/2022 20:49:13.
