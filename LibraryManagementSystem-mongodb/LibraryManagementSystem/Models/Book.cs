using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [BsonId]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Author { get; set; } = "";

        [Required]
        public string ISBN { get; set; } = "";

        // True means the book can currently be borrowed.
        public bool IsAvailable { get; set; } = true;
    }
}
