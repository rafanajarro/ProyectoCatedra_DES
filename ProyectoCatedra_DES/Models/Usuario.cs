﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCatedra_DES.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string CorreoElectronico { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        public bool Activo { get; set; }

        [ForeignKey("RolesUsuario")]
        public int RolId { get; set; }
        public RolesUsuario RolUsuario { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? FechaCreacion { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UltimoAcceso { get; set; }
    }
}
