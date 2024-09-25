using System.ComponentModel.DataAnnotations;

namespace ProyectoCatedra_DES.Models
{
    public class RegisterDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string CorreoElectronico { get; set; }

        [Required]
        [StringLength(8)]
        [Phone]
        public string Telefono { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(9)]
        public string Dui { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required]
        public string Rol { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? FechaNacimiento { get; set; }
    }
}
