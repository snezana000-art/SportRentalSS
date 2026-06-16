using System;
using System.ComponentModel.DataAnnotations;

namespace GradeJournal.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "User";

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}