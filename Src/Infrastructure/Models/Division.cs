using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Visitor_Check_Application.Models;

public partial class Division
{
    [Key]
    [Column("Division_Id")]
    [StringLength(1)]
    [Unicode(false)]
    public string DivisionId { get; set; } = null!;

    [Column("Division_Name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? DivisionName { get; set; }

    

	[JsonIgnore] 
	public virtual ICollection<Hospital> Hospitals { get; set; } = new List<Hospital>();

}
