using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Ambrella.Models
{
    public class Building
    {
        [Key]
        public int BuildingId { get; set; }
        [Required]
        public string Name { get; set; }
        
        public string LandlordId { get; set; }
        public IdentityUser Landlord { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public int Capacity { get; set; }
        public decimal Cost { get; set; }
        [DataType(DataType.ImageUrl)]
        public string RoomImage { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ExteriorImage { get; set; }

        [DataType(DataType.ImageUrl)]
        public string StudyImage { get; set; }

        [DataType(DataType.ImageUrl)]
        public string KitchenImage { get; set; }
    }
}
