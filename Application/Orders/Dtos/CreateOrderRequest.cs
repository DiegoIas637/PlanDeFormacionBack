namespace Application.Orders.Dtos
{
    public class CreateOrderRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
