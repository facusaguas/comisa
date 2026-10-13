using System.ComponentModel.DataAnnotations;

namespace GestorNomina.Web.Models.ViewModels;

public class UsuarioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede tener más de 100 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(255, ErrorMessage = "El email no puede tener más de 255 caracteres")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres")]
    [Display(Name = "Nombre de Usuario")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un rol")]
    [Display(Name = "Rol")]
    public int RolId { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    // Para edición - opcional
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña (dejar en blanco si no desea cambiarla)")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    [Display(Name = "Confirmar Nueva Contraseña")]
    public string? ConfirmPassword { get; set; }

    // Propiedades de solo lectura para mostrar
    public string? RolNombre { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
