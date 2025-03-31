using LibMgmt.Models;

namespace LibMgmt.Services.interfaces
{
    public interface ILibraryManagementService
    {
        Task<ServiceResult<Book>> AddBook(Book book);
        Task<ServiceResult<Book>> UpdateBook(Book book);
        Task<ServiceResult<bool?>> DeleteBook(string isbn);
        Task<ServiceResult<IEnumerable<Book>>> ListAllBooks();
        Task<ServiceResult<Book>> GetBook(string isbn);
    }
}
