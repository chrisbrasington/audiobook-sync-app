using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiobookParserApp
{
    public class BookCollection : IList<Book>
    {
        private List<Book> _books = new List<Book>();

        public Book this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => _books.Count;

        public bool IsReadOnly => false;

        public BookCollection() { }

        public BookCollection(string root)
        {
            foreach (string bookPath in Directory.GetDirectories(root))
            {
                Book book = new Book(bookPath);

                if(!book.IsEmpty)
                {
                    _books.Add(book);
                }
            }
        }

        public void Add(Book item)
        {
            _books.Add(item);
        }

        public void Clear()
        {
            _books.Clear();
        }

        public bool Contains(Book item)
        {
            foreach(Book book in _books)
            {
                if(book == item)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(Book[] array, int arrayIndex)
        {
            _books.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Book> GetEnumerator()
        {
            return _books.GetEnumerator();
        }

        public int IndexOf(Book item)
        {
            int index = 0;
            foreach( Book book in _books)
            {
                if(book == item)
                {
                    return index; ;
                }
                index++;
            }
            return -1;
        }

        public void Insert(int index, Book item)
        {
            _books.Insert(index, item);
        }

        public bool Remove(Book item)
        {
           return _books.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _books.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _books.GetEnumerator();
        }

        public void Sort()
        {
            _books = _books.OrderBy(b => b.Title).ToList();
        }

        public bool Export(string path)
        {
            foreach(Book book in _books)
            {
                book.Export(path);
            }

            return true;
        }

        public string Status()
        {
            int backupCount = _books.Where(b => b.BackupOperationOccurred).Count();
            int verified = _books.Where(b => b.Verified).Count();

            string result = $"Backed up {backupCount} books.";

            if(verified == _books.Count)
            {
                result += "\nAll books verified in backup directory";
            }
            else
            {
                result += $"\nERROR: Not all books verified (check for missing): {verified}/{_books.Count}";
            }

            return result;
        }
    }
}
