# 📦 Warehouse Management System (WMS)

> A modern, enterprise-grade Warehouse Management System built with .NET 8, featuring both WinForms desktop application and ASP.NET Core web interface using Clean Architecture principles.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)]()

## ✨ Overview

This Warehouse Management System provides comprehensive inventory management capabilities through two interfaces:
- **🖥️ WinForms Desktop Application**: Professional, scanner-optimized interface for warehouse operations
- **🌐 ASP.NET Core Web Application**: Modern, responsive web interface with Bootstrap 5 styling

Built using Clean Architecture principles with Domain-Driven Design, the system ensures maintainability, testability, and scalability for enterprise-grade warehouse operations.



## 🏗️ Architecture

### Clean Architecture Implementation

```
┌─────────────────┐   ┌─────────────────┐
│   WinForms UI   │   │  ASP.NET Core   │
│   (Desktop)     │   │   (Web MVC)     │
└─────────────────┘   └─────────────────┘
         │                       │
         └───────────┬───────────┘
                     │
            ┌─────────────────┐
            │   Application   │  ← Use Cases, DTOs, Results
            │     Layer       │
            └─────────────────┘
                     │
            ┌─────────────────┐
            │     Domain      │  ← Entities, Value Objects, Services
            │     Layer       │
            └─────────────────┘
                     │
            ┌─────────────────┐
            │ Infrastructure  │  ← Data Access, External Services
            │     Layer       │
            └─────────────────┘
```


## 📂 Project Structure

```
📁 Warehouse Management System/
├── 🎯 Wms.Domain/                    # Domain Layer
│   ├── Entities/                     # Domain Entities (Item, Location, Stock, etc.)
│   ├── ValueObjects/                 # Value Objects (Barcode, Quantity)
│   ├── Enums/                       # Domain Enumerations (MovementType)
│   ├── Services/                    # Domain Services (IStockMovementService)
│   └── Repositories/                # Repository Interfaces
├── 🚀 Wms.Application/              # Application Layer
│   ├── UseCases/                    # Application Use Cases
│   │   ├── Receiving/               # Receiving Operations
│   │   ├── Inventory/               # Stock Management
│   │   ├── Items/                   # Item Management
│   │   ├── Locations/               # Location Management
│   │   ├── Picking/                 # Picking Operations
│   │   └── Reports/                 # Reporting
│   ├── DTOs/                        # Data Transfer Objects
│   └── Common/                      # Shared Application Logic
├── 🔧 Wms.Infrastructure/           # Infrastructure Layer
│   ├── Data/                        # Database Context & Configurations
│   ├── Repositories/                # Repository Implementations
│   └── Services/                    # External Service Implementations
├── 🖥️ Wms.WinForms/                 # WinForms Desktop Application
│   ├── Forms/                       # Application Forms
│   │   ├── DashboardForm.cs         # KPI Dashboard
│   │   ├── ReceivingForm.cs         # Item Receiving
│   │   ├── PutawayForm.cs           # Putaway Operations
│   │   ├── PickingForm.cs           # Order Picking
│   │   ├── InventoryForm.cs         # Stock Management
│   │   ├── ItemManagementForm.cs    # Item Master Data
│   │   └── LocationManagementForm.cs # Location Setup
│   ├── Common/                      # UI Helpers & Utilities
│   └── Program.cs                   # Application Entry Point
├── 🌐 Wms.ASP/                      # ASP.NET Core Web Application
│   ├── Controllers/                 # MVC Controllers
│   ├── Views/                       # Razor Views
│   ├── Models/                      # View Models
│   ├── wwwroot/                     # Static Assets
│   └── Program.cs                   # Web Application Entry Point
└── 🧪 Test Projects/                # Unit & Integration Tests
    ├── Wms.Domain.Tests/            # Domain Layer Tests
    ├── Wms.Application.Tests/       # Application Layer Tests
    └── Wms.Infrastructure.Tests/    # Infrastructure Layer Tests
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- SQLite (embedded database - no additional setup required)

### 🖥️ Running the Desktop Application

```bash
# Clone the repository
git clone https://github.com/RealAhmedOsama/Warehouse-Management-System.git
cd "Warehouse Management System"

# Restore NuGet packages
dotnet restore

# Run the WinForms application
dotnet run --project "Warehouse Management System/Wms.WinForms.csproj"
```

### 🌐 Running the Web Application

```bash
# Navigate to the web project
cd Wms.ASP

# Run the web application
dotnet run

# Open browser to https://localhost:5001 or http://localhost:5000
```

### 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Wms.Domain.Tests/
```







#### Form Navigation Shortcuts

| Form | Shortcut | Primary Function |
|------|----------|------------------|
| Dashboard | - | System overview and KPIs |
| Receiving | F1=Receive, F2=Clear | Barcode-based receiving |
| Putaway | F1=Putaway, F2=Clear | Location transfers |
| Picking | F1=Pick, F2=Clear | Order fulfillment |
| Inventory | F1=Refresh, F2=Adjust, F3=Summary | Stock management |
| Items | F1=Refresh, F2=Add, F3=Edit | Item master data |
| Locations | F1=Refresh, F2=Add, F3=Edit | Location master data |
| Reports | F1=Generate, F2=Export | Movement reporting |

## ⚙️ Configuration

### Database Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=warehouse.db"
  }
}
```

### Application Settings

```json
{
  "WMS": {
    "Dashboard": {
      "RefreshIntervalSeconds": 300,
      "LowStockThreshold": 10
    },
    "Barcode": {
      "ScanTimeout": 5000,
      "ValidationEnabled": true
    }
  }
}
```

## 📚 API Reference

### Key Use Cases

```csharp
// Receiving an item
var result = await receiveItemUseCase.ExecuteAsync(new ReceiveItemDto
{
    ItemSku = "WIDGET-001",
    LocationCode = "RECEIVE",
    Quantity = 100,
    LotNumber = "LOT123",
    ReferenceNumber = "PO-456"
}, userId);

// Getting stock levels
var stock = await getStockUseCase.ExecuteAsync(new GetStockQuery
{
    ItemSku = "WIDGET-001",
    LocationCode = "A001"
});

// Creating a new item
var item = await createItemUseCase.ExecuteAsync(new CreateItemDto
{
    Sku = "NEW-001",
    Name = "New Product",
    UnitOfMeasure = "EA",
    RequiresLot = true
}, userId);
```



















This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**SOUHAIL**
- GitHub: (https://github.com/souhailwaf)
- Email: Contact via GitHub

## 🙏 Acknowledgments

- **Microsoft** for the excellent .NET ecosystem and tooling
- **Entity Framework Team** for the powerful ORM capabilities
- **Bootstrap Team** for the amazing responsive CSS framework
- **Serilog Community** for structured logging excellence
- **Open Source Community** for inspiration and best practices

## 🔗 Related Documentation

- [UI Enhancement Guide](./Warehouse%20Management%20System/README-UI-ENHANCEMENTS.md)
- [Responsive Design Implementation](./Warehouse%20Management%20System/RESPONSIVE-UI-DOCUMENTATION.md)
- [Runtime Fixes Documentation](./Warehouse%20Management%20System/RUNTIME-FIXES-DOCUMENTATION.md)

---

<div align="center">
  


---

<p>Built with  using .NET 8, Clean Architecture, and modern design principles</p>
<p><strong>⭐ Star this repo if you find it helpful!</strong></p>

</div>#   w a r e h o 
 
 
