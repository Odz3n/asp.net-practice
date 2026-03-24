using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;

namespace hw_2_2_3_26.Services;

public interface IPublisherService
{
    Task<IEnumerable<PublisherSummaryDto>> GetAllPublishers(CancellationToken ct);
    Task<PublisherDetailDto?> GetPublisherById(int id, CancellationToken ct);
    Task<IEnumerable<PublisherDetailDto>> GetPublisherBySearchParameters(PublisherSearchParameters parameters, CancellationToken ct);
    Task<PublisherSummaryDto> Create(CreatePublisherRequest request, CancellationToken ct);
    Task<bool> Update(int id, UpdatePublisherRequest request, CancellationToken ct);
    Task<bool> PartialUpdate(int id, PartialUpdatePublisherRequest request, CancellationToken ct);
    Task<bool> Delete(int id, CancellationToken ct);
}