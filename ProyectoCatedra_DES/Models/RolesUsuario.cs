using System.ComponentModel.DataAnnotations;

namespace ProyectoCatedra_DES.Models
{
    public class RolesUsuario
    {
        [Key]
        public int RolId { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoRol { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
    }
}
