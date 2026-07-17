public class MomoResponse
{
    public string partnerCode { get; set; }
    public string orderId { get; set; }
    public string requestId { get; set; }
    public long amount { get; set; }
    public string orderInfo { get; set; }
    public string orderType { get; set; }
    public string transId { get; set; }
    public int resultCode { get; set; }
    public string message { get; set; }
    public string payUrl { get; set; } // Đây là link chứa mã QR
    public string signature { get; set; }
}