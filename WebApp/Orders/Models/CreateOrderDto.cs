using System.ComponentModel.DataAnnotations;

namespace WebApp.Orders.Models;

public class OrderCreateDto
{
    [Required]
    [StringLength(256, MinimumLength = 3)]
    public string ProductName { get; set; } = string.Empty;
}
