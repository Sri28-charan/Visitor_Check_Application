using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Visitor_Check_Application.Models;

[Table("Hospital")]
public partial class Hospital
{
    
    [Column("Hospital_Name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? HospitalName { get; set; }

    [Key]
    [Column("Facility_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string FacilityCode { get; set; } = null!;

    [Column("Max_Limit")]
    public int? MaxLimit { get; set; }

    [Column("Division_Id")]
    [StringLength(1)]
    [Unicode(false)]
    public string? DivisionId { get; set; }

    [ForeignKey("DivisionId")]
    [InverseProperty("Hospitals")]
    public virtual Division? Division { get; set; }


	[Column("Visitors_Allowed")]
	public bool? VisitorsAllowed { get; set; }


	public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
