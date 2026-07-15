using Hoaii.Infrastructure;
using Hoaii.Web.Models.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class BlogController(HoaiiDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var posts = await db.BlogPosts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();

        var featuredPost = posts.FirstOrDefault(p => p.IsFeatured) ?? posts.FirstOrDefault();
        var rest = posts.Where(p => p != featuredPost).ToList();

        var model = new BlogIndexViewModel
        {
            Featured = featuredPost is null ? null : MapCard(featuredPost),
            Posts = rest.Select(MapCard).ToList(),
        };

        return View(model);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var post = await db.BlogPosts.FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
        if (post is null)
        {
            return NotFound();
        }

        return View(new BlogPostDetailsViewModel
        {
            Title = post.Title,
            Category = post.Category,
            DateText = post.PublishedAt.ToString("dd/MM/yyyy"),
            Author = post.Author,
            ImageUrl = post.ImageUrl,
            Excerpt = post.Excerpt,
            Content = post.Content,
        });
    }

    private static BlogCardViewModel MapCard(Domain.Entities.BlogPost p) => new()
    {
        ImageUrl = p.ImageUrl,
        Category = p.Category,
        Title = p.Title,
        Excerpt = p.Excerpt,
        DateText = p.PublishedAt.ToString("dd/MM/yyyy"),
        Url = $"/blog/{p.Slug}",
    };
}
