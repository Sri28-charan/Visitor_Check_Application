using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Visitor_Check_Application.Models;

[Index("PatientName", Name = "UQ__Patients__2AC4463402CC7745", IsUnique = true)]
[Index("Room", Name = "UQ__Patients__DA155985CB328957", IsUnique = true)]
public partial class Patient
{
    [Key]
    [Column("Patient_id")]
    [StringLength(50)]
    [Unicode(false)]
    public string PatientId { get; set; } = null!;

    [Column("Patient_name")]
    [StringLength(20)]
    [Unicode(false)]
    public string? PatientName { get; set; }

    [Column("Floor_number")]
    [StringLength(20)]
    [Unicode(false)]
    public string FloorNumber { get; set; } = string.Empty;

    [Column("Room")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Room { get; set; }

    public int? Bed { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string Isolation { get; set; } = null!;

    [Column("Visitors_allowed")]
    public bool VisitorsAllowed { get; set; }

    [Column("Visitors_inRoom")]
    public int? VisitorsInRoom { get; set; }

    [Column("Facility_Code")]
    public string FacilityCode { get; set; } = null!;

    [ForeignKey(nameof(FacilityCode))]
    public virtual Hospital Hospital { get; set; } = null!;

	public DateTime? VisitorStartTime { get; set; }

}
