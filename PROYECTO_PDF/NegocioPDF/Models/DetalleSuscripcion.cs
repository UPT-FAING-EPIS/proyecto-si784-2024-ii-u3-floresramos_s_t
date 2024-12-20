// NegocioPDF/Models/DetalleSuscripcion.cs
using System;

namespace NegocioPDF.Models
{
    public class DetalleSuscripcion
    {
 public int Id { get; set; }
    public required string tipo_suscripcion { get; set; }
    public DateTime? fecha_inicio { get; set; }
    public DateTime? fecha_final { get; set; }
    public decimal? precio { get; set; }
    public int operaciones_realizadas { get; set; }
    public int UsuarioId { get; set; }
    public virtual Usuario? Usuario { get; set; }
    }
}
