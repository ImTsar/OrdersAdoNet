namespace OrdersEfCore.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int AnalysisId { get; set; }
        public string AnalysisName { get; set; } 
    }
}
