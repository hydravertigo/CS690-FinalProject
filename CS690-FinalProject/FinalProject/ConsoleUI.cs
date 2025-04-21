namespace FinalProject;

using Spectre.Console;

public class ConsoleUI
{
	BookDatabase bookDatabase;

	public ConsoleUI()
	{
		bookDatabase = new BookDatabase();
	}

	public void Show()
	{
		var mode = "";

		while ( mode != "Exit Program" )
		{

			mode = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
				.Title("Please select option:")
				.AddChoices(new[] {
				  "List All Books",
				  "Add Book",
				  "Search Book",
				  "Update Book",
				  "Remove Book",
				  "Count Books",
				  "Report Books",
				  "Exit Program"
				}));

			if (mode == "List All Books" )
			{
				bookDatabase.ReportBooks("All","All");
			}
			else if (mode == "Add Book")
			{
				// Get book details from user
				string title = AskForInput("Title: ");
				string author = AskForInput("Author: ");
				string genre = AskForInput("Genre: ");
				int rating = int.Parse(AskForInput("Rating (1-5): "));

				// Correct our rating range
				if ( rating < 1 )
					rating = 1;
				else if ( rating > 5 )
					rating = 5;

				string state = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
					.Title("Please Choose State Option:")
					.AddChoices(new[] {
					"Owns",
					"Wants",
					"Selling",
					"Sold"
					}));

				// Determine location of book
				string location;

				// If the state is "Wants" then the location must be the store
				if ( state == "Wants" )
					location = "Store";
				else // otherwise ask Sofia for a location string
					location = AskForInput("Location: ");

				// Attempt to add book to database
				bookDatabase.AddBook(new Book(title,author,genre,rating,state,location));

				// Show the book that we just tried to add
				bookDatabase.SearchBook(title,true);

				// Attempt to add book to database
				//bookDatabase.AddBook(title,
				//		author,
				//		genre,
				//		rating,
				//		state,
				//		location);
			}
			else if ( mode == "Search Book" )
			{
				string title = AskForInput("Search Title: ");
				bookDatabase.SearchBook(title,true);
			}
			else if ( mode == "Update Book" )
			{
				string title = AskForInput("Update Title: ");

				// Search for our book in the database
				Book foundBook = bookDatabase.SearchBook(title,true);

				// Only update if the book is in the database
				if ( ! String.IsNullOrEmpty(foundBook.ToString()))
				{
					// New console output table
					//var table = new Table();

					foundBook.Author = UpdateField("Author",foundBook.Author);
					foundBook.Genre = UpdateField("Genre",foundBook.Genre);
					foundBook.Rating = int.Parse(UpdateField("Rating",foundBook.Rating.ToString()));
					
					// Ask the user to select a new state
					var keepStateOption = $"Keep Current State : ({foundBook.State})";

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
						foundBook.State = newStateSelect;

					//If we have sold our book then its location can only be the store
					if ( foundBook.State == "Sold" )
						foundBook.Location = "Store";
					else
						foundBook.Location = UpdateField("Location",foundBook.Location);

					// Now that all of the fields in our book object have been updated
					// use this object to update the database
					bookDatabase.UpdateBook(foundBook);

					// Show us the new state for the book in the database
					bookDatabase.SearchBook(foundBook.Title,true);
				}
			}
			else if ( mode == "Remove Book" )
			{
				string title = AskForInput("Remove Title: ");
				bookDatabase.RemoveBook(title,true);
			}
			else if ( mode == "Count Books" )
			{
				var bookCount = bookDatabase.BookCount();
				Console.WriteLine($"\nThere are {bookCount} books in the database\n");
			}
			else if ( mode == "Report Books")
			{
				string field;
				string searchvalue;

				// What field will we search by
				field = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
				.Title("Please select option:")
				.AddChoices(new[] {
				  "All Books",
				  "Author",
				  "Genre",
				  "Rating",
				  "Location",
				  "State"
				}));

				if ( field == "All Books")
				{
					field = "All";
					searchvalue = "All";
				}
				else
				{
					if (field == "State")
					{
						searchvalue = AnsiConsole.Prompt(
							new SelectionPrompt<string>()
							.Title("Please select option:")
							.AddChoices(new[] {
							"Owns",
							"Wants",
							"Selling",
							"Sold"
							}));
					}
					else
					{
						searchvalue = AskForInput("Please enter a search value: ");
					}
				}

				bookDatabase.ReportBooks(field,searchvalue);
			}
		}
	}

	// Method to get non-empty input string from user
	public static string AskForInput(string message)
	{
		string answer = "";

		while ( answer == "")
		{
			Console.Write(message);

			// This construction is a work around for warning 
			// CS8600: Converting null literal or possible null value to non-nullable type
			answer = Console.ReadLine() ?? "";
		}

		return answer;
	}

	// Prompt user to enter value for field
	// If user enters "" then return the existing field
	// otherwise return the updated field
	public static string UpdateField(string message, string field)
	{
		Console.WriteLine("\nEnter Update Information, press enter to keep current value: ");
		Console.Write($"{message} : ({field}) : ");
		var answer = Console.ReadLine(); 

		if ( ! String.IsNullOrEmpty(answer))
			field = answer;

		return field;
	}

}
