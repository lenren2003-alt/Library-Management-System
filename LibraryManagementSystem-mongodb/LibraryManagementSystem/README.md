# Library Management System

Beginner-friendly ASP.NET Core MVC application using C# and MongoDB.

## Requirements
- Visual Studio 2022/2026 with ASP.NET and web development installed
- .NET 10 SDK
- A MongoDB database (e.g. a free MongoDB Atlas cluster)

## Setup
1. Open `LibraryManagementSystem.csproj` in Visual Studio and restore NuGet packages.
2. Set your real MongoDB connection string (do NOT put your real password in
   `appsettings.json` if this project is ever pushed to source control). From
   a terminal in the project folder, run:
   ```
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:MongoDb" "mongodb+srv://admin:YOUR_REAL_PASSWORD@cluster0.txa3qyi.mongodb.net/?appName=Cluster0"
   ```
   This overrides the placeholder value in `appsettings.json` only on your
   own machine.
3. Press `Ctrl + F5` or click Run.
4. MongoDB creates the database and collections (`Books`, `Members`, `Loans`,
   `Counters`) automatically the first time data is saved.
5. Start by adding a member and a book.
6. Go to Loans and borrow the book.

## Main concepts to learn
- Models = data
- Controllers = application logic
- Views = HTML pages
- LibraryContext = connection between C# and MongoDB (wraps IMongoCollection<T>)
- MongoDB.Driver = database operations
- MongoDB = document database (no schema, no server-side joins between collections)

## Loan rule
A book is due 14 days after it is borrowed. An active loan after 14 days is shown as OVERDUE.
