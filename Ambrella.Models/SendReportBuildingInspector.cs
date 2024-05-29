using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class SendReportBuildingInspector
    {
        [Key]
        public int SendReportBuildingInspectorId { get; set; }
        public int BuildingInspectionReportId { get; set; }
        public BuildingInspectionReport BuildingInspectionReport { get; set; }
        public string InspectorId { get; set; }
        public IdentityUser Inspector { get; set; }
        public string InstitutionId { get; set; }
        public IdentityUser Institution { get; set; }
        public string? LandlordId { get; set; }
        public IdentityUser Landlord { get; set; }
        public BuildingReportStatus Status { get; set; }
        public DateTime SubmittedOn { get; set; }
    }

    public enum BuildingReportStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
