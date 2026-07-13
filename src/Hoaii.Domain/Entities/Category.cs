namespace Hoaii.Domain.Entities;

public enum CategoryType
{
    ProductType, // Trà, Khăn, Tượng gốm, Rượu
    Occasion,    // Quà tết, Quà trung thu, Quà theo dịp, ...
}

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public CategoryType Type { get; set; }
    public int SortOrder { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}
