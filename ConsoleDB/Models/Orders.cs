namespace ConsoleDB.Models
{
    /// <summary>
    /// 注文
    /// </summary>
    public class Orders
    {
        public long OrderId { get; set; }
        public long CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public long Quantity { get; set; }
    }
}
