using System.Text;
using Hoaii.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class SitemapController(HoaiiDbContext db) : Controller
{
    // Static top-level pages that aren't backed by a DB table. Kept in one place so a new
    // top-level route only needs adding here, not hunted down across the sitemap logic.
    private static readonly string[] StaticPaths =
    [
        "/",
        "/ve-chung-toi",
        "/hop-tac",
        "/lien-he",
        "/blog",
        "/qua-theo-dip",
        "/qua-tang-ca-nhan",
    ];

    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var urls = new List<(string Loc, DateTime? LastMod)>();

        foreach (var path in StaticPaths)
        {
            urls.Add((baseUrl + path, null));
        }

        var categorySlugs = await db.Categories
            .Select(c => c.Slug)
            .ToListAsync();
        urls.AddRange(categorySlugs.Select(slug => (baseUrl + "/danh-muc/" + slug, (DateTime?)null)));

        var products = await db.Products
            .Where(p => p.IsActive)
            .Select(p => new { p.Slug, p.UpdatedAt, p.CreatedAt })
            .ToListAsync();
        urls.AddRange(products.Select(p => (baseUrl + "/san-pham/" + p.Slug, (DateTime?)(p.UpdatedAt ?? p.CreatedAt))));

        var posts = await db.BlogPosts
            .Where(p => p.IsPublished)
            .Select(p => new { p.Slug, p.PublishedAt })
            .ToListAsync();
        urls.AddRange(posts.Select(p => (baseUrl + "/blog/" + p.Slug, (DateTime?)p.PublishedAt)));

        var policyPages = await db.PolicyPages
            .Where(p => p.IsPublished)
            .Select(p => p.Slug)
            .ToListAsync();
        urls.AddRange(policyPages.Select(slug => (baseUrl + "/chinh-sach/" + slug, (DateTime?)null)));

        var xml = new StringBuilder();
        xml.Append("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.Append("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");
        foreach (var (loc, lastMod) in urls)
        {
            xml.Append("<url><loc>").Append(System.Net.WebUtility.HtmlEncode(loc)).Append("</loc>");
            if (lastMod is not null)
            {
                xml.Append("<lastmod>").Append(lastMod.Value.ToString("yyyy-MM-dd")).Append("</lastmod>");
            }
            xml.Append("</url>");
        }
        xml.Append("</urlset>");

        return Content(xml.ToString(), "application/xml", Encoding.UTF8);
    }
}
