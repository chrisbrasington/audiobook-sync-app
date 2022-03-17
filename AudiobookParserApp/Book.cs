using System.Text.RegularExpressions;

namespace AudiobookParserApp
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string FullPath { get; set; }

        public string Guid { get; set; }

        public string ExportFolder
        {
            get
            {
                string value = $"{Title.TrimEnd()} - {Author}";
                var invalids = Path.GetInvalidFileNameChars();
                value = String.Join("_", value.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
                return value; 
            }
        }
        public List<string> Images { get; set; }
        public List<string> AudioFiles { get; set; }
        public List<string> OtherContent { get; set; }

        public bool IsEmpty
        {
            get { return AudioFiles.Count == 0; }
        }

        public bool Verified { get; set; }
        public bool BackupOperationOccurred { get; set; }

        public Book() { }

        public Book(string path)
        {
            this.FullPath = path;
            this.Guid = Path.GetFileName(path);
                
            Images = new List<string>();
            AudioFiles = new List<string>();
            OtherContent = new List<string>();

            foreach (string file in Directory.GetFiles(path))
            {

                switch (Path.GetExtension(file))
                {
                    case ".jpg":
                    case ".png":
                        {
                            Images.Add(file);
                            break;
                        }
                    case ".mp3":
                        {
                            AudioFiles.Add(file);
                            break;
                        }
                    default:
                        {
                            OtherContent.Add(file);
                            break;
                        }
                }
            }

            if (!IsEmpty)
            {
                TagLib.File metaData = TagLib.File.Create(AudioFiles[0]);

                // album is more accurate than title, but does not always exist
                if (metaData.Tag.Album == null)
                {
                    Title = metaData.Tag.Title.Split('-')[0];
                }
                else
                {
                    Title = metaData.Tag.Album;
                }
                Author = metaData.Tag.FirstPerformer;
            
                // remove invalid characters from directory name
                Description = StripHTML(metaData.Tag.Comment);
            }
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public override string ToString()
        {
            return $"{Title} / {Author} - {AudioFiles.Count} audio files\n";
        }

        public bool Export(string path)
        {
            if(IsEmpty)
            {
                return false;
            }

            string output = Path.Combine(path, ExportFolder);


            Console.WriteLine(output);
            Console.WriteLine($"\t{Guid}");

            if(!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);  
            }

            bool workWasDone = false;

            foreach (string source in Images)
            {
                workWasDone |= CopyFile(source, output);
            }
            foreach(string source in AudioFiles)
            {
                workWasDone |= CopyFile(source, output);
            }

            if (workWasDone)
            { 
                Console.WriteLine("\tAll files archived");
                BackupOperationOccurred = true;
            }
            else
            {
                Console.WriteLine("\tAll files verified");
            }
                
            Verified = true;
            Console.WriteLine();

            return true;
        }

        private bool CopyFile(string source, string output)
        {
            string destination = Path.Combine(output, Path.GetFileName(source));

            Console.Write($"\t{Path.GetFileName(source)}");

            if (!File.Exists(destination))
            {
                Console.WriteLine(" - Copied");
                File.Copy(source, destination);
                return true;
            }
                
            Console.WriteLine(" - Exists");
            return false; // already exists
        }
    }
}
