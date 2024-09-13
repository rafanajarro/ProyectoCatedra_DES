using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ProyectoCatedra_DES.Models
{
    public class Producto
    {
        [Key]
        [StringLength(5)]
        public string  CodigoProucto { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreProducto { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public int Stock { get; set; }

        [ForeignKey("Categoria")]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}
