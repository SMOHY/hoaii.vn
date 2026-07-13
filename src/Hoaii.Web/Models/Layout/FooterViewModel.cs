namespace Hoaii.Web.Models.Layout;

public class FooterLink
{
    public required string Label { get; init; }
    public required string Url { get; init; }
}

public class FooterColumn
{
    public required string Title { get; init; }
    public required IReadOnlyList<FooterLink> Links { get; init; }
}

public class SocialLink
{
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required string IconPath { get; init; }
}

public class FooterViewModel
{
    public required IReadOnlyList<FooterColumn> Columns { get; init; }
    public required IReadOnlyList<SocialLink> SocialLinks { get; init; }
}
