namespace Hoaii.Domain.Entities;

public enum Gender
{
    Male,
    Female,
}

public class Customer
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Address> Addresses { get; set; } = [];
}
