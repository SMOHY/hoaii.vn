using Hoaii.Web.Models.Category;

namespace Hoaii.Web.Models.Search;

public class SearchGroupViewModel
{
    public required string CategoryName { get; init; }
    public required int TotalCount { get; init; }
    public required IReadOnlyList<ProductCardViewModel> Products { get; init; }
    public required string ShowMoreUrl { get; init; }
    public bool IsFallback { get; init; }
}

public class SearchPageViewModel
{
    public required string Query { get; init; }
    public required int TotalResultCount { get; init; }
    public required IReadOnlyList<SearchGroupViewModel> Groups { get; init; }
}
