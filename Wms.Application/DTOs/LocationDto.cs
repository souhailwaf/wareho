// Wms.Application/DTOs/LocationDto.cs

namespace Wms.Application.DTOs;

public record LocationDto(
    int Id,
    string Code,
    string Name,
    int WarehouseId,
    int? ParentLocationId,
    bool IsPickable,
    bool IsReceivable,
    bool IsActive,
    int Capacity,
    string FullPath,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateLocationDto(
    string Code,
    string Name,
    int WarehouseId,
    int? ParentLocationId = null,
    bool IsPickable = true,
    bool IsReceivable = true,
    int Capacity = 1000
);

public record UpdateLocationDto(
    string Name,
    bool IsPickable = true,
    bool IsReceivable = true,
    int Capacity = 1000
);