namespace Finance.Data.Models;

public class Address
{
    public string FullName { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string Town { get; set; } = null!;
    public string Country { get; set; } = null!;
}