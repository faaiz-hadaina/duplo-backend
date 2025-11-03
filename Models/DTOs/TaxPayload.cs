using System.Text.Json.Serialization;

namespace DuploBackend.Models.DTOs;

public class TaxPayload
{
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("platform_code")]
    public string PlatformCode { get; set; } = "022";

    [JsonPropertyName("order_amount")]
    public decimal OrderAmount { get; set; }
}
