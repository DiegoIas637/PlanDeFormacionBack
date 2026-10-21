namespace Application.Orders.Dtos
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public decimal AppliedPrice { get; set; }
    }
}
