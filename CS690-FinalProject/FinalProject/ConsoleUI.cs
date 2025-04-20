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
				bookDatabase.AddBook(title,
						author,
						genre,
						rating,
						state,
						location);
			}
			else if ( mode == "Search Book" )
			{
				string title = AskForInput("Search Title: ");
				bookDatabase.SearchBook(title);
			}
			else if ( mode == "Update Book" )
			{
				string title = AskForInput("Update Title: ");
				bookDatabase.UpdateBook(title);
			}
			else if ( mode == "Remove Book" )
			{
				string title = AskForInput("Remove Title: ");
				bookDatabase.RemoveBook(title);
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
}
