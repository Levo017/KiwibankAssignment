using LibMgmt.Models;
using LibMgmt.Repositories;
using LibMgmt.Services;
using LibMgmt.UI;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static IIsbnValidator _isbnValidator;

    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IBookRepo, InMemoryBookRepo>();
        services.AddSingleton<ILibraryManagementService, LibraryManagementService>();
        services.AddSingleton<IIsbnValidator, IsbnValidator>(); 

        var serviceProvider = services.BuildServiceProvider();
        var libService = serviceProvider.GetService<ILibraryManagementService>()!;
        _isbnValidator = serviceProvider.GetService<IIsbnValidator>()!;

        var userChoice = default(char);
        while (userChoice != '9')
        {
            UiHelper.ShowMenu();

            if (!char.TryParse(Console.ReadLine(), out userChoice) || !"123459".Contains(userChoice))
            {
                UiHelper.ShowInvalidInput();
            }
            else
            {
                switch (userChoice)
                {
                    case '1':
                        // Add new book
                        var bookToCreate = UiHelper.CreateBookFromUserInput(_isbnValidator);
                        if (bookToCreate == null)
                        {
                            break;
                        }
                        var addResult =  await libService.AddBook(bookToCreate);
                        if (addResult.IsSuccess)
                        {
                            UiHelper.ShowSuccess(nameof(libService.AddBook));
                            break;
                        }

                        UiHelper.ShowError(nameof(libService.AddBook), addResult.ErrorCode);
                        break;
                    case '2':
                        // Update exisiting book
                        var bookToUpdate = UiHelper.CreateBookFromUserInput(_isbnValidator);
                        if (bookToUpdate == null)
                        {
                            break;
                        }
                        var updateBookResult = await libService.UpdateBook(bookToUpdate);
                        if (updateBookResult.IsSuccess)
                        {
                            UiHelper.ShowSuccess(nameof(libService.UpdateBook));
                            break;
                        }
                        UiHelper.ShowError(nameof(libService.UpdateBook), updateBookResult.ErrorCode);
                        break;
                    case '3':
                        var isbn = UiHelper.ReadBookProperty(nameof(Book.ISBN));
                        var deleteBookResult =  await libService.DeleteBook(isbn);
                        if (deleteBookResult.IsSuccess)
                        {
                            UiHelper.ShowSuccess(nameof(libService.DeleteBook));
                            break;
                        }

                        UiHelper.ShowError(nameof(libService.DeleteBook), deleteBookResult.ErrorCode);
                        break;
                    case '4':
                        // List all books
                        var listBooksResult = await libService.ListAllBooks();
                        if (listBooksResult.IsSuccess)
                        {
                            UiHelper.ShowSuccess(nameof(libService.ListAllBooks));
                            UiHelper.PresentBooks(listBooksResult.Result);
                            break;
                        }

                        UiHelper.ShowError(nameof(libService.ListAllBooks), listBooksResult.ErrorCode);
                        break;
                    case '5':
                        // view a book
                        isbn = UiHelper.ReadBookProperty(nameof(Book.ISBN));
                        var getBookResult = await libService.GetBook(isbn);
                        if (getBookResult.IsSuccess)
                        {
                            UiHelper.ShowSuccess(nameof(libService.GetBook));
                            UiHelper.PresentBook(getBookResult.Result);
                            break;
                        }

                        UiHelper.ShowError(nameof(libService.GetBook), getBookResult.ErrorCode);
                        break;
                }
            }
        }
    }
}