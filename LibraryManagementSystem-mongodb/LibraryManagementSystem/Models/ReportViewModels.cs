namespace LibraryManagementSystem.Models
{
    // Shown on the Overdue report — one row per late loan.
    public class OverdueLoanViewModel
    {
        public int LoanId { get; set; }
        public string BookTitle { get; set; } = "";
        public string MemberName { get; set; } = "";
        public string MemberEmail { get; set; } = "";
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }

        public int DaysOverdue => (DateTime.Today - DueDate).Days;
    }

    // Shown on the Popular Books report.
    public class PopularBookViewModel
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int BorrowCount { get; set; }
    }

    // Shown on the Frequent Borrowers report.
    public class FrequentBorrowerViewModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public int BorrowCount { get; set; }
    }
}
