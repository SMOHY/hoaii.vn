namespace Hoaii.Domain.Entities;

public class BlogPost
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Category { get; set; }
    public string? Excerpt { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime PublishedAt { get; set; }
}
