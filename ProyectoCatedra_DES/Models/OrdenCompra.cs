using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCatedra_DES.Models
{
    public class OrdenCompra
    {
        [Key]
        public int OrdenCompraId { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; }

        [Required]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        [Required]
        public int CantidadCompra { get; set; }

        [Required]
        public decimal TotalOrdenCompra { get; set; }
    }
}
