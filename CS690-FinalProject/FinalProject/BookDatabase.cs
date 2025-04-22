namespace FinalProject;

using Microsoft.Data.Sqlite;
using Spectre.Console;

public class BookDatabase
{
	public BookDatabase()
	{
		// If a book database does not exist, create a new database with sample entries
		if ( ! File.Exists("books.db"))
		{
			using (var connection = new SqliteConnection("Data Source=books.db"))
			{
				connection.Open();

				var command = connection.CreateCommand();

				command.CommandText =
					@"
						create table books
						(
							title text not null,
							author text not null,
							genre text  not null,
							rating integer not null,
							state text not null,
							location text not null,
							primary key(title)
						);

						replace into books values('Alice in Wonderland','Lewis Carrol','Fantasy','5','Owns','Shelf');
						replace into books values('Treasure Island','Robert Lewis Stevenson','Adventure','5','Owns','Shelf');
						replace into books values('Dracula','Bram Stoker','Horror','5','Selling','Shelf');
						replace into books values('Moby Dick','Herman Melville','Drama','1','Owns','Shelf');
						replace into books values('Oliver Twist','Charles Dickens','Drama','4','Wants','Store');
					";
				command.ExecuteNonQuery();
				connection.Close();
			}
		}
	}

	// Add a book to the database
	public bool AddBook(Book newBook, bool display=false)
	{
		// Assume the book does not get added
		bool added = false;

		// Fail fast and silently if any of our fields are empty
		if ( String.IsNullOrEmpty(newBook.Title))
			return false;

		if ( String.IsNullOrEmpty(newBook.Author))
			return false;

		if ( String.IsNullOrEmpty(newBook.Genre))
			return false;

		if ( newBook.Rating == -1 )
			return false;

		if ( String.IsNullOrEmpty(newBook.State))
			return false;

		if ( String.IsNullOrEmpty(newBook.Location))
			return false;

		// Connect to the database and attempt to add the book
		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			var command = connection.CreateCommand();

			command.CommandText = 
				@"
					INSERT INTO books(title,author,genre,rating,location,state)
					values ($title,$author,$genre,$rating,$location,$state)
				";

			command.Parameters.AddWithValue("$title", newBook.Title);
			command.Parameters.AddWithValue("$author", newBook.Author);
			command.Parameters.AddWithValue("$genre", newBook.Genre);
			command.Parameters.AddWithValue("$rating", newBook.Rating);
			command.Parameters.AddWithValue("$state", newBook.State);
			command.Parameters.AddWithValue("$location", newBook.Location);

			// We cannot add the same title twice
			try
			{
				added = true;
				command.ExecuteNonQuery();
			}
			catch ( Exception e)
			{
				added = false;
				Console.WriteLine(e.Message);
				Console.WriteLine($"The title {newBook.Title} is already in the database. Use UpdateBook to change it\n");
			}	

			connection.Close();

			// Show the book we just added if display == true
			SearchBook(newBook.Title, display);
		}
		return added;
	}

	// Find a book based on its title
	public Book SearchBook(string title = "", bool display=false)
	{
		// Create empty book object that we'll fill with the book details we find
		Book searchedBook = new Book();

		// If no title was provided then silently return our empty book since there are no results
		if ( String.IsNullOrEmpty(title))
			return searchedBook;

		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			var table = new Table();

			var command = connection.CreateCommand();

			command.CommandText =
				@"
					select * from books where title = $title
				";

			command.Parameters.AddWithValue("$title", title);

			using (var reader = command.ExecuteReader())
			{
				// Are there any books in the result set?
				if ( reader.HasRows )
				{
					reader.Read();
					
					table.AddColumn("Title");
					table.AddColumn("Author");
					table.AddColumn("Genre");
					table.AddColumn("Rating");
					table.AddColumn("State");
					table.AddColumn("Location");

					searchedBook.Title = reader["Title"].ToString() ?? "";
					searchedBook.Author = reader["Author"].ToString() ?? "";
					searchedBook.Genre  = reader["Genre"].ToString() ?? "";
					searchedBook.Rating = int.Parse(reader["Rating"].ToString() ?? "");
					searchedBook.State  = reader["State"].ToString() ?? "";
					searchedBook.Location = reader["Location"].ToString() ?? "";

					table.AddRow($"{searchedBook.Title}",
							$"{searchedBook.Author}",
							$"{searchedBook.Genre}",
							$"{searchedBook.Rating}",
							$"{searchedBook.State}",
							$"{searchedBook.Location}");
				}
				else
				{
					table.AddColumn($"The title \"{title}\" is not in the database");
				}

				// Only display our table if display == true
				if ( display )
					AnsiConsole.Write(table);

			}
			connection.Close();
		}
		return searchedBook;
	}

	// Change our book
	public bool UpdateBook(Book inputBook, bool display=false)
	{
		// Fail fast and silently if there is no title to update
		if ( String.IsNullOrEmpty(inputBook.Title))
			return false;

		// Make sure the title is in the database
		if ( String.IsNullOrEmpty(SearchBook(inputBook.Title,false).ToString()))
			return false;

		// Update the database 
		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			var updatecommand = connection.CreateCommand();

			updatecommand.CommandText =
				@"
					REPLACE INTO books(title,author,genre,rating,location,state)
					values ($title,$author,$genre,$rating,$location,$state)
				";

			updatecommand.Parameters.AddWithValue("$title", inputBook.Title);
			updatecommand.Parameters.AddWithValue("$author", inputBook.Author);
			updatecommand.Parameters.AddWithValue("$genre", inputBook.Genre);
			updatecommand.Parameters.AddWithValue("$rating", inputBook.Rating);
			updatecommand.Parameters.AddWithValue("$state", inputBook.State);
			updatecommand.Parameters.AddWithValue("$location", inputBook.Location);

			// If our query has not changed anything, then fail out
			if ( updatecommand.ExecuteNonQuery() != 1 )
				return false;

			connection.Close();

			if ( display )
				SearchBook(inputBook.Title,display);
		}
		return true;
	}

	public int RemoveBook(string title = "", bool display = false)
	{
		// If no title provided, then return no books have been removed
		if ( String.IsNullOrEmpty(title))
			return 0;

		// Assume no books will be removed
		int numberRemoved = 0;
		
		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			// If the title isn't in the database, then removing it means nothing
			if ( SearchBook(title,display).ToString() == "")
				return numberRemoved;

			var command = connection.CreateCommand();

			command.CommandText =
				@"
					delete from books where title = $title
				";

			command.Parameters.AddWithValue("$title", title);

			numberRemoved = command.ExecuteNonQuery();
			connection.Close();

			// Tell the user the book has been removed
			if ( display )
				Console.WriteLine("Status: Removed\n");
		}

		return numberRemoved;
	}

	public int ReportBooks(string searchfield = "", string searchvalue = "")
	{
		int resultCount = 0;

		// If our search or value fields are empty then no books will be found
		if ( String.IsNullOrEmpty(searchfield) || String.IsNullOrEmpty(searchvalue))
			return resultCount;

		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			var command = connection.CreateCommand();

			if ( searchfield == "All" && searchvalue == "All" )
			{
				command.CommandText = $"select * from books";
			}
			else
				command.CommandText = $"select * from books where {searchfield} = $searchvalue";

			command.Parameters.AddWithValue("$searchvalue", searchvalue);

			var table = new Table();
			
			table.AddColumn("Title");
			table.AddColumn("Author");
			table.AddColumn("Genre");
			table.AddColumn("Rating");
			table.AddColumn("State");
			table.AddColumn("Location");

			using (var reader = command.ExecuteReader())
			{
				// Only show something on the screen if there are books to print
				if ( reader.HasRows )
				{
					while (reader.Read())
					{
						resultCount++;

						var searchtitle = reader["Title"].ToString();
						var searchauthor = reader["Author"].ToString();
						var searchgenre = reader["Genre"].ToString();
						var searchrating = reader["Rating"].ToString();
						var searchstate = reader["State"].ToString();
						var searchlocation = reader["Location"].ToString();

						table.AddRow($"{searchtitle}",
								$"{searchauthor}",
								$"{searchgenre}",
								$"{searchrating}",
								$"{searchstate}",
								$"{searchlocation}");
					}
				}
				else
				{
					table = new Table();
					table.AddColumn("This report has no entries");
				}
			}
			AnsiConsole.Write(table);
			Console.WriteLine($"There are {resultCount} books in this report");
			Console.WriteLine("\nTo print report: Copy and paste the above report into a text editor, then press CTRL-P\n");
			connection.Close();
		}
		return resultCount;
	}

	public int BookCount()
	{
		// Assume there are no books in the database
		int bookCount = 0;

		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			var command = connection.CreateCommand();

			command.CommandText =
				@"
					select count(*) from books
				";

			using (var reader = command.ExecuteReader())
			{
				if ( reader.Read())
					bookCount = int.Parse(reader.GetString(0));
			}
		}
		return bookCount;
	}
}
