namespace FinalProject;

using Spectre.Console;

public class ConsoleUI
{
	public void Show()
	{
        var mode = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select option:")
                .AddChoices(new[] {
                    "Add Book",
						  "Search Book",
						  "Report Book",
						  "Exit Program"
                }));
	}
}
