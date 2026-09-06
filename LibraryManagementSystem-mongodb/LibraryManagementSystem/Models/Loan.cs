using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        [BsonId]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        public DateTime LoanDate { get; set; } = DateTime.Today;

        // This stays null until the book is returned.
        public DateTime? ReturnDate { get; set; }

        // Books and Members live in their own MongoDB collections (Loan only
        // stores their Ids). These properties are filled in manually by the
        // controller after fetching, purely so the Views can display
        // book/member details - they are never saved to the Loans collection.
        [BsonIgnore]
        public Book? Book { get; set; }

        [BsonIgnore]
        public Member? Member { get; set; }
    }
}
