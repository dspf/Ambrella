using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class SendReportCompanyInspector
    {
        [Key]
        public int SendReportCompanyInspectorId { get; set; }
        public int CompanyInspectionReportId { get; set; }
        public CompanyInspectionReport CompanyInspectionReport { get; set; }
        public string InspectorId { get; set; }
        public IdentityUser Inspector { get; set; }
        public string AdminId { get; set; }
        public IdentityUser Admin { get; set; }
        public string? LandlordId { get; set; }
        public IdentityUser Landlord { get; set; }
        public SendReportStatus Status { get; set; }
        public DateTime SubmittedOn { get; set; }
    }

    public enum SendReportStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
