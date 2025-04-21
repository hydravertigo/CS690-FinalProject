namespace FinalProject;

// Book class to allow us to deal with books as objects

public class Book
{
	public string 	Title		{ get; set; }
	public string 	Author	{ get; set; }
	public string 	Genre		{ get; set; }
	public int 		Rating	{ get; set; }
	public string 	State		{ get; set; }
	public string	Location	{ get; set; }

	// Create an empty book object by default
	public Book(string Title = "", string Author = "", string Genre = "", 
					int Rating = -1, string State = "", string Location = "")
	{
		this.Title = Title;
		this.Author= Author;
		this.Genre = Genre;
		this.Rating = Rating;
		this.State = State;
		this.Location = Location;
	}

	public override string ToString()
	{
		return this.Title;
	}
}

