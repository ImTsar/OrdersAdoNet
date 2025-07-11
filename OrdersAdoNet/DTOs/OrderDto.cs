using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersAdoNet.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int AnalysisId { get; set; }
        public string AnalysisName { get; set; }
    }
}
