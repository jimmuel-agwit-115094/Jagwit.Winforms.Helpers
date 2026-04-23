# Jagwit.Winforms.Helpers

A class library of reusable WinForms helper utilities — EF Core transaction helpers, UI message dialogs, date formatting, null checking, concurrency checking, server clock, and RDLC printing.

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

Windows Forms application required for both targets.

---

## Utilities

### DatabaseTransactionHelper

Extension method on `DbContext` that wraps your async operations in a `TransactionScope`. Automatically handles common exceptions and displays user-friendly dialogs.

**Usage**

```csharp
using Jagwit.Winforms.Helpers.DbContexts;

await _context.ExecuteInTransactionAsync(async () =>
{
    // your EF Core operations here
    _context.Orders.Add(newOrder);
    await _context.SaveChangesAsync();
});
```

**Exception handling built-in**

| Exception | Behavior |
|---|---|
| `NullReferenceException` | Shows error dialog — "Null data error" |
| `DBConcurrencyException` | Shows error dialog — "Dirty data error" |
| `InvalidOperationException` (known) | Shows warning dialog with the message |
| `Exception` (any other) | Shows error dialog with full details |

Known `InvalidOperationException` prefixes treated as warnings:
- `INSUFFICIENT STOCK`
- `PRODUCT NOT FOUND`
- `INVALID OPERATION`

---

### MessageHandler

Static wrapper around `MessageBox.Show` for consistent dialog styling across your WinForms app.

**Usage**

```csharp
using Jagwit.Winforms.Helpers.Utilities;

// Error dialog
MessageHandler.ShowError("Something went wrong.");

// Warning dialog
MessageHandler.ShowWarning("Stock is running low.");

// Info dialog
MessageHandler.ShowInfo("Record saved successfully.");

// Yes/No confirmation
DialogResult result = MessageHandler.ShowConfirmation("Delete this record?");
if (result == DialogResult.Yes)
{
    // proceed
}
```

---

### ClockHelper

Retrieves the current date and time from the database server via a raw ADO.NET connection. Supports an injectable `DateTimeProvider` delegate for unit testing.

> Available on both `net8.0-windows` and `net472`.

**Usage**

```csharp
using Jagwit.Winforms.Helpers.Utilities;
using System.Data.Common;

// With EF Core (requires Microsoft.EntityFrameworkCore.Relational):
var connection = (DbConnection)_context.Database.GetDbConnection();
DateTime serverTime = await ClockHelper.GetServerDateTimeAsync(connection, "SELECT NOW()");

// With a direct ADO.NET connection:
using var conn = new MySqlConnection(connectionString);
DateTime serverTime = await ClockHelper.GetServerDateTimeAsync(conn, "SELECT NOW()");

// Override for testing:
ClockHelper.DateTimeProvider = () => Task.FromResult(new DateTime(2026, 1, 1));
```

The `sql` parameter defaults to `SELECT GETDATE()` (SQL Server). Pass `SELECT NOW()` for MySQL or PostgreSQL.

---

### DateFormatHelper

Static methods for formatting `DateTime` values into display-ready strings.

> Available on both `net8.0-windows` and `net472`.

**Usage**

```csharp
using Jagwit.Winforms.Helpers.Utilities;

// "April 23, 2026"
string full = DateFormatHelper.FormatDate(DateTime.Now);

// "April 23, 2026 | 02:30 PM"
string withTime = DateFormatHelper.FormatDateWithTime(DateTime.Now);

// "Apr. 23, 2026"
string shortDate = DateFormatHelper.FormatShortDate(DateTime.Now);

// Custom format
string custom = DateFormatHelper.FormatDate(DateTime.Now, "yyyy-MM-dd");
```

---

### NullCheckerHelper

Throws a `NullReferenceException` with a descriptive message when a database entity is null.

> Available on both `net8.0-windows` and `net472`.

**Usage**

```csharp
using Jagwit.Winforms.Helpers.Utilities;

var product = await _context.Products.FindAsync(id);
NullCheckerHelper.NullCheck(product);
// continues safely — product is not null here
```

---

### PrintingHelper

Configures a `ReportViewer` control with an RDLC report, data source, and optional parameters, then switches to print-layout mode and refreshes.

> **Available on `net472` only.** No official ReportViewer NuGet package exists for `net8.0-windows`.  
> Requires `Microsoft.ReportingServices.ReportViewerControl.Winforms` installed in your project.

RDLC files are resolved from `<Application.StartupPath>\Printing\rdlc\<reportName>.rdlc`.

**Usage**

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

Detects optimistic concurrency violations by comparing a row-version token recorded at load time against its current database value.

> Available on both `net8.0-windows` and `net472`.

**Usage**

```csharp
using Jagwit.Winforms.Helpers.Utilities;

// Throws DBConcurrencyException if the record was modified since it was loaded
VersionCheckerHelper.ConcurrencyCheck(originalVersion, latestVersion);
```

Pair this with a `RowVersion` / `Timestamp` column in your EF Core model to detect dirty-read conflicts before saving.

---

## License

[MIT](LICENSE) © Jimmuel Agwit
