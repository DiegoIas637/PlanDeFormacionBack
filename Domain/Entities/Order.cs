namespace Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal AppliedPrice { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
