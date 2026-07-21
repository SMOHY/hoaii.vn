using Hoaii.Domain.Entities;

namespace Hoaii.Web.Models.Category;

public static class ProductCardMapper
{
    public static ProductCardViewModel Map(Hoaii.Domain.Entities.Product p, string cardVariant = "grid")
    {
        var (label, badgeVariant) = p.Badge switch
        {
            ProductBadge.New => ("Hàng mới", "new"),
            ProductBadge.Sale when p.CompareAtPrice is > 0 =>
                ($"-{Math.Round((1 - p.Price / p.CompareAtPrice!.Value) * 100)}%", "sale"),
            ProductBadge.Sale => ("Giảm giá", "sale"),
            ProductBadge.OutOfStock => ("Hết hàng", "out-of-stock"),
            _ => ("", ""),
        };

        return new ProductCardViewModel
        {
            Id = p.Id,
            Slug = p.Slug,
            ImageUrl = p.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.Url,
            Name = p.Name,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            BadgeLabel = label,
            BadgeVariant = badgeVariant,
            VariantCount = p.Variants.Count,
            VariantNames = p.Variants.Select(v => v.Name).ToList(),
            CardVariant = cardVariant,
        };
    }
}
