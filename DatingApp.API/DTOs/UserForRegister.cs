using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForRegister
    {
        [Required]
        public string  Username { get; set; }


        [Required]
        [StringLength(80, MinimumLength= 6, ErrorMessage="Password must be greater than 6 characters")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]

        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public UserForRegister()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}