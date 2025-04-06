namespace FinalProject

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
}
