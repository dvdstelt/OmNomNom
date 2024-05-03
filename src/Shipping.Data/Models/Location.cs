using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.Data.Models
{
    public class Location
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Town { get; set; }
        public string ZipCode { get; set; }
    }
}
