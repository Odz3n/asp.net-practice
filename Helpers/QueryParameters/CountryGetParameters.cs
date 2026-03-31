namespace hw_2_2_3_26.Helpers.QueryParameters;

public record CountryGetParameters(
    int Page,
    int Size,
    string? Sort,
    string? Search,
    string? Author,
    string? Publisher
);