namespace hw_2_2_3_26.Helpers.QueryParameters;

public record PublisherGetParameters(
    int Page,
    int Size,
    string? Sort,
    string? Search,
    string? Country,
    string? Book,
    DateTime? FoundationDate
);