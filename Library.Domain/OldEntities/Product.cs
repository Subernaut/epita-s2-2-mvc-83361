namespace Library.Domain.OldEntities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }

        public List<InvoiceLine> InvoiceLines { get; set; } = new();
    }
}
