namespace HouseHelp.Contracts.Residents;

public record ResidentFlatDto(Guid FlatId, Guid BuildingId, string Number, string BuildingName);

public record ResidentFlatsResponseDto(IReadOnlyList<ResidentFlatDto> Flats);
