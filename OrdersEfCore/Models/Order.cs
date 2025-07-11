namespace OrdersEfCore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int AnalysisId { get; set; }

        public Analysis Analysis { get; set; }
    }
}
