namespace Hoaii.Domain.Entities;

/// <summary>Admin-curated "related products" pick for one product's detail page (up to 4, see
/// ProductsController's picker) — ProductController.Details falls back to random same-category
/// products when a product has none of these. Both FKs point at Product, so only one side
/// cascades (ProductId) to dodge SQL Server's multiple-cascade-paths rejection on a self-join;
/// ProductsController.Delete cleans up rows where the deleted product is the RelatedProductId
/// side by hand.</summary>
public class RelatedProduct
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int RelatedProductId { get; set; }
    public Product? RelatedTo { get; set; }
    public int SortOrder { get; set; }
}
