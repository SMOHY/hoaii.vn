namespace Hoaii.Domain.Entities;

/// <summary>
/// A storefront policy / terms page rendered at /chinh-sach/{slug}. The body is an ordered list
/// of blocks (paragraphs, headings, bullets) so the admin can rearrange copy without touching code.
/// </summary>
public class PolicyPage
{
    public int Id { get; set; }
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public required string NavLabel { get; set; }
    public required string BreadcrumbLabel { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; } = true;

    public List<PolicyBlock> Blocks { get; set; } = [];
}

public class PolicyBlock
{
    public int Id { get; set; }
    public int PolicyPageId { get; set; }
    public PolicyBlockKind Kind { get; set; }
    public required string Text { get; set; }
    public int SortOrder { get; set; }

    public PolicyPage? PolicyPage { get; set; }
}

public enum PolicyBlockKind
{
    Paragraph,
    Heading,
    Bullet,
}
