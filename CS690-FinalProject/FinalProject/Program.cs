namespace FinalProject;

class Program
{
    static void Main(string[] args)
    {
        string selection = "";

        while ( selection != "e" && selection != "E")
        {

            Console.Write("Main Menu\n\n");
            Console.Write("[A]dd Book\n");
            Console.Write("[S]earch Book\n");
            Console.Write("[R]eport Books\n");
            Console.Write("[E]xit\n");

            Console.Write("\nPlease Select Option by first letter: ");

            selection = Console.ReadLine();

            switch(selection)
            {
                case "a":
                case "A":
                    Console.Write("Add Book");
                    break;
                case "s":
                case "S":
                    Console.Write("Search Book");
                    break;
                case "r":
                case "R":
                    Console.Write("Report Book");
                    break;
                case "e":
                case "E":
                    Console.Write("Exit Out");
                    break;
                default:
                    Console.Write("Unknown option");
                    break;
            }
        }
    }
}
