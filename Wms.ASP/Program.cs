using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Wms.Application.UseCases.Inventory;
using Wms.Application.UseCases.Items;
using Wms.Application.UseCases.Locations;
using Wms.Application.UseCases.Picking;
using Wms.Application.UseCases.Receiving;
using Wms.Application.UseCases.Reports;
using Wms.Domain.Entities;
using Wms.Domain.Repositories;
using Wms.Domain.Services;
using Wms.Infrastructure.Data;
using Wms.Infrastructure.Repositories;
using Wms.Infrastructure.Services;

namespace Wms.ASP;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add Session
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // Add Entity Framework
        builder.Services.AddDbContext<WmsDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection")
                            ?? "Server=localhost;Database=warehouse;User=root;Password=;Port=3306;",
                            new MySqlServerVersion(new Version(8, 0, 21))));

        // Register infrastructure services
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IStockMovementService, StockMovementService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        // Register application services
        builder.Services.AddScoped<IGetStockUseCase, GetStockUseCase>();
        builder.Services.AddScoped<IStockAdjustmentUseCase, StockAdjustmentUseCase>();
        builder.Services.AddScoped<IGetItemsUseCase, GetItemsUseCase>();
        builder.Services.AddScoped<ICreateItemUseCase, CreateItemUseCase>();
        builder.Services.AddScoped<IUpdateItemUseCase, UpdateItemUseCase>();
        builder.Services.AddScoped<IGetLocationsUseCase, GetLocationsUseCase>();
        builder.Services.AddScoped<ICreateLocationUseCase, CreateLocationUseCase>();
        builder.Services.AddScoped<IReceiveItemUseCase, ReceiveItemUseCase>();
        builder.Services.AddScoped<IPutawayUseCase, PutawayUseCase>();
        builder.Services.AddScoped<IPickOrderUseCase, PickOrderUseCase>();
        builder.Services.AddScoped<IMovementReportUseCase, MovementReportUseCase>();

        var app = builder.Build();

        // Initialize Database
        await InitializeDatabaseAsync(app);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();

        app.UseAuthorization();

        app.MapControllerRoute(
            "default",
            "{controller=Dashboard}/{action=Index}/{id?}");

        app.Run();
    }

    private static async Task InitializeDatabaseAsync(WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WmsDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Ensure Users table exists (for existing databases)
            await EnsureUsersTableExistsAsync(context, logger);

            // Seed initial data if needed
            await SeedInitialDataAsync(context, logger, loggerFactory);

            logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task SeedInitialDataAsync(WmsDbContext context, ILogger logger, ILoggerFactory loggerFactory)
    {
        // Check if we already have data
        if (await context.Items.AnyAsync() || await context.Locations.AnyAsync())
        {
            // Still check if we need to seed default user
            if (!await context.Users.AnyAsync())
            {
                await SeedDefaultUserAsync(context, loggerFactory);
            }
            return; // Database already seeded
        }

        // Create default warehouse
        var warehouse = new Warehouse("Main Warehouse", "MAIN");
        context.Warehouses.Add(warehouse);
        await context.SaveChangesAsync();

        // Create some sample locations
        var receivingLocation = new Location("RECEIVING", "Receiving Area", warehouse.Id);
        receivingLocation.SetReceivable(true);
        receivingLocation.SetPickable(false);

        var storageLocation = new Location("A001", "Storage Area A1", warehouse.Id);
        storageLocation.SetReceivable(true);
        storageLocation.SetPickable(true);

        var shippingLocation = new Location("SHIPPING", "Shipping Area", warehouse.Id);
        shippingLocation.SetReceivable(false);
        shippingLocation.SetPickable(true);

        context.Locations.AddRange(receivingLocation, storageLocation, shippingLocation);

        // Create some sample items
        var item1 = new Item("WIDGET-001", "Standard Widget", "EA");
        var item2 = new Item("GADGET-001", "Premium Gadget", "EA", true);
        var item3 = new Item("TOOL-001", "Professional Tool", "EA", requiresSerial: true);

        context.Items.AddRange(item1, item2, item3);

        await context.SaveChangesAsync();

        // Create default user
        await SeedDefaultUserAsync(context, loggerFactory);

        logger.LogInformation("Initial seed data created successfully");
    }

    private static async Task SeedDefaultUserAsync(WmsDbContext context, ILoggerFactory loggerFactory)
    {
        if (await context.Users.AnyAsync(u => u.Username == "admin" || u.Email == "admin@wms.com"))
        {
            return; // Default user already exists
        }

        // Create default admin user with password "admin123"
        var authLogger = loggerFactory.CreateLogger<Infrastructure.Services.AuthService>();
        var authService = new Infrastructure.Services.AuthService(
            new Infrastructure.Repositories.UserRepository(context),
            authLogger
        );
        
        var passwordHash = authService.HashPassword("admin123");
        var defaultUser = new User("admin", "admin@wms.com", passwordHash, "Administrator");
        
        context.Users.Add(defaultUser);
        await context.SaveChangesAsync();

        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogInformation("Default admin user created (username: admin, password: admin123)");
    }

    private static async Task EnsureUsersTableExistsAsync(WmsDbContext context, ILogger logger)
    {
        try
        {
            // Try to query the table to see if it exists
            try
            {
                await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM Users LIMIT 1");
                logger.LogInformation("Users table already exists");
                return;
            }
            catch
            {
                // Table doesn't exist, create it
                logger.LogInformation("Creating Users table...");
            }

            // Create Users table
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS `Users` (
                    `Id` INT NOT NULL AUTO_INCREMENT,
                    `Username` VARCHAR(50) NOT NULL,
                    `Email` VARCHAR(200) NOT NULL,
                    `PasswordHash` VARCHAR(255) NOT NULL,
                    `FullName` VARCHAR(200) NOT NULL,
                    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
                    `CreatedAt` DATETIME(6) NOT NULL,
                    `UpdatedAt` DATETIME(6) NULL,
                    PRIMARY KEY (`Id`),
                    UNIQUE INDEX `IX_Users_Username` (`Username`),
                    UNIQUE INDEX `IX_Users_Email` (`Email`),
                    INDEX `IX_Users_IsActive` (`IsActive`)
                ) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci");

            logger.LogInformation("Users table created successfully");
        }
        catch (Exception ex)
        {
            // Table might already exist or there's another issue
            logger.LogWarning(ex, "Could not create Users table: {Error}", ex.Message);
        }
    }
}