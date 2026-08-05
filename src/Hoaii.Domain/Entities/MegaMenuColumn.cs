namespace Hoaii.Domain.Entities;

/// <summary>How a custom mega-menu column gets its content — the 3 choices an admin picks
/// between when adding a column (see Areas/Admin/Views/Menu/Index.cshtml).</summary>
public enum MegaMenuColumnKind
{
    /// <summary>Admin hand-picks specific products, ordered, capped at 4 (MegaMenuColumnItem.ProductId).</summary>
    Pick,
    /// <summary>Shows products tagged with one specific Collection (CollectionId), same idea as
    /// the built-in "Theo bộ sưu tập" column but pinned to a collection the admin names, rather
    /// than following whichever collection the current category's products happen to use.</summary>
    Collection,
    /// <summary>A short list of links to other categories (MegaMenuColumnItem.CategoryId) — same
    /// shape as the built-in "Sản phẩm"/"Theo bộ sưu tập" auto columns, but admin-curated.</summary>
    CategoryLinks,
}

/// <summary>An admin-added column in one of the 4 built-in mega-menu panels, alongside the
/// built-in "Bán chạy nhất"/"Phiên bản giới hạn"/etc columns that live in code
/// (MenuController.CuratedSlots) rather than in a table. Those stay code-defined because they're
/// part of the original Figma layout; this table is for whatever an admin adds beyond that.</summary>
public class MegaMenuColumn
{
    public int Id { get; set; }
    public required string PanelKey { get; set; }
    public required string Title { get; set; }
    public int SortOrder { get; set; }
    public MegaMenuColumnKind Kind { get; set; }

    /// <summary>Only used when Kind == Collection.</summary>
    public int? CollectionId { get; set; }
    public Collection? Collection { get; set; }

    public List<MegaMenuColumnItem> Items { get; set; } = [];
}

/// <summary>One row inside a MegaMenuColumn — a product (Kind == Pick) or a category link
/// (Kind == CategoryLinks). Which field is populated depends on the parent column's Kind.</summary>
public class MegaMenuColumnItem
{
    public int Id { get; set; }
    public int MegaMenuColumnId { get; set; }
    public MegaMenuColumn? Column { get; set; }
    public int SortOrder { get; set; }

    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}
