using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int NumberOfProperties { get; set; }
        public string LandlordId { get; set; }
        public IdentityUser Landlord { get; set; }

        public string Location { get; set; }
        public string Address { get; set; }
        [DataType(DataType.ImageUrl)]
        public string? HeadQuarterImg { get; set; }

      
    }
}
