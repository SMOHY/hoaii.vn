namespace Hoaii.Web.Models.Blog;

public class BlogCardViewModel
{
    public string? ImageUrl { get; init; }
    public required string Category { get; init; }
    public required string Title { get; init; }
    public string? Excerpt { get; init; }
    public required string DateText { get; init; }
    public required string Url { get; init; }
}

public class BlogIndexViewModel
{
    public required BlogCardViewModel? Featured { get; init; }
    public required IReadOnlyList<BlogCardViewModel> Posts { get; init; }
}

public class BlogPostDetailsViewModel
{
    public required string Title { get; init; }
    public required string Category { get; init; }
    public required string DateText { get; init; }
    public string? Author { get; init; }
    public string? ImageUrl { get; init; }
    public string? Excerpt { get; init; }
    public string? Content { get; init; }
}
