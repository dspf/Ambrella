using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class ApplicationBuilding
    {
        [Key]
        public int ApplicationBuildingId { get; set; }
        public int BuildingId { get; set; }
        public Building Building { get; set; }
        public string LandlordId { get; set; }
        public IdentityUser Landlord { get; set; }
       
        public string InstitutionId { get; set; }
        public IdentityUser Institution { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime SubmittedOn { get; set; }

        public DateTime ScheduledDate { get; set; }
    }
    public enum ApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }
   
}
