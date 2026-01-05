// Wms.Infrastructure/Repositories/ItemRepository.cs

using Microsoft.EntityFrameworkCore;
using Wms.Domain.Entities;
using Wms.Domain.Repositories;
using Wms.Domain.ValueObjects;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(WmsDbContext context) : base(context)
    {
    }

    public async Task<Item?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.Sku == sku.ToUpperInvariant(), cancellationToken);
    }

    public async Task<Item?> GetByBarcodeAsync(Barcode barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.Barcodes.Any(b => b.Value == barcode.Value))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToUpperInvariant();
        return await _dbSet
            .Where(i => i.Sku.Contains(term) ||
                        i.Name.ToUpper().Contains(term) ||
                        i.Barcodes.Any(b => b.Value.Contains(term)))
            .OrderBy(i => i.Sku)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(i => i.Sku == sku.ToUpperInvariant(), cancellationToken);
    }

    public override async Task<IEnumerable<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(i => i.Sku)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetActiveItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.IsActive)
            .OrderBy(i => i.Sku)
            .ToListAsync(cancellationToken);
    }
}