using System;
using System.Collections.Generic;
namespace OrdersEfCore.Models
{
    public class Analysis
    {
        public int Id { get; set; }       
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int GroupId { get; set; }

        public Group Group { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
