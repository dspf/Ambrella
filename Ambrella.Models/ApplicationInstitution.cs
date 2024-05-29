using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class ApplicationInstitution
    {
        [Key]
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public Building Building { get; set; }
        public string StudentId { get; set; }
        public IdentityUser Student { get; set; }
        public string InstitutionId { get; set; }
        public IdentityUser Institution { get; set; }
        public LearnerStatus Status { get; set; }
        public DateTime SubmittedOn { get; set; }
    }
    public enum LearnerStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
