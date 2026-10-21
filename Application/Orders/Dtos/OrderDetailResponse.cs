namespace Application.Orders.Dtos
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal AppliedPrice { get; set; }
        public decimal Total { get; set; }
        public OrderUserResponse User { get; set; } = new();
        public OrderProductResponse Product { get; set; } = new();
    }

    public class OrderUserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class OrderProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
