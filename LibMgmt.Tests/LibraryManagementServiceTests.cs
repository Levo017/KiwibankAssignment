using LibMgmt.Models;
using LibMgmt.Repositories;
using LibMgmt.Repositories.interfaces;
using LibMgmt.Services.implementations;
using LibMgmt.Services.interfaces;
using Moq;

namespace LibMgmt.Tests
{
    public class LibraryManagementServiceTests
    {

        private const string ISBN = "978-7-6499-1995-5";

        private IIsbnValidator GetMockIsbnValidator(bool validationResult)
        {
            var mockValidationService = new Mock<IIsbnValidator>();
            mockValidationService
                .Setup(x => x.IsValid(It.IsAny<string>()))
                .Returns(validationResult);

            return mockValidationService.Object;
        }

        private IBookRepo GetMockBookRepo(bool duplicateKey = false, bool notExistingKey = false, bool hasError = false, bool throwError = false)
        {
            var mockBookRepo = new Mock<IBookRepo>();

            if (throwError)
            {
                mockBookRepo.Setup(x => x.Add(It.IsAny<Book>())).Throws(new ApplicationException());
                mockBookRepo.Setup(x => x.Update(It.IsAny<Book>())).Throws(new ApplicationException());
                mockBookRepo.Setup(x => x.GetByKey(It.IsAny<string>())).Throws(new ApplicationException());
                mockBookRepo.Setup(x => x.Delete(It.IsAny<string>())).Throws(new ApplicationException()); 
                mockBookRepo.Setup(x => x.GetAll()).Throws(new ApplicationException());

                return mockBookRepo.Object;
            }

            var mockDeleteResult = new RepositoryResult<bool>()
            {
                Result = notExistingKey || hasError ? false : true,
                Error = notExistingKey ? RepositoryErrors.KeyNotExist : hasError ? RepositoryErrors.Other : null,
                Message = hasError ? "Exception message" : null
            };

            var getAllResult = new RepositoryResult<IEnumerable<Book>>()
            {
                Result = hasError ? null : new List<Book>(),
                Error = duplicateKey ? RepositoryErrors.KeyDuplicate : hasError ? RepositoryErrors.Other : null,
                Message = hasError ? "Exception message" : null
            };

            var getOneResult = new RepositoryResult<Book?>()
            {
                Result = null,
                Error = notExistingKey ? RepositoryErrors.KeyNotExist : hasError ? RepositoryErrors.Other : null,
                Message = hasError ? "Exception message" : null
            };

            mockBookRepo.Setup(x => x.Add(It.IsAny<Book>()))
                .ReturnsAsync((Book book) =>
                    new RepositoryResult<Book?>()
                    {
                        Result = duplicateKey || hasError ? null : book,
                        Error = duplicateKey ? RepositoryErrors.KeyDuplicate : hasError ? RepositoryErrors.Other : null,
                        Message = hasError ? "Exception message" : null
                    })
                .Callback((Book book) =>
                {
                    if (!hasError)
                    {
                        getOneResult.Result = notExistingKey ? null : book;
                        getAllResult.Result = new List<Book>() { book };
                    }
                });

            mockBookRepo.Setup(x => x.Update(It.IsAny<Book>()))
                .ReturnsAsync((Book book) => 
                    new RepositoryResult<Book?>()
                    {
                        Result = notExistingKey || hasError ? null : book,
                        Error = notExistingKey ? RepositoryErrors.KeyNotExist : hasError ? RepositoryErrors.Other : null,
                        Message = hasError ? "Exception message" : null
                    })
                .Callback((Book book) =>
                {
                    if (!hasError)
                    {
                        getOneResult.Result = notExistingKey ? null : book; 
                        getAllResult.Result = new List<Book>() { book };
                    }
                });
            mockBookRepo.Setup(x => x.GetByKey(It.IsAny<string>()))
                .ReturnsAsync(getOneResult);
            mockBookRepo.Setup(x => x.Delete(It.IsAny<string>())).ReturnsAsync(mockDeleteResult);
            mockBookRepo.Setup(x => x.GetAll())
                .ReturnsAsync(getAllResult);

            return mockBookRepo.Object;
        }

        [Fact]
        public async Task CRUD_Functions_Success()
        {
            var mockValidationService = GetMockIsbnValidator(true);
            var mockRepository = GetMockBookRepo();
            var sub = new LibraryManagementService(mockRepository, mockValidationService);

            var book1 = new Book(ISBN)
            {
                Author = "John_1",
                Title = "Anything will do",
                Description = "This is John's first book."
            };

            // Add
            var bookAdded1 = (await sub.AddBook(book1)).Result;
            Assert.Equal(book1, bookAdded1);

            // Get All
            var currentBooks = (await sub.ListAllBooks()).Result;
            Assert.NotNull(currentBooks);
            Assert.Single(currentBooks);
            Assert.Contains(book1, currentBooks);

            // Get one
            var bookToFind = (await sub.GetBook(ISBN)).Result;
            Assert.NotNull(bookToFind);
            Assert.Equal(book1, bookToFind);

            // Update
            var expectedTitle = "Just anything";
            var bookToUpdate = book1;
            bookToUpdate.Title = expectedTitle;

            var bookUpdated = (await sub.UpdateBook(bookToUpdate)).Result;
            Assert.Equal(bookToUpdate, bookUpdated);

            // Delete
            var bookDeleteResult = (await sub.DeleteBook(ISBN)).Result;
            Assert.True(bookDeleteResult);
        }

