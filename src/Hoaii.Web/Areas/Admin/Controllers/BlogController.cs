using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class BlogController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/blog")]
    public async Task<IActionResult> Index()
    {
        var posts = await Db.BlogPosts
            .OrderByDescending(p => p.PublishedAt)
            .Select(p => new { p.Id, p.Title, p.Slug, p.Category, p.IsFeatured, p.IsPublished, p.PublishedAt })
            .ToListAsync();
        return View(posts
            .Select(p => (p.Id, p.Title, p.Slug, p.Category, p.IsFeatured, p.IsPublished, p.PublishedAt))
            .ToList());
    }

    [HttpGet("/admin/blog/them")]
    public IActionResult Create() =>
        View("Edit", new BlogPost { Title = "", Slug = "", Category = "Đời sống", PublishedAt = DateTime.Now });

    [HttpGet("/admin/blog/{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await Db.BlogPosts.FindAsync(id);
        if (post is null) return NotFound();
        return View(post);
    }

    [HttpPost("/admin/blog/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int id, string title, string? slug, string category, string? author,
        string? excerpt, string? content, string? imageUrl,
        bool isFeatured, bool isPublished, DateTime? publishedAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Fail("Tiêu đề không được để trống.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        var finalSlug = string.IsNullOrWhiteSpace(slug) ? Slug.From(title) : Slug.From(slug);
        if (await Db.BlogPosts.AnyAsync(p => p.Slug == finalSlug && p.Id != id))
        {
            Fail($"Slug \"{finalSlug}\" đã tồn tại.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        if (id == 0)
        {
            var post = new BlogPost
            {
                Title = title.Trim(),
                Slug = finalSlug,
                Category = string.IsNullOrWhiteSpace(category) ? "Đời sống" : category.Trim(),
                Author = author?.Trim(),
                Excerpt = excerpt?.Trim(),
                Content = content,
                ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim(),
                IsFeatured = isFeatured,
                IsPublished = isPublished,
                PublishedAt = publishedAt ?? DateTime.Now,
            };
            Db.BlogPosts.Add(post);
            auth.Audit("Thêm bài viết", nameof(BlogPost), null, title);
            await EnforceSingleFeaturedAsync(post);
            await Db.SaveChangesAsync();
            Ok("Đã thêm bài viết.");
        }
        else
        {
            var post = await Db.BlogPosts.FindAsync(id);
            if (post is null) return NotFound();
            post.Title = title.Trim();
            post.Slug = finalSlug;
            post.Category = string.IsNullOrWhiteSpace(category) ? "Đời sống" : category.Trim();
            post.Author = author?.Trim();
            post.Excerpt = excerpt?.Trim();
            post.Content = content;
            post.ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
            post.IsFeatured = isFeatured;
            post.IsPublished = isPublished;
            post.PublishedAt = publishedAt ?? post.PublishedAt;
            auth.Audit("Sửa bài viết", nameof(BlogPost), id, title);
            await EnforceSingleFeaturedAsync(post);
            await Db.SaveChangesAsync();
            Ok("Đã lưu bài viết.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/blog/{id:int}/an-hien")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var post = await Db.BlogPosts.FindAsync(id);
        if (post is null) return NotFound();
        post.IsPublished = !post.IsPublished;
        auth.Audit(post.IsPublished ? "Đăng bài viết" : "Ẩn bài viết", nameof(BlogPost), id, post.Title);
        await Db.SaveChangesAsync();
        Ok(post.IsPublished ? "Đã đăng bài viết." : "Đã ẩn bài viết.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/blog/{id:int}/xoa")]
    [Authorize(Policy = AdminAuth.PolicyOwner)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await Db.BlogPosts.FindAsync(id);
        if (post is null) return NotFound();
        Db.BlogPosts.Remove(post);
        auth.Audit("Xóa bài viết", nameof(BlogPost), id, post.Title);
        await Db.SaveChangesAsync();
        Ok("Đã xóa bài viết.");
        return RedirectToAction(nameof(Index));
    }

    // Only one post carries the "featured" flag — the home strip and blog page each show a single
    // featured card, so clearing the others keeps that promise no matter what the admin toggles.
    private async Task EnforceSingleFeaturedAsync(BlogPost featured)
    {
        if (!featured.IsFeatured) return;
        await Db.BlogPosts
            .Where(p => p.IsFeatured && p.Id != featured.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsFeatured, false));
    }
}
