namespace Hoaii.Domain.Entities;

public class Ward
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public Province Province { get; set; } = null!;
    public required string Name { get; set; }
}
