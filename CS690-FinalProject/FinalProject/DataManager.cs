namespace FinalProject;

//using System;
//using System.IO;
using Microsoft.Data.Sqlite;
using Spectre.Console;

#pragma warning disable CS8604 // Do not fuss about null int conversions
#pragma warning disable CS8600 // Do not fuss about null string conversions

public class DataManager
{
	public DataManager()
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
							location text not null,
							state text not null,
							primary key(title)
						);
					";
				command.ExecuteNonQuery();
				//connection.Close();
			}
		}
	}

	// Add a book to the database
	public void AddBook(string title = "",
			string author = "",
			string genre = "",
			int rating = -1,
			string location = "",
			string state = ""
			)
	{
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

			if ( rating < 1 )
				rating = 1;
			else if ( rating > 5 )
				rating = 5;

			if ( location == "")
			{
				Console.Write("Location: ");
				location = Console.ReadLine();
			}

			if ( state == "" )
			{
				Console.Write("State: ");
				state = Console.ReadLine();
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
			command.Parameters.AddWithValue("$location", location);
			command.Parameters.AddWithValue("$state", state);

			// We cannot add the same title twice
			try
			{
				command.ExecuteNonQuery();
			}
			catch ( Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine($"\nThe title {title} is already in the database. Use UpdateBook to change it\n");
			}	

			connection.Close();

			SearchBook(title);
		}
	}

	// Find a book based on its title
	public void SearchBook(string title = "")
	{
		using (var connection = new SqliteConnection("Data Source=books.db"))
		{
			var table = new Table();
			
			table.AddColumn("Title");
			table.AddColumn("Author");
			table.AddColumn("Genre");
			table.AddColumn("Rating");
			table.AddColumn("Location");
			table.AddColumn("State");

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
				while (reader.Read())
				{
					var searchtitle = reader.GetString(0);
					var searchauthor = reader.GetString(1);
					var searchgenre = reader.GetString(2);
					var searchrating = reader.GetValue(3);
					var searchlocation = reader.GetString(4);
					var searchstate = reader.GetString(5);

					table.AddRow($"{searchtitle}",
							$"{searchauthor}",
							$"{searchgenre}",
							$"{searchrating}",
							$"{searchlocation}",
							$"{searchstate}");
				}
			}
			AnsiConsole.Write(table);
			connection.Close();
		}
	}
 

}
