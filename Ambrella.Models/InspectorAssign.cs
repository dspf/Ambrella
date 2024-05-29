using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class InspectorAssign
    {
        [Key]
        public int InspectorAssignId { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string InspectorId { get; set; }
        public IdentityUser Inspector { get; set; }
   
        public AssignmentStatus Status { get; set; }
        public DateTime SubmittedOn { get; set; }
    }
    public enum AssignmentStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
