using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class InstitutionAssign
    {
        [Key]
        public int InstitutionAssignId { get; set; }

        [ForeignKey("Building")]
        public int BuildingId { get; set; } // Add this property
        public Building Building { get; set; }
        public string? InspectorId { get; set; }
        public IdentityUser? Inspector { get; set; }
        public InstitutionAssignmentStatus Status { get; set; }
        public DateTime SubmittedOn { get; set; }

    }
    public enum InstitutionAssignmentStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
