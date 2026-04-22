# Jagwit.Winforms.Helpers

A .NET 8 class library of reusable WinForms helper utilities — EF Core transaction helpers, UI message dialogs, and more.

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

- .NET 8.0 (Windows)
- Windows Forms application

---

## Helpers

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
using Jagwit.Winforms.Helpers.Helpers;

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

## License

[MIT](LICENSE) © Jimmuel Agwit
