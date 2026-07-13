namespace Hoaii.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public required string FullName { get; set; }
    public required string Phone { get; set; }

    public int ProvinceId { get; set; }
    public Province Province { get; set; } = null!;
    public int WardId { get; set; }
    public Ward Ward { get; set; } = null!;

    public required string AddressDetail { get; set; }
    public string? PostalCode { get; set; }
    public bool IsDefault { get; set; }
}
