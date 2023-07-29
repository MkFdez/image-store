using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ForChartModel
    {
        [Required]
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}
