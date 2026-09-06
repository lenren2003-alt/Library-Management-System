using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagementSystem.Models
{
    public class Member
    {
        [BsonId]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";
    }
}
