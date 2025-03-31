using LibMgmt.Models;
using LibMgmt.Repositories.interfaces;

namespace LibMgmt.Repositories.implementations
{
    public class InMemoryBookRepo : IBookRepo
    {
        private readonly Dictionary<string, Book> _memoryCache = [];

        public async Task<RepositoryResult<bool>> Delete(string isbn)
        {
            try
            {
                if (!_memoryCache.ContainsKey(isbn))
                {
                    return RepositoryResult<bool>.CreateRepositoryResult(false, RepositoryErrors.KeyNotExist);
                }

                _memoryCache.Remove(isbn);
                return RepositoryResult<bool>.CreateRepositoryResult(true, RepositoryErrors.KeyNotExist);
            }
            catch (Exception ex)
            {
                return RepositoryResult<bool>.CreateRepositoryResult(false, RepositoryErrors.Other, ex.Message);
            }
        }

        public async Task<RepositoryResult<Book?>> GetByKey(string isbn)
        {
            try
            {
                if (_memoryCache.ContainsKey(isbn))
                {
                    return RepositoryResult<Book?>.CreateRepositoryResult<Book?>(_memoryCache[isbn]);
                }
                return RepositoryResult<Book?>.CreateRepositoryResult<Book?>(null, RepositoryErrors.KeyNotExist);
            }
            catch (Exception ex)
            {
                return RepositoryResult<Book?>.CreateRepositoryResult<Book?>(null, RepositoryErrors.Other, ex.Message);
            }
        }

        public async Task<RepositoryResult<IEnumerable<Book>>> GetAll()
        {
            try
            {
                var books = _memoryCache.Values.AsEnumerable();
                return RepositoryResult<IEnumerable<Book>>.CreateRepositoryResult(books);
            }
            catch (Exception ex)
            {
                return RepositoryResult<IEnumerable<Book>>.CreateRepositoryResult<IEnumerable<Book>>(null, RepositoryErrors.Other, ex.Message);
            }
        }

        public async Task<RepositoryResult<Book?>> Add(Book book)
        {
            try
            {
                if (_memoryCache.ContainsKey(book.ISBN))
                {
                    return RepositoryResult<Book?>.CreateRepositoryResult<Book?>(null, RepositoryErrors.KeyDuplicate);
                }

                _memoryCache.Add(book.ISBN, book);
                return RepositoryResult<Book?>.CreateRepositoryResult(_memoryCache[book.ISBN]);
            }
            catch (Exception ex)
            {
                return RepositoryResult<Book?>.CreateRepositoryResult<Book?>(null, RepositoryErrors.Other, ex.Message);
            }
        }

        public async Task<RepositoryResult<Book?>> Update(Book book)
        {
            try
            {
                if (_memoryCache.ContainsKey(book.ISBN))
                {
                    _memoryCache[book.ISBN] = book;
                    return RepositoryResult<Book?>.CreateRepositoryResult(_memoryCache[book.ISBN]);
                }

                return RepositoryResult<Book?>.CreateRepositoryResult<Book?>(null, RepositoryErrors.KeyNotExist);
            }
            catch (Exception ex)
            {
                return RepositoryResult<Book?>.CreateRepositoryResult<Book?>(null, RepositoryErrors.Other, ex.Message);
            }
        }
    }
}
