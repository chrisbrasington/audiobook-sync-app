using AudiobookParserApp;

string root = $"C:\\users\\{Environment.UserName}\\Music";
string source = Path.Combine(root, "My Media\\OverDrive");
string destination = Path.Combine(root, "audiobooks");

Console.WriteLine($"Searching {root}..");

if(Directory.Exists(source))
{
    Console.WriteLine("Found Overdrive folder..\n");

    BookCollection library = new BookCollection(source);
    library.Sort();

    foreach(Book book in library)
    {
        Console.WriteLine(book);
        book.Export(destination);
    }

    Console.WriteLine(library.Status());
}
else
{
    Console.WriteLine("Overdrive folder not found, exiting");
}
