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
				  "Add Book",
				  "Search Book",
				  "Update Book",
				  "Remove Book",
				  "Count Books",
				  "Report Books",
				  "Exit Program"
				}));

			if (mode == "Add Book")
			{
				bookDatabase.AddBook();
			}
			else if ( mode == "Search Book" )
			{
				bookDatabase.SearchBook();
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
				Console.WriteLine($"\nThere are {bookDatabase.bookCount} books in the database\n");
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
        Console.Write(message);
        return Console.ReadLine();
    }

}
