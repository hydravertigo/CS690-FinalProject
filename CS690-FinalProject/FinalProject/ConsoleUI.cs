namespace FinalProject;

using Spectre.Console;

#pragma warning disable CS8603 // Do not fuss about null reference returns
#pragma warning disable CS8604 // Do not fuss about null int conversions
#pragma warning disable CS8600 // Do not fuss about null string conversions

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
				var bookCount = bookDatabase.BookCount();
				Console.WriteLine($"There are {bookCount} books in the database\n");
			}
			else if (mode == "Add Book")
			{
				string title = AskForInput("Title: ");
				string author = AskForInput("Author: ");
				string genre = AskForInput("Genre: ");
				int rating = int.Parse(AskForInput("Rating (1-5): "));

				string state = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
					.Title("Please Choose State Option:")
					.AddChoices(new[] {
					"Owns",
					"Wants",
					"Selling",
					"Sold"
					}));
			
				string location = AskForInput("Location: ");

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
				bookDatabase.UpdateBook();
			}
			else if ( mode == "Remove Book" )
			{
				bookDatabase.RemoveBook();
			}
			else if ( mode == "Count Books" )
			{
				var bookCount = bookDatabase.BookCount();
				Console.WriteLine($"\nThere are {bookCount} books in the database\n");
			}
			else if ( mode == "Report Books")
			{
				var field = AnsiConsole.Prompt(
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
					bookDatabase.ReportBooks("All","All");
				}
				else
				{
					string searchvalue;

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
					bookDatabase.ReportBooks(field,searchvalue);
				}
			}
		}
	}

	public static string AskForInput(string message)
	{
		string answer = "";

		while ( answer == "")
		{
			Console.Write(message);
			answer = Console.ReadLine();
		}

		return answer;
	}

}
