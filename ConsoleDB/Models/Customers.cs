namespace ConsoleDB.Models
{
    /// <summary>
    /// 顧客
    /// </summary>
    public class Customers
    {
        public long CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public DateTime RegisterdDate { get; set; }
    }
}
