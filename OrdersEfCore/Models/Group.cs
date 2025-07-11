namespace OrdersEfCore.Models
{
    public class Group
    {
        public int Id { get; set; } 
        public string Name { get; set; }      
        public string StorageTemp{ get; set; }     

        public ICollection<Analysis> Analyses { get; set; }
    }
}
