using Microsoft.AspNetCore.Mvc;
using Wms.Application.UseCases.Inventory;
using Wms.ASP.Filters;
using Wms.ASP.Models;

namespace Wms.ASP.Controllers;

[RequireAuth]
public class InventoryController : Controller
{
    private string CurrentUserId => HttpContext.Session.GetString("Username") ?? "UNKNOWN";
    private readonly IGetStockUseCase _getStockUseCase;
    private readonly ILogger<InventoryController> _logger;
    private readonly IStockAdjustmentUseCase _stockAdjustmentUseCase;

    public InventoryController(
        IGetStockUseCase getStockUseCase,
        IStockAdjustmentUseCase stockAdjustmentUseCase,
        ILogger<InventoryController> logger)
    {
        _getStockUseCase = getStockUseCase;
        _stockAdjustmentUseCase = stockAdjustmentUseCase;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? searchTerm, bool showSummary = false)
    {
        try
        {
            var model = new InventoryViewModel
            {
                SearchTerm = searchTerm,
                ShowSummary = showSummary
            };

            if (showSummary)
            {
                var summaryResult = await _getStockUseCase.GetStockSummaryAsync();
                if (summaryResult.IsSuccess)
                {
                    model.StockSummary = summaryResult.Value.OrderBy(s => s.ItemSku).ToList();
                }
            }
            else
            {
                var result = await _getStockUseCase.GetAllStockAsync();
                if (result.IsSuccess)
                {
                    var stockItems = result.Value.Where(s => s.AvailableQuantity > 0);

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        stockItems = stockItems.Where(s =>
                            s.ItemSku.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            s.ItemName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            s.LocationCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    }

                    model.StockItems = stockItems.ToList();
                }
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inventory data");
            TempData["ErrorMessage"] = "Error loading inventory data. Please try again.";
            return View(new InventoryViewModel());
        }
    }

    [HttpGet]
    public IActionResult Adjust(string itemSku, string locationCode, decimal currentQuantity)
    {
        var model = new StockAdjustmentViewModel
        {
            ItemSku = itemSku,
            LocationCode = locationCode,
            CurrentQuantity = currentQuantity,
            NewQuantity = currentQuantity
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Adjust(StockAdjustmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new StockAdjustmentDto(
                model.ItemSku,
                model.LocationCode,
                model.NewQuantity,
                model.Reason
            );

            var result = await _stockAdjustmentUseCase.ExecuteAsync(request, CurrentUserId);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return View(model);
            }

            TempData["SuccessMessage"] = $"Stock adjusted successfully! Movement ID: {result.Value.MovementId}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adjusting stock");
            TempData["ErrorMessage"] = "Error adjusting stock. Please try again.";
            return View(model);
        }
    }
}