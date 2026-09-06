using LibraryManagementSystem.Models;
using MongoDB.Driver;

namespace LibraryManagementSystem.Data
{
    // This class connects our C# models to MongoDB collections.
    // It replaces the old EF Core DbContext.
    public class LibraryContext
    {
        public IMongoCollection<Book> Books { get; }
        public IMongoCollection<Member> Members { get; }
        public IMongoCollection<Loan> Loans { get; }

        // Used to generate simple auto-incrementing integer Ids,
        // since MongoDB doesn't provide these automatically like SQL does.
        private readonly IMongoCollection<Counter> _counters;

        public LibraryContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            var databaseName = configuration["MongoDbSettings:DatabaseName"] ?? "LibraryManagementSystem";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            Books = database.GetCollection<Book>("Books");
            Members = database.GetCollection<Member>("Members");
            Loans = database.GetCollection<Loan>("Loans");
            _counters = database.GetCollection<Counter>("Counters");
        }

        // Atomically increments and returns the next Id for the given collection name
        // (e.g. "Books", "Members", "Loans"). Mimics SQL identity columns.
        public int GetNextId(string collectionName)
        {
            var filter = Builders<Counter>.Filter.Eq(c => c.Id, collectionName);
            var update = Builders<Counter>.Update.Inc(c => c.Sequence, 1);
            var options = new FindOneAndUpdateOptions<Counter>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var counter = _counters.FindOneAndUpdate(filter, update, options);
            return counter.Sequence;
        }
    }

    // Backing document for the auto-increment counters described above.
    public class Counter
    {
        public string Id { get; set; } = "";
        public int Sequence { get; set; }
    }
}
