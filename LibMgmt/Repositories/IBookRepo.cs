using LibMgmt.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Caching;


namespace LibMgmt.Repositories
{
    public enum RepositoryErrors
    {
        KeyNotExist,
        KeyDuplicate,
        Other
    }

    public class RepositoryResult<T>
    {
        public T? Result { get; set; }
        public RepositoryErrors? Error { get; set; }   
        public string? Message { get; set; }

        public static RepositoryResult<T> CreateRepositoryResult<T>(T? result, RepositoryErrors? error = null, string? message = null)
        {
            return new RepositoryResult<T>()
            {
                Result = result,
                Error = error,
                Message = message
            };
        }
    }

    public interface IBaseRepo<T,K>
    {
        Task<RepositoryResult<T?>> Add(T value);
        Task<RepositoryResult<T?>> GetByKey(K key);
        Task<RepositoryResult<bool>> Delete(K key);
        Task<RepositoryResult<IEnumerable<T>>> GetAll();
        Task<RepositoryResult<T?>> Update(T value);
    }

    public interface IBookRepo : IBaseRepo<Book, string>
    {
    }

    public class InMemoryBookRepo : IBookRepo
    {
        private readonly Dictionary<string, Book> _memoryCache = [];

        public async Task<RepositoryResult<bool>> Delete(string isbn)
        {
            try
            {
                if (!_memoryCache.ContainsKey(isbn))
                {
                    return RepositoryResult<bool>.CreateRepositoryResult<bool>(false, RepositoryErrors.KeyNotExist);
                }

                _memoryCache.Remove(isbn);
                return RepositoryResult<bool>.CreateRepositoryResult<bool>(true, RepositoryErrors.KeyNotExist);
            }
            catch (Exception ex)
            {
                return RepositoryResult<bool>.CreateRepositoryResult<bool>(false, RepositoryErrors.Other, ex.Message);
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
                return RepositoryResult<IEnumerable<Book>>.CreateRepositoryResult<IEnumerable<Book>>(books);
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
