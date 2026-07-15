namespace Hoaii.Domain.Entities;

public class BlogPost
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Category { get; set; }
    public string? Excerpt { get; set; }

    /// <summary>Full article body (plain text / simple HTML). The detail page renders this;
    /// falls back to the excerpt when empty so old posts still show something.</summary>
    public string? Content { get; set; }

    public string? Author { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFeatured { get; set; }

    /// <summary>Drafts (false) are hidden from the storefront but still editable in admin.</summary>
    public bool IsPublished { get; set; } = true;

    public DateTime PublishedAt { get; set; }
}
