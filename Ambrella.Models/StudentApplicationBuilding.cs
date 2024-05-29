using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class StudentApplicationBuilding
    {
        [Key]
        public int StudentApplicationBuildingId { get; set; }
        public int BuildingId { get; set; }
        public Building Building { get; set; }
        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }
        public string LandlordId { get; set; }
        public ApplicationUser Landlord { get; set; }
        public DateTime ApplicationDate { get; set; }
        public StudentStatus Status { get; set; }
    }

    public enum StudentStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
