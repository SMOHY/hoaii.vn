using Hoaii.Domain.Entities;

namespace Hoaii.Web.Areas.Admin.Models;

public class ProductListViewModel
{
    public IReadOnlyList<Row> Products { get; init; } = [];
    public int Page { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    public string? Query { get; init; }
    public int? CategoryId { get; init; }
    public IReadOnlyList<Category> Categories { get; init; } = [];

    public class Row
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public string? ImageUrl { get; init; }
        public required string CategoryName { get; init; }
        public decimal Price { get; init; }
        public int Stock { get; init; }
        public bool IsActive { get; init; }
        public bool IsFeatured { get; init; }
        public ProductBadge Badge { get; init; }
    }
}

/// <summary>Everything the product create/edit form needs — the product, its images and variants.</summary>
public class ProductEditViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Slug { get; init; } = "";
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public ProductBadge Badge { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsActive { get; init; } = true;
    public int SortOrder { get; init; }
    public int CategoryId { get; init; }
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
    public string? StoryTitle { get; init; }
    public string? StoryBody { get; init; }
    public string? StoryImageUrl { get; init; }
    public string? FeatureTitle { get; init; }
    public string? FeatureBody { get; init; }
    public string? FeatureImageUrl { get; init; }

    // Story / feature copy moves into the DB in phase 2 (CMS); for now the storefront still
    // templates it in ProductController.

    public IReadOnlyList<string> ImageUrls { get; init; } = [];
    public IReadOnlyList<VariantRow> Variants { get; init; } = [];
    public IReadOnlyList<Category> Categories { get; init; } = [];

    public class VariantRow
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public decimal PriceModifier { get; init; }
        public string? Sku { get; init; }
        public int StockQuantity { get; init; }
    }
}
