namespace DuploBackend.Models.DTOs;

public class OrderRequest
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}
