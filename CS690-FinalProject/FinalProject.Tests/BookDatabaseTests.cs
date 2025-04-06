namespace FinalProject.Tests;

using FinalProject;

public class BookDatabaseTests
{
	BookDatabase bookDatabase;

	public BookDatabaseTests()
	{
		bookDatabase = new BookDatabase();
	}

	[Fact]
	public void AddBookTest()
	{
		// Get the current book count
		int currentCount = bookDatabase.BookCount();

		// Add an imaginary book to the database
		bookDatabase.AddBook("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf");

		// Check that our count has incremented by one
		Assert.Equal(currentCount + 1,bookDatabase.BookCount());

		// Get rid of our fake entry so it does not cause problems
		bookDatabase.RemoveBook("Fakebook");
	}
}
