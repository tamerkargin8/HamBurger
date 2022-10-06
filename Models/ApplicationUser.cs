using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HamBurger.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Custome olarak IdentityUser üzerine veriler ekledik
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Adress { get; set; }
        public string Semt { get; set; }
        [Required]
        public string City { get; set; }
        [NotMapped] //Database veya ORM(Object Relational Mapping) yani objeyi veritabanı köprü özelliğini iptal ettik
        public string Role { get; set; }


    }
}
