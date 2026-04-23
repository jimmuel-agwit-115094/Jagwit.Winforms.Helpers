# Jagwit.Winforms.Helpers

Reusable WinForms utilities for .NET 8 and .NET Framework 4.7.2 — EF Core transactions, message dialogs, date formatting, null checking, concurrency checking, server clock, and RDLC printing.

---

## Installation

```bash
dotnet add package Jagwit.Winforms.Helpers
```

Or via NuGet Package Manager in Visual Studio:

```
Install-Package Jagwit.Winforms.Helpers
```

---

## Requirements

| Target | Runtime |
|---|---|
| `net8.0-windows` | .NET 8.0 (Windows) |
| `net472` | .NET Framework 4.7.2 or later |

A Windows Forms application is required for both targets.

---

## Breaking Change — v1.1.3

The `Helpers` folder and namespace were renamed to `Utilities`.

```csharp
// Before
using Jagwit.Winforms.Helpers.Helpers;

// After
using Jagwit.Winforms.Helpers.Utilities;
```

---

## Utilities

### DatabaseTransactionHelper

Extension method on `DbContext` that wraps async operations in a `TransactionScope`. Handles common exceptions and surfaces them as user-friendly dialogs.

**Namespace:** `Jagwit.Winforms.Helpers.DbContexts`  
**Targets:** `net8.0-windows`, `net472`

```csharp
using Jagwit.Winforms.Helpers.DbContexts;

await _context.ExecuteInTransactionAsync(async () =>
{
    _context.Orders.Add(newOrder);
    await _context.SaveChangesAsync();
});
```

**Built-in exception handling**

| Exception | Behavior |
|---|---|
| `NullReferenceException` | Error dialog — "Null data error" |
| `DBConcurrencyException` | Error dialog — "Dirty data error" |
| `InvalidOperationException` (known prefix) | Warning dialog with the message |
| Any other `Exception` | Error dialog with full details |

Known `InvalidOperationException` prefixes treated as warnings:
- `INSUFFICIENT STOCK`
- `PRODUCT NOT FOUND`
- `INVALID OPERATION`

---

### MessageHandler

Static wrapper around `MessageBox.Show` for consistent dialog styling.

**Namespace:** `Jagwit.Winforms.Helpers.Utilities`  
**Targets:** `net8.0-windows`, `net472`

```csharp
using Jagwit.Winforms.Helpers.Utilities;

MessageHandler.ShowError("Something went wrong.");
MessageHandler.ShowWarning("Stock is running low.");
MessageHandler.ShowInfo("Record saved successfully.");

DialogResult result = MessageHandler.ShowConfirmation("Delete this record?");
if (result == DialogResult.Yes) { }
```

---

### ClockHelper

Retrieves the current date and time from the database server via a raw ADO.NET connection. Supports an injectable `DateTimeProvider` for unit testing.

**Namespace:** `Jagwit.Winforms.Helpers.Utilities`  
**Targets:** `net8.0-windows`, `net472`

```csharp
using Jagwit.Winforms.Helpers.Utilities;
using System.Data.Common;

// With a direct ADO.NET connection:
using var conn = new MySqlConnection(connectionString);
DateTime serverTime = await ClockHelper.GetServerDateTimeAsync(conn, "SELECT NOW()");

// With EF Core (requires Microsoft.EntityFrameworkCore.Relational):
var conn = (DbConnection)_context.Database.GetDbConnection();
DateTime serverTime = await ClockHelper.GetServerDateTimeAsync(conn);

// Override for testing:
ClockHelper.DateTimeProvider = () => Task.FromResult(new DateTime(2026, 1, 1));
```

The `sql` parameter defaults to `SELECT GETDATE()` (SQL Server). Use `SELECT NOW()` for MySQL or PostgreSQL.

---

### DateFormatHelper

Static methods for formatting `DateTime` values into display-ready strings.

**Namespace:** `Jagwit.Winforms.Helpers.Utilities`  
**Targets:** `net8.0-windows`, `net472`

```csharp
using Jagwit.Winforms.Helpers.Utilities;

DateFormatHelper.FormatDate(DateTime.Now);           // "April 23, 2026"
DateFormatHelper.FormatDateWithTime(DateTime.Now);   // "April 23, 2026 | 02:30 PM"
DateFormatHelper.FormatShortDate(DateTime.Now);      // "Apr. 23, 2026"

// Custom format
DateFormatHelper.FormatDate(DateTime.Now, "yyyy-MM-dd");
```

---

### NullCheckerHelper

Throws a `NullReferenceException` with a descriptive message when a database entity is `null`.

**Namespace:** `Jagwit.Winforms.Helpers.Utilities`  
**Targets:** `net8.0-windows`, `net472`

```csharp
using Jagwit.Winforms.Helpers.Utilities;

var product = await _context.Products.FindAsync(id);
NullCheckerHelper.NullCheck(product);
// safe to use product below
```

---

### PrintingHelper

Configures a `ReportViewer` control with an RDLC file, data source, and optional parameters, then switches to print-layout mode and refreshes.

**Namespace:** `Jagwit.Winforms.Helpers.Utilities`  
**Targets:** `net472` only — no official ReportViewer NuGet package exists for `net8.0-windows`

RDLC files are resolved from `<Application.StartupPath>\Printing\rdlc\<reportName>.rdlc`.

```csharp
using Jagwit.Winforms.Helpers.Utilities;

PrintingHelper.PrintReport(
    reportViewer: reportViewer1,
    reportName: "ReceiptRdlc",
    dataSource: receiptDataTable,
    dataSetName: "ReceiptDataSet",
    parameters: new Dictionary<string, string>
    {
        { "CashierName", "John Doe" },
        { "TransactionDate", DateTime.Now.ToString("MMMM dd, yyyy") }
    }
);
```

---

### VersionCheckerHelper

Detects optimistic concurrency violations by comparing a row-version token captured at load time against its current database value.

**Namespace:** `Jagwit.Winforms.Helpers.Utilities`  
**Targets:** `net8.0-windows`, `net472`

```csharp
using Jagwit.Winforms.Helpers.Utilities;

// Throws DBConcurrencyException if the record was modified since it was loaded
VersionCheckerHelper.ConcurrencyCheck(originalVersion, latestVersion);
```

Pair with a `RowVersion` / `Timestamp` column in your EF Core model to detect dirty-read conflicts before saving.

---

## License

[MIT](LICENSE) © Jimmuel Agwit
