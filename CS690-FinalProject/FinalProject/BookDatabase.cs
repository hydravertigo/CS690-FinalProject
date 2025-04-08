namespace FinalProject;

using Microsoft.Data.Sqlite;
using Spectre.Console;

#pragma warning disable CS8604 // Do not fuss about null int conversions
#pragma warning disable CS8600 // Do not fuss about null string conversions

public class BookDatabase
{
	public BookDatabase()
	{
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
					";
				command.ExecuteNonQuery();
				//connection.Close();
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

		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			if ( title == "" )
			{
				Console.Write("Title: ");
				title = Console.ReadLine();
			}

			if ( author == "" )
			{
				Console.Write("Author: ");
				author = Console.ReadLine();
			}

			if ( genre == "" )
			{
				Console.Write("Genre: ");
				genre = Console.ReadLine();
			}

			if ( rating == -1 )
			{
				Console.Write("Rating (1-5): ");
				rating = int.Parse(Console.ReadLine());
			}

			// Correct our rating range
			if ( rating < 1 )
				rating = 1;
			else if ( rating > 5 )
				rating = 5;

			if ( state == "" )
			{
				state = AnsiConsole.Prompt(
						new SelectionPrompt<string>()
						.Title("Please Choose State Option:")
						.AddChoices(new[] {
						"Owns",
						"Wants",
						"Selling",
						"Sold"
						}));
			}

			// If the state is "Wants" then the location must be the store
			if ( state == "Wants" )
			{
				location = "Store";
			}
			else if ( location == "")
			{
				Console.Write("Location: ");
				location = Console.ReadLine();
			}

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

			SearchBook(title);
		}
		return added;
	}

	// Find a book based on its title
	public string SearchBook(string title = "")
	{
		// Assume there is no book found
		string foundbook = "";

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

			if ( title == "" )
			{
				Console.Write("Title: ");
				title = Console.ReadLine();
			}

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

					foundbook = reader.GetString(0);

					var searchauthor = reader.GetString(1);
					var searchgenre = reader.GetString(2);
					var searchrating = reader.GetValue(3);
					var searchstate = reader.GetString(4);
					var searchlocation = reader.GetString(5);

					table.AddRow($"{foundbook}",
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
		return foundbook;
	}

	// Change our book
	public void UpdateBook(string title = "")
	{
		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			var table = new Table();

			connection.Open();

			if ( title == "")
			{
				Console.Write("Title: ");
				title = Console.ReadLine();
			}

			// Make sure that the title is in the database
			if ( SearchBook(title) == "")
			{
				return;
			}

			var command = connection.CreateCommand();

			command.CommandText =
				@"
					select * from books where title = $title
				";

			command.Parameters.AddWithValue("$title", title);

			// Get the current settings
			using (var reader = command.ExecuteReader())
			{
				// This should always return true
				if ( reader.HasRows )
				{
					reader.Read();
					var searchtitle = reader.GetString(0);
					var searchauthor = reader.GetString(1);
					var searchgenre = reader.GetString(2);
					var searchrating = reader.GetValue(3);
					var searchstate = reader.GetString(4);
					var searchlocation = reader.GetString(5);
					reader.Close();

					// We do not change our titles
					Console.WriteLine($"Title : {searchtitle}");

					Console.WriteLine("Enter Update Information, press enter to keep current value: ");

					Console.Write($"Author : ({searchauthor}) : ");
					var newauthor = Console.ReadLine();
					if ( newauthor != "" )
						searchauthor = newauthor;	

					Console.Write($"Genre : ({searchgenre}) : ");
					var newgenre = Console.ReadLine();
					if ( newgenre != "" )
						searchgenre = newgenre;	

					Console.Write($"Rating : ({searchrating}) :");
					var newrating = Console.ReadLine();
					if ( newrating != "" )
						searchrating = newrating;	

					Console.WriteLine($"State : ({searchstate}) : ");
					var newstate  = AnsiConsole.Prompt(
							new SelectionPrompt<string>()
							.Title("Please select option:")
							.AddChoices(new[] {
							"Keep Current State",
							"Owns",
							"Wants",
							"Selling",
							"Sold"
							}));
	
					if ( newstate != "Keep Current State" )
						searchstate = newstate;	

					// If we have sold our book then its location can only be the store
					if ( newstate == "Sold" )
					{
						searchlocation = "Store";
					}
					else
					{
						Console.Write($"Location : ({searchlocation}) :");
						var newlocation = Console.ReadLine();
						if ( newlocation != "" )
							searchlocation = newlocation;	
					}

					var updatecommand = connection.CreateCommand();

					updatecommand.CommandText =
						@"
							REPLACE INTO books(title,author,genre,rating,location,state)
							values ($title,$author,$genre,$rating,$location,$state)
						";

					updatecommand.Parameters.AddWithValue("$title", searchtitle);
					updatecommand.Parameters.AddWithValue("$author", searchauthor);
					updatecommand.Parameters.AddWithValue("$genre", searchgenre);
					updatecommand.Parameters.AddWithValue("$rating", searchrating);
					updatecommand.Parameters.AddWithValue("$state", searchstate);
					updatecommand.Parameters.AddWithValue("$location", searchlocation);

					updatecommand.ExecuteNonQuery();
					connection.Close();

					table = new Table();
					
					table.AddColumn("Title");
					table.AddColumn("Author");
					table.AddColumn("Genre");
					table.AddColumn("Rating");
					table.AddColumn("State");
					table.AddColumn("Location");

					table.AddRow($"{searchtitle}",
							$"{searchauthor}",
							$"{searchgenre}",
							$"{searchrating}",
							$"{searchstate}",
							$"{searchlocation}");
					AnsiConsole.Write(table);
				}
			}
		}
	}

	public int RemoveBook(string title = "")
	{
		// Assume no books will be removed
		int numberRemoved = 0;
		
		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			if ( title == "" )
			{
				Console.Write("Title: ");
				title = Console.ReadLine();
			}

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

	public void ReportBooks(string searchfield = "", string searchvalue = "")
	{
		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			connection.Open();

			if ( searchfield == "" )
			{
				Console.Write("Field: ");
				searchfield = Console.ReadLine();
			}

			if ( searchvalue == "" )
			{
				Console.Write("Value: ");
				searchvalue = Console.ReadLine();
			}

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
						var searchtitle = reader.GetString(0);
						var searchauthor = reader.GetString(1);
						var searchgenre = reader.GetString(2);
						var searchrating = reader.GetValue(3);
						var searchstate = reader.GetString(4);
						var searchlocation = reader.GetString(5);

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
			connection.Close();
		}
	}

	public int BookCount()
	{
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
				{
					bookCount = int.Parse(reader.GetString(0));
				}
			}
		}
		return bookCount;
	}
}
