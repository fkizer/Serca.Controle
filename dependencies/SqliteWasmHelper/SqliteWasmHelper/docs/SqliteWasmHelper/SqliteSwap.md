# SqliteSwap class

Performs the backup or restore. Override to inject your own functionality, such as sending the file to the server.
```csharp
public class SqliteSwap : ISqliteSwap
```
## Public Members
| name | description |
| --- | --- |
| [SqliteSwap](SqliteSwap/SqliteSwap.md)() | The default constructor. |
| [DoSwap](SqliteSwap/DoSwap.md)(…) | Performs the swap between live database and backup. |
## See Also
* interface [ISqliteSwap](./ISqliteSwap.md)
* namespace [SqliteWasmHelper](../SqliteWasmHelper.md)
* [SqliteSwap.cs](https://github.com/JeremyLikness/SqliteWasmHelper/blob/main/SqliteWasmHelper/SqliteSwap.cs)
<!-- DO NOT EDIT: generated by xmldocmd for SqliteWasmHelper.dll -->


Version 1.1.0-beta+9209ba6c71 generated on 04/21/2022 20:49:13.