        [Fact]
        public async Task Invalid_ISBN()
        {
            var mockValidationService = GetMockIsbnValidator(false);
            var mockRepository = GetMockBookRepo();
            var sub = new LibraryManagementService(mockRepository, mockValidationService);

            var book = new Book(ISBN)
            {
                Author = "John_1",
                Title = "Anything will do",
                Description = "This is John's first book."
            };

            // Add
            var addBookResult = await sub.AddBook(book);
            Assert.False(addBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.INVALID_ISBN, addBookResult.ErrorCode);

            // Get one
            var getBookResult = await sub.GetBook(ISBN);
            Assert.False(getBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.INVALID_ISBN, getBookResult.ErrorCode);

            // Update
            var updateBookResult = await sub.UpdateBook(book);
            Assert.False(updateBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.INVALID_ISBN, updateBookResult.ErrorCode);

        }

        [Fact]
        public async Task Map_KeyNotFound_RepositoryError()
        {
            var mockValidationService = GetMockIsbnValidator(true);
            var mockRepository = GetMockBookRepo(notExistingKey:true);
            var sub = new LibraryManagementService(mockRepository, mockValidationService);

            var book = new Book(ISBN)
            {
                Author = "John_1",
                Title = "Anything will do",
                Description = "This is John's first book."
            };

            // Update
            var updateBookResult = await sub.UpdateBook(book);
            Assert.False(updateBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.ISBN_NOT_EXIST, updateBookResult.ErrorCode);

            // Get one
            var getBookResult = await sub.GetBook(ISBN);
            Assert.False(getBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.ISBN_NOT_EXIST, getBookResult.ErrorCode);

            // Delete
            var deleteBookResult = await sub.DeleteBook(ISBN);
            Assert.False(deleteBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.ISBN_NOT_EXIST, deleteBookResult.ErrorCode);

        }

        [Fact]
        public async Task Map_DuplicateKey_RepositoryError()
        {
            var mockValidationService = GetMockIsbnValidator(true);
            var mockRepository = GetMockBookRepo(duplicateKey: true);
            var sub = new LibraryManagementService(mockRepository, mockValidationService);

            var book = new Book(ISBN)
            {
                Author = "John_1",
                Title = "Anything will do",
                Description = "This is John's first book."
            };

            // Add
            var addBookResult = await sub.AddBook(book);
            Assert.False(addBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.ISBN_ALREADY_EXISTING, addBookResult.ErrorCode);
        }

        [Fact]
        public async Task Map_Other_RepositoryError()
        {
            var mockValidationService = GetMockIsbnValidator(true);
            var mockRepository = GetMockBookRepo(hasError: true);
            var sub = new LibraryManagementService(mockRepository, mockValidationService);

            var book = new Book(ISBN)
            {
                Author = "John_1",
                Title = "Anything will do",
                Description = "This is John's first book."
            };

            // Add
            var addBookResult = await sub.AddBook(book);
            Assert.False(addBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.REPOSITORY_ERROR, addBookResult.ErrorCode);

            // Get one
            var getBookResult = await sub.GetBook(ISBN);
            Assert.False(getBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.REPOSITORY_ERROR, getBookResult.ErrorCode);

            // Update
            var updateBookResult = await sub.UpdateBook(book);
            Assert.False(updateBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.REPOSITORY_ERROR, updateBookResult.ErrorCode);
        }

        [Fact]
        public async Task Map_Other_ServiceError()
        {
            var mockValidationService = GetMockIsbnValidator(true);
            var mockRepository = GetMockBookRepo(throwError: true);
            var sub = new LibraryManagementService(mockRepository, mockValidationService);

            var book = new Book(ISBN)
            {
                Author = "John_1",
                Title = "Anything will do",
                Description = "This is John's first book."
            };

            // Add
            var addBookResult = await sub.AddBook(book);
            Assert.False(addBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.OTHER_ERROR, addBookResult.ErrorCode);

            // Get one
            var getBookResult = await sub.GetBook(ISBN);
            Assert.False(getBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.OTHER_ERROR, getBookResult.ErrorCode);

            // Update
            var updateBookResult = await sub.UpdateBook(book);
            Assert.False(updateBookResult.IsSuccess);
            Assert.Equal(LibraryManagementService.OTHER_ERROR, updateBookResult.ErrorCode);
        }
    }
}