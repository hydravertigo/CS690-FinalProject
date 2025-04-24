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
		// We should get a true response when adding a new book to the database
		Assert.True(bookDatabase.AddBook(new Book("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf")));
		
		// Clean up after ourselves
		bookDatabase.RemoveBook("Fakebook");
	}

	[Fact]
	public void AddBookCountTest()
	{
		// Get the current book count
		int currentCount = bookDatabase.BookCount();

		// Add an imaginary book to the database
		bookDatabase.AddBook(new Book("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf"));

		// Check that our count has incremented by one
		Assert.Equal(currentCount + 1,bookDatabase.BookCount());

		// Get rid of our fake entry so it does not cause problems
		bookDatabase.RemoveBook("Fakebook");
	}

	[Fact]
	public void AddBookTwiceTest()
	{
		// We should get a false response when adding a book that is already present
		Assert.True(bookDatabase.AddBook(new Book("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf")));
		Assert.False(bookDatabase.AddBook(new Book("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf")));

		// Clean up after ourselves
		bookDatabase.RemoveBook("Fakebook");
	}

	[Fact]
	public void AddEmptyBookTest()
	{
		// We should get a false response when adding an empty book
		Assert.False(bookDatabase.AddBook(new Book()));
	}

	[Fact]
	public void SearchEmptyBookTest()
	{
		// We should get an empty string back when we search for an empty title string
		Assert.Equal("",bookDatabase.SearchBook("",false).ToString());
	}

	[Fact]
	public void SearchMissingBookTest()
	{
		// We should get an empty string back when we search for a book that does not exist
		Assert.Equal("",bookDatabase.SearchBook("_This_book_does_not_exist_fake_title").ToString());
	}

	[Fact]
	public void RemoveBookTest()
	{
		// Add an imaginary book to the database
		bookDatabase.AddBook(new Book("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf"));

		// Verify that the book is there
		Assert.Equal("Fakebook",bookDatabase.SearchBook("Fakebook",false).ToString());

		// Remove book, only 1 title should be removed
		Assert.Equal(1,bookDatabase.RemoveBook("Fakebook"));

		// Verify that the book is absent
		Assert.Equal("",bookDatabase.SearchBook("Fakebook").ToString());
	}

	[Fact]
	public void RemoveEmptyBookTest()
	{
		// We should get a 0 response when trying to remove a non-existent book
		Assert.Equal(0,bookDatabase.RemoveBook(""));
	}

	[Fact]
	public void RemoveMissingBookTest()
	{
		// We should get a false response back when we attempt to remove a book that does not exist
		Assert.Equal(0,bookDatabase.RemoveBook("_This_book_does_not_exist_fake_title"));
	}

	[Fact]
	public void EmptyReportTest()
	{
		// An empty report should return zero books
		Assert.Equal(0,bookDatabase.ReportBooks("",""));
	}

	[Fact]
	public void MissingReportTest()
	{
		// A report that doesn't have any entries should return 0 books
		Assert.Equal(0,bookDatabase.ReportBooks("Author","_non_existent_author"));
	}

	// A report of all books should return the same count as the BookCount function
	[Fact]
	public void AllBooksReportTest()
	{
		// Get the current book count
		int currentCount = bookDatabase.BookCount();

		Assert.Equal(currentCount,bookDatabase.ReportBooks("All","All"));
	}

	[Fact]
	public void UpdateBookTest()
	{
		// Create an imaginary book
		Book fakeBook = new Book("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf");

		// Add the imaginary book to the database
		bookDatabase.AddBook(fakeBook);

		// Verify that the book is there
		Assert.Equal("Fakebook",bookDatabase.SearchBook("Fakebook",false).ToString());

		// Change the author in our book object
		fakeBook.Author="OtherAuthor";

		// Update the book in the database, the book should get updated
		Assert.True(bookDatabase.UpdateBook(fakeBook));

		// Remove book, only 1 title should be removed
		Assert.Equal(1,bookDatabase.RemoveBook("Fakebook"));

		// Verify that the book is absent
		Assert.Equal("",bookDatabase.SearchBook("Fakebook",false).ToString());
	}

	// An missing book should not get updated
	[Fact]
	public void UpdateEmptyBookTest()
	{
		// Create an imaginary book
		Book fakeBook = new Book("Fakebook","Fakeauthor","FakeGenre",5,"Owns","Shelf");

		// Because this book has not been added to the database, it cannot be updated
		Assert.False(bookDatabase.UpdateBook(fakeBook));
	}

}
