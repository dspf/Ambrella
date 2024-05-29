using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class Contract
    {
        [Key]
        public int ContractId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public byte[]? FileContent { get; set; }

        public string? FileName { get; set; }

        public string? FileContentType { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? InstitutionId { get; set; }

        public IdentityUser Institution { get; set; }

        public string? LandlordId { get; set; }

        public IdentityUser Landlord { get; set; }

        public ContractStatus Status { get; set; }
    }

    public enum ContractStatus
    {
        Draft,
        Sent,
        Received,
        Accepted,
        Rejected
    }
}
