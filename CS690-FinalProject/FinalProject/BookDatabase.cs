namespace FinalProject;

using Microsoft.Data.Sqlite;
using Spectre.Console;

#pragma warning disable CS8600 // Do not fuss about null string conversions
#pragma warning disable CS8603 // Do not fuss about null reference returns
#pragma warning disable CS8604 // Do not fuss about null int conversions

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
	public bool AddBook(string title = "",
			string author = "",
			string genre = "",
			int rating = -1,
			string state = "",
			string location = ""
			)
	{
		// Assume the book does not get added
		bool added = false;

		// Fail fast and silently if any of our fields are empty

		if ( title == "" )
			return false;

		if ( author == "" )
			return false;

		if ( genre == "" )
			return false;

		if ( rating == -1 )
			return false;

		if ( state == "")
			return false;

		if ( location == "")
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

			command.Parameters.AddWithValue("$title", title);
			command.Parameters.AddWithValue("$author", author);
			command.Parameters.AddWithValue("$genre", genre);
			command.Parameters.AddWithValue("$rating", rating);
			command.Parameters.AddWithValue("$state", state);
			command.Parameters.AddWithValue("$location", location);

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
				Console.WriteLine($"\nThe title {title} is already in the database. Use UpdateBook to change it\n");
			}	

			connection.Close();

			// Show the book we just added
			SearchBook(title);
		}
		return added;
	}

	// Find a book based on its title
	public string SearchBook(string title = "")
	{
		// If no title was provided then silently return empty string
		if ( title == "" )
			return "";

		// Assume there is no book found
		string foundtitle = "";

		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			var table = new Table();
			
			table.AddColumn("Title");
			table.AddColumn("Author");
			table.AddColumn("Genre");
			table.AddColumn("Rating");
			table.AddColumn("State");
			table.AddColumn("Location");

			connection.Open();

			var command = connection.CreateCommand();

			command.CommandText =
				@"
					select * from books where title = $title
				";

			command.Parameters.AddWithValue("$title", title);

			using (var reader = command.ExecuteReader())
			{
				// Only show something on the screen if there are books to print
				if ( reader.HasRows )
				{
					reader.Read();

					foundtitle = reader["Title"].ToString();

					var searchauthor = reader["Author"].ToString();
					var searchgenre = reader["Genre"].ToString();
					var searchrating = reader["Rating"].ToString();
					var searchstate = reader["State"].ToString();
					var searchlocation = reader["Location"].ToString();

					table.AddRow($"{foundtitle}",
							$"{searchauthor}",
							$"{searchgenre}",
							$"{searchrating}",
							$"{searchstate}",
							$"{searchlocation}");
				}
				else
				{
					table = new Table();
					table.AddColumn($"The title \"{title}\" is not in the database");
				}

				AnsiConsole.Write(table);

			}
			connection.Close();
		}
		return foundtitle;
	}

	// Change our book
	public bool UpdateBook(string inputTitle = "",
			string inputAuthor = "",
			string inputGenre = "",
			int inputRating = -1,
			string inputState = "",
			string inputLocation = ""
			)
	{
		// Fail fast and silently if there is no title to update
		if ( inputTitle == "" )
			return false;

		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			var table = new Table();

			connection.Open();

			// Make sure that the title is in the database
			if ( SearchBook(inputTitle) == "")
			{
				return false;
			}

			var command = connection.CreateCommand();

			command.CommandText =
				@"
					select * from books where title = $title
				";

			command.Parameters.AddWithValue("$title", inputTitle);

			// Get the current settings
			using (var reader = command.ExecuteReader())
			{
				// This should always return true
				if ( reader.HasRows )
				{
					reader.Read();

					var searchTitle = reader["Title"].ToString();
					var searchAuthor = reader["Author"].ToString();
					var searchGenre = reader["Genre"].ToString();
					var searchRating = reader["Rating"].ToString();
					var searchState = reader["State"].ToString();
					var searchLocation = reader["Location"].ToString();

					reader.Close();

					// Variables that our book will be set to in the database
					string newTitle;
					string newAuthor;
					string newGenre;
					string newRating;
					string newState;
					string newLocation = "";

					// We do not change our titles
					newTitle = searchTitle;

					//Console.WriteLine("Enter Update Information, press enter to keep current value: ");

					newAuthor = UpdateField("Author",inputAuthor,searchAuthor);

					newGenre = UpdateField("Genre",inputGenre,searchGenre);

					if ( inputRating == -1 )
						newRating = UpdateField("Rating","",searchRating);
					else
						newRating = inputRating.ToString();

					// Assume we will keep the current state
					newState = searchState;

					if ( inputState == "")
					{
						// Ask the user to select a new state

						var keepStateOption = $"Keep Current State : ({searchState})";

						//Console.WriteLine($"State : ({searchState}) : ");
						var newStateSelect  = AnsiConsole.Prompt(
								new SelectionPrompt<string>()
								.Title("\nPlease select state:")
								.AddChoices(new[] {
								keepStateOption,
								"Owns",
								"Wants",
								"Selling",
								"Sold"
								}));
		
						if ( newStateSelect != keepStateOption )
							newState = newStateSelect;
					}

					if ( inputLocation == "" )
					{
						// If we have sold our book then its location can only be the store
						if ( newState == "Sold" )
						{
							newLocation = "Store";
						}
						else
							newLocation = UpdateField("Location",inputLocation,searchLocation);
					}

					var updatecommand = connection.CreateCommand();

					updatecommand.CommandText =
						@"
							REPLACE INTO books(title,author,genre,rating,location,state)
							values ($title,$author,$genre,$rating,$location,$state)
						";

					updatecommand.Parameters.AddWithValue("$title", newTitle);
					updatecommand.Parameters.AddWithValue("$author", newAuthor);
					updatecommand.Parameters.AddWithValue("$genre", newGenre);
					updatecommand.Parameters.AddWithValue("$rating", newRating);
					updatecommand.Parameters.AddWithValue("$state", newState);
					updatecommand.Parameters.AddWithValue("$location", newLocation);

					// If our query has not changed the title, then fail out
					if ( updatecommand.ExecuteNonQuery() != 1 )
						return false;

					connection.Close();

					table = new Table();
					
					table.AddColumn("Title");
					table.AddColumn("Author");
					table.AddColumn("Genre");
					table.AddColumn("Rating");
					table.AddColumn("State");
					table.AddColumn("Location");

					table.AddRow($"{newTitle}",
							$"{newAuthor}",
							$"{newGenre}",
							$"{newRating}",
							$"{newState}",
							$"{newLocation}");
					AnsiConsole.Write(table);
				}
			}
		}
		return true;
	}

	public int RemoveBook(string title = "")
	{
		// If no title provided, then return no books have been removed
		if ( title == "" )
			return 0;

		// Assume no books will be removed
		int numberRemoved = 0;
		
		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			// If the title isn't in the database, then removing it means nothing
			if ( SearchBook(title) == "")
			{
				return numberRemoved;
			}

			var command = connection.CreateCommand();

			command.CommandText =
				@"
					delete from books where title = $title
				";

			command.Parameters.AddWithValue("$title", title);

			numberRemoved = command.ExecuteNonQuery();
			connection.Close();

			// Tell the user the book has been removed
			Console.WriteLine("Status: Removed\n");
		}

		return numberRemoved;
	}

	public int ReportBooks(string searchfield = "", string searchvalue = "")
	{
		int resultCount = 0;

		// If our search or value fields are empty then no books will be found
		if ( searchfield == "" || searchvalue == "" )
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

	// Prompt user to enter value for field
	// If user enters "" then return the existing field
	// otherwise return the updated field
	public static string UpdateField(string message, string inputfield, string field)
	{
		if ( inputfield != "")
			return inputfield;

		Console.WriteLine("\nEnter Update Information, press enter to keep current value: ");
		Console.Write($"{message} : ({field}) : ");
		var answer = Console.ReadLine(); 

		if ( answer != "" )
			field = answer;

		return field;
	}

}
