# Database Migration Guide: SQLite to SQL Server

## Prerequisites

1. **SQL Server installed** (choose one):
   - **SQL Server Express** (free): https://www.microsoft.com/sql-server/sql-server-downloads
   - **SQL Server LocalDB** (comes with Visual Studio): Usually already installed
   - **SQL Server Developer Edition** (free): https://www.microsoft.com/sql-server/sql-server-downloads

2. **Verify SQL Server is running**:
   - Open SQL Server Configuration Manager
   - Or check if LocalDB is available: `sqllocaldb info`

## Step-by-Step Migration Process

### Step 1: Install EF Core Tools (if not already installed)

Open PowerShell or Command Prompt and run:

```bash
dotnet tool install --global dotnet-ef
```

If already installed, update it:
```bash
dotnet tool update --global dotnet-ef
```

### Step 2: Restore NuGet Packages

```bash
dotnet restore
```

### Step 3: Update Connection String (Already Done ✅)

The connection strings have been updated to use SQL Server LocalDB:
- **Development**: `Server=(localdb)\mssqllocaldb;Database=WarehouseManagement_Dev;Trusted_Connection=true;TrustServerCertificate=true;`
- **Production**: `Server=(localdb)\mssqllocaldb;Database=WarehouseManagement;Trusted_Connection=true;TrustServerCertificate=true;`

**For Full SQL Server**, update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WarehouseManagement;User Id=your_username;Password=your_password;TrustServerCertificate=true;"
  }
}
```

**For SQL Server Express**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=WarehouseManagement;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Step 4: Create Initial Migration

Navigate to the ASP.NET project and create the migration:

```bash
cd Wms.ASP
dotnet ef migrations add InitialSqlServerMigration --project ../Wms.Infrastructure --startup-project .
```

This creates a `Migrations` folder in `Wms.Infrastructure` with the migration files.

### Step 5: Apply Migration to Create Database

Apply the migration to create the SQL Server database:

```bash
dotnet ef database update --project ../Wms.Infrastructure --startup-project .
```

This will:
- Create the SQL Server database (if it doesn't exist)
- Create all tables based on your entity configurations
- Set up indexes and relationships

### Step 6: Verify Database Creation

**Option A: Using SQL Server Management Studio (SSMS)**
1. Open SQL Server Management Studio
2. Connect to `(localdb)\mssqllocaldb` or your SQL Server instance
3. Expand Databases → You should see `WarehouseManagement` database
4. Expand Tables → Verify all tables are created (Items, Locations, Stock, Movements, etc.)

**Option B: Using Command Line**
```bash
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT name FROM sys.databases WHERE name = 'WarehouseManagement'"
```

### Step 7: Run the Application

The application will automatically seed initial data on first run:

```bash
# For Web Application
cd Wms.ASP
dotnet run

# For WinForms Application
cd "Warehouse Management System"
dotnet run
```

## Migrating Existing SQLite Data (Optional)

If you have existing data in SQLite and want to migrate it to SQL Server:

### Option 1: Manual Export/Import

1. **Export from SQLite**:
   ```bash
   sqlite3 wms.db .dump > sqlite_export.sql
   ```

2. **Convert SQL statements** (SQLite syntax differs from SQL Server):
   - Remove SQLite-specific syntax
   - Convert data types if needed
   - Update INSERT statements

3. **Import to SQL Server**:
   ```bash
   sqlcmd -S "(localdb)\mssqllocaldb" -d WarehouseManagement -i converted_export.sql
   ```

### Option 2: Use a Migration Script

Create a C# script to read from SQLite and write to SQL Server:

```csharp
// MigrationScript.cs
using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;
// ... read from SQLite, write to SQL Server
```

### Option 3: Use Application Logic

1. Keep SQLite connection temporarily
2. Read all data using existing repositories
3. Write to SQL Server using new connection
4. Verify data integrity

## Troubleshooting

### Error: "Cannot open database"

**Solution**: Ensure SQL Server is running:
```bash
# For LocalDB
sqllocaldb start mssqllocaldb

# For SQL Server Express
net start MSSQLSERVER
```

### Error: "Login failed"

**Solution**: Check connection string and SQL Server authentication:
- Use `Trusted_Connection=true` for Windows Authentication
- Or provide correct username/password for SQL Authentication

### Error: "Database already exists"

**Solution**: Either:
1. Drop existing database: `DROP DATABASE WarehouseManagement;`
2. Or use a different database name in connection string

### Error: "Migration already applied"

**Solution**: Check migration history:
```bash
dotnet ef migrations list --project ../Wms.Infrastructure --startup-project .
```

## Future Migrations

After the initial migration, for any schema changes:

1. **Create new migration**:
   ```bash
   dotnet ef migrations add MigrationName --project ../Wms.Infrastructure --startup-project .
   ```

2. **Apply migration**:
   ```bash
   dotnet ef database update --project ../Wms.Infrastructure --startup-project .
   ```

3. **Rollback migration** (if needed):
   ```bash
   dotnet ef database update PreviousMigrationName --project ../Wms.Infrastructure --startup-project .
   ```

## Production Deployment

For production environments:

1. **Use proper SQL Server** (not LocalDB)
2. **Update connection string** with production server details
3. **Use migrations** instead of `EnsureCreated()`:
   ```csharp
   // In Program.cs, replace EnsureCreatedAsync() with:
   await context.Database.MigrateAsync();
   ```
4. **Set up connection pooling** and **backup strategy**
5. **Configure SQL Server authentication** properly

## Summary

✅ **Completed Steps**:
- Updated NuGet packages (SQLite → SQL Server)
- Updated connection strings
- Updated DbContext configuration

📋 **Next Steps**:
1. Install EF Core tools: `dotnet tool install --global dotnet-ef`
2. Create migration: `dotnet ef migrations add InitialSqlServerMigration`
3. Apply migration: `dotnet ef database update`
4. Run application: `dotnet run`

The database will be automatically seeded with sample data on first run!

