using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrella.Models
{
    public class BuildingInspectionReport
    {
        [Key]
        public int BuildingInspectionReportId { get; set; }
        public string BuildingName { get; set; }
        public string BuildingAddress { get; set; }
        public string InspectorId { get; set; }
        public IdentityUser Inspector { get; set; }
        public string BuildingLocation { get; set; }
        public int NumberOfRooms { get; set; }
        public bool IsBuildingVerified { get; set; }
        public string InspectorNotes { get; set; }
        public DateTime InspectionDate { get; set; }

        [DataType(DataType.ImageUrl)]
        public string RoomImage { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ExteriorImage { get; set; }

        [DataType(DataType.ImageUrl)]
        public string StudyImage { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Kitchen { get; set; }
    }
}
