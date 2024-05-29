using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class ApplicationCompany
    {
        [Key]
        public int ApplicationCompanyId { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string LandlordId { get; set; }
        public ApplicationUser Landlord { get; set; }
       
        public string AdminId { get; set; }
        public IdentityUser Admin { get; set; }
        public ApplicationStatuss Status { get; set; }
        public DateTime SubmittedOn { get; set; }
    }
    public enum ApplicationStatuss
    {
        Pending,
        Approved,
        Rejected
    }
}
