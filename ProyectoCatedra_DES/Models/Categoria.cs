using System.ComponentModel.DataAnnotations;

namespace ProyectoCatedra_DES.Models
{
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        
        public ICollection<Producto> Productos { get; set; }
    }
}
