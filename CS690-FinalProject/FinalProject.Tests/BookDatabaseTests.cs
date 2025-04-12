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
	public void AddBookCountTest()
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

	[Fact]
	public void AddBookTest()
	{
		// We should get a true response when adding a new book to the database
		Assert.True(bookDatabase.AddBook("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf"));
		bookDatabase.RemoveBook("Fakebook");
	}

	[Fact]
	public void AddEmptyBookTest()
	{
		// We should get a false response when adding an empty book
		Console.WriteLine("Adding Empty book");
		Assert.False(bookDatabase.AddBook());
	}

	[Fact]
	public void AddBookTwiceTest()
	{
		// We should get a false response when adding a book that is already present
		Assert.True(bookDatabase.AddBook("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf"));
		Assert.False(bookDatabase.AddBook("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf"));
		bookDatabase.RemoveBook("Fakebook");
	}

	[Fact]
	public void RemoveBookTest()
	{
		// Add an imaginary book to the database
		bookDatabase.AddBook("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf");

		// Verify that the book is there
		Assert.Equal("Fakebook",bookDatabase.SearchBook("Fakebook"));

		// Remove the book from the database
		bookDatabase.RemoveBook("Fakebook");

		// Verify that the book is absent
		Assert.Equal("",bookDatabase.SearchBook("Fakebook"));
	}

}
