using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;

namespace hw_2_2_3_26.Services;

public interface ICountryService
{
    Task<IEnumerable<CountrySummaryDto>> GetAllCountries(CancellationToken ct);
    Task<CountryDetailDto?> GetCountryById(int id, CancellationToken ct);
    Task<IEnumerable<CountryDetailDto>> GetCountryBySearchParameters(CountrySearchParameters parameters, CancellationToken ct);
    Task<CountrySummaryDto> Create(CreateCountryRequest request, CancellationToken ct);
    Task<bool> Update(int id, UpdateCountryRequest request, CancellationToken ct);
    Task<bool> PartialUpdate(int id, PartialUpdateCountryRequest request, CancellationToken ct);
    Task<bool> Delete(int id, CancellationToken ct);
}