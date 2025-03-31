using LibMgmt.Models;
using LibMgmt.Repositories;
using LibMgmt.Repositories.interfaces;
using LibMgmt.Services.interfaces;

namespace LibMgmt.Services.implementations
{
    public class LibraryManagementService : ILibraryManagementService
    {
        public const long INVALID_ISBN = 1000001;
        public const long ISBN_ALREADY_EXISTING = 1000002;
        public const long ISBN_NOT_EXIST = 1000003;
        public const long REPOSITORY_ERROR = 1000004;
        public const long OTHER_ERROR = 1000005;

        private readonly IBookRepo _bookRepo;
        private readonly IIsbnValidator _isbnValidator;

        public LibraryManagementService(IBookRepo bookRepo, IIsbnValidator isbnValidator)
        {
            _bookRepo = bookRepo;
            _isbnValidator = isbnValidator;
        }

        private ServiceResult<T> CreateServiceResult<T>(T? result, long? errorCode = null, string? errorMessage = null)
        {
            if (result != null)
            {
                return new ServiceResult<T>() { Result = result };
            }

            return new ServiceResult<T>() { ErrorCode = errorCode.Value, ErrorMessage = errorMessage };
        }

        public async Task<ServiceResult<bool?>> DeleteBook(string isbn)
        {
            try
            {
                if (!_isbnValidator.IsValid(isbn))
                {
                    return CreateServiceResult<bool?>(null, INVALID_ISBN);
                }

                var result = await _bookRepo.Delete(isbn);

                if (result.Result)
                {
                    return CreateServiceResult<bool?>(true);
                }
                else
                {
                    var errorCode = result.Error == RepositoryErrors.KeyNotExist ? ISBN_NOT_EXIST : REPOSITORY_ERROR;
                    return CreateServiceResult<bool?>(null, errorCode);
                }
            }
            catch (Exception ex)
            {
                return CreateServiceResult<bool?>(null, OTHER_ERROR, ex.Message);
            }
        }

        public async Task<ServiceResult<Book?>> GetBook(string isbn)
        {
            try
            {
                if (!_isbnValidator.IsValid(isbn))
                {
                    return CreateServiceResult<Book?>(null, INVALID_ISBN);
                }

                var result = await _bookRepo.GetByKey(isbn);
                if (result.Result != null)
                {
                    return CreateServiceResult<Book?>(result.Result);
                }
                else
                {
                    var errorCode = result.Error == RepositoryErrors.KeyNotExist ? ISBN_NOT_EXIST : REPOSITORY_ERROR;
                    return CreateServiceResult<Book?>(null, errorCode);
                }
            }
            catch (Exception ex)
            {
                return CreateServiceResult<Book?>(null, OTHER_ERROR, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<Book>>> ListAllBooks()
        {
            try
            {
                var result = await _bookRepo.GetAll();
                if (result.Result != null)
                {
                    return CreateServiceResult(result.Result);
                }
                else
                {
                    return CreateServiceResult<IEnumerable<Book>>(null, REPOSITORY_ERROR);
                }
            }
            catch (Exception ex)
            {
                return CreateServiceResult<IEnumerable<Book>>(null, OTHER_ERROR, ex.Message);
            }
        }

        public async Task<ServiceResult<Book?>> AddBook(Book book)
        {
            try
            {
                if (!_isbnValidator.IsValid(book.ISBN))
                {
                    return CreateServiceResult<Book?>(null, INVALID_ISBN);
                }

                var result = await _bookRepo.Add(book);
                if (result.Result != null)
                {
                    return CreateServiceResult<Book?>(result.Result);
                }
                else
                {
                    var errorCode = result.Error == RepositoryErrors.KeyDuplicate ? ISBN_ALREADY_EXISTING : REPOSITORY_ERROR;
                    return CreateServiceResult<Book?>(null, errorCode);
                }
            }
            catch (Exception ex)
            {
                return CreateServiceResult<Book?>(null, OTHER_ERROR, ex.Message);
            }
        }

        public async Task<ServiceResult<Book?>> UpdateBook(Book book)
        {
            try
            {
                if (!_isbnValidator.IsValid(book.ISBN))
                {
                    return CreateServiceResult<Book?>(null, INVALID_ISBN);
                }

                var result = await _bookRepo.Update(book);
                if (result.Result != null)
                {
                    return CreateServiceResult<Book?>(result.Result);
                }
                else
                {
                    var errorCode = result.Error == RepositoryErrors.KeyNotExist ? ISBN_NOT_EXIST : REPOSITORY_ERROR;
                    return CreateServiceResult<Book?>(null, errorCode);
                }
            }
            catch (Exception ex)
            {
                return CreateServiceResult<Book?>(null, OTHER_ERROR, ex.Message);
            }
        }
    }
}
