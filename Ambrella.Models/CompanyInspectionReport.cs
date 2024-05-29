using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class CompanyInspectionReport
    {
        [Key]
        public int CompanyInspectionReportId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string InspectorId { get; set; }
        public IdentityUser Inspector { get; set; }
        public string CompanyLocation { get; set; }
        public int NumberOfBuildings { get; set; }
        public bool IsCompanyVerified { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string InspectorNotes { get; set; }
        public DateTime InspectionDate { get; set; }

        [DataType(DataType.ImageUrl)]
        public string FirstBuilding { get; set; }

        [DataType(DataType.ImageUrl)]
        public string SecondBuilding { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ThirdBuilding { get; set; }

        [DataType(DataType.ImageUrl)]
        public string FourthBuilding { get; set; }
    }
}
