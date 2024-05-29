using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace Ambrella.Models
{
    public class Notification
    {
        
            [Key]
            public int Id { get; set; }
            public string Message { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? LandlordId { get; set; }
            public ApplicationUser Landlord { get; set; }
        
    }
}
