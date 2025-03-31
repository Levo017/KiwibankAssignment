using LibMgmt.Models;
using LibMgmt.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMgmt.UI
{
    public static class UiHelper
    {
        private static readonly ConsoleColor ASK_USER_INPUT_COLOR = ConsoleColor.Yellow;
        private static readonly ConsoleColor USER_INPUT_OPTION_COLOR = ConsoleColor.DarkYellow;
        private static readonly ConsoleColor INPUT_TEXT_COLOR = ConsoleColor.White;
        private static readonly ConsoleColor OUTPUT_STATUS_SUCCESS_COLOR = ConsoleColor.Green;
        private static readonly ConsoleColor OUTPUT_TEXT_COLOR = ConsoleColor.Gray;
        private static readonly ConsoleColor OUTPUT_STATUS_FAIL_COLOR = ConsoleColor.Red;

        internal static void ShowMenu()
        {
            Console.ForegroundColor = ASK_USER_INPUT_COLOR;
            Console.WriteLine("Please choose from following options:");
            Console.ForegroundColor = USER_INPUT_OPTION_COLOR;
            Console.WriteLine("1. Add a new book.");
            Console.WriteLine("2. Update an existing book.");
            Console.WriteLine("3. Delete a book.");
            Console.WriteLine("4. List all books.");
            Console.WriteLine("5. View details of a specific book.");
            Console.WriteLine("9. Exit.");
            Console.ForegroundColor = INPUT_TEXT_COLOR;
        }

        internal static void ShowInvalidInput()
        {
            Console.ForegroundColor = OUTPUT_STATUS_FAIL_COLOR;
            Console.WriteLine("Invalid input, please try again.");
            Console.WriteLine();
            Console.ForegroundColor = INPUT_TEXT_COLOR;
        }

        internal static Book? CreateBookFromUserInput(IIsbnValidator isbnValidator)
        {
            Console.ForegroundColor = ASK_USER_INPUT_COLOR;
            Console.WriteLine("Please input the following info for the book:");
            Console.ForegroundColor = USER_INPUT_OPTION_COLOR;
            Console.WriteLine("ISBN number:");
            Console.ForegroundColor = INPUT_TEXT_COLOR;
            var isbn = Console.ReadLine();
            if (!isbnValidator.IsValid(isbn))
            {
                ShowInvalidInput();
                return null;
            }
            Console.ForegroundColor = OUTPUT_TEXT_COLOR;
            Console.WriteLine($"Book ISBN number: {isbn}");
            Console.WriteLine();

            var title = ReadBookProperty(nameof(Book.Title));
            var author = ReadBookProperty(nameof(Book.Author));
            var description = ReadBookProperty(nameof(Book.Description));

            return new Book(isbn)
            {
                Title = title,
                Author = author,
                Description = description
            };
        }

        internal static string ReadBookProperty(string propertyName)
        {
            Console.ForegroundColor = USER_INPUT_OPTION_COLOR;
            Console.WriteLine($"{propertyName}:");
            Console.ForegroundColor = INPUT_TEXT_COLOR;
            var propertyValue = Console.ReadLine();

            Console.ForegroundColor = OUTPUT_TEXT_COLOR;
            Console.WriteLine($"Book {propertyName}: {propertyValue}");
            Console.WriteLine();
            return propertyValue;
        }

        internal static void PresentBook(Book book)
        {
            Console.ForegroundColor = OUTPUT_TEXT_COLOR;
            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Book Title: {book.Title}");
            Console.WriteLine($"ISBN: {book.ISBN}");
            Console.WriteLine($"Author: {book.Author}");
            Console.WriteLine($"Description: {book.Description}");
            Console.WriteLine();
        }

        internal static void PresentBooks(IEnumerable<Book> books)
        {
            Console.ForegroundColor = OUTPUT_TEXT_COLOR;
            Console.WriteLine($"Book list with {books.Count()} books");
            Console.WriteLine();
            foreach (var book in books)
            {
                PresentBook(book);
            }
        }

        internal static void ShowError(string operationName, long errorCode)
        {
            Console.ForegroundColor = OUTPUT_STATUS_FAIL_COLOR;
            Console.WriteLine($"Operation {operationName} is unsuccessful with error code: {errorCode}.");
        }

        internal static void ShowSuccess(string operationName)
        {
            Console.ForegroundColor = OUTPUT_STATUS_SUCCESS_COLOR;
            Console.WriteLine($"Operation {operationName} is successful.");
        }
    }
}
