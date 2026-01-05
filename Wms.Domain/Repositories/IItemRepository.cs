// Wms.Domain/Repositories/IItemRepository.cs

using Wms.Domain.Entities;
using Wms.Domain.ValueObjects;

namespace Wms.Domain.Repositories;

public interface IItemRepository : IRepository<Item>
{
    Task<Item?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Item?> GetByBarcodeAsync(Barcode barcode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);
}