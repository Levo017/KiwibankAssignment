using LibMgmt.Models;
using LibMgmt.Repositories;

namespace LibMgmt.Tests
{
    public class BookRepoTests : IDisposable
    {
        public enum Actions {
            update,
            delete,
            get
        }

        private const string ISBN_1 = "978-4-7405-2824-6";
        private const string ISBN_2 = "978-8-9498-6413-6";
         
        [Fact]
        public async Task CRUD_Functions_Success()
        {
            var expectedTitle = "Anything will do";

            var book1 = new Book(ISBN_1)
            {
                Author = "John_1",
                Title = expectedTitle,
                Description = "This is John's first book."
            };

            var book2 = new Book(ISBN_2)
            {
                Author = "Jane_1",
                Title = expectedTitle,
                Description = "This is Jane's first book."
            };

            var sub = new InMemoryBookRepo();

            // Add
            var bookAdded1 = (await sub.Add(book1)).Result;
            Assert.Equal(book1, bookAdded1);

            var bookAdded2 = (await sub.Add(book2)).Result;
            Assert.Equal(book2, bookAdded2);

            // Get All
            var currentBooks = (await sub.GetAll()).Result;
            Assert.NotNull(currentBooks);
            Assert.Equal(2, currentBooks.Count());
            Assert.Contains(book1, currentBooks);
            Assert.Contains(book2, currentBooks);

            // Get one
            var bookToFind = (await sub.GetByKey(ISBN_2)).Result;
            Assert.NotNull(bookToFind);
            Assert.Equal(book2, bookToFind);

            // Update
            expectedTitle = "Just anything";
            var bookToUpdate = book1;
            bookToUpdate.Title = expectedTitle;

            var bookUpdated = (await sub.Update(bookToUpdate)).Result;
            Assert.Equal(bookToUpdate, bookUpdated);

            bookToFind = (await sub.GetByKey(bookUpdated.ISBN)).Result;
            Assert.NotNull(bookToFind);
            Assert.Equal(bookUpdated, bookToFind);

            // Delete
            var bookDeleteResult = (await sub.Delete(book2.ISBN)).Result;
            Assert.True(bookDeleteResult);

            currentBooks = (await sub.GetAll()).Result;
            Assert.NotNull(currentBooks);
            Assert.Single(currentBooks);
            Assert.DoesNotContain(book2, currentBooks);
        }

        [Fact]
        public async Task Duplicate_Key_Insert() 
        {
            var sub = new InMemoryBookRepo();

            var expectedTitle = "Anything will do";

            var book1 = new Book(ISBN_1)
            {
                Author = "John_1",
                Title = expectedTitle,
                Description = "This is John's first book."
            };

            var book2 = new Book(ISBN_1) // same ISBN / Key as book1, expects error
            {
                Author = "Jane_1",
                Title = expectedTitle,
                Description = "This is Jane's first book."
            };

            // Add
            var bookAdded1 = (await sub.Add(book1)).Result;
            Assert.Equal(book1, bookAdded1);

            var bookAdded2Result = await sub.Add(book2);
            Assert.Null(bookAdded2Result.Result);
            Assert.Equal(RepositoryErrors.KeyDuplicate, bookAdded2Result.Error);
        }

        [Theory]
        [InlineData(Actions.get)]
        [InlineData(Actions.update)]
        public async Task NonExist_Key(Actions action)
        {
            var sub = new InMemoryBookRepo();

            RepositoryResult<Book?> result;

            if (action == Actions.get)
            {
                result = await sub.GetByKey(ISBN_1);
            } else { 
                var bookToUpdate = new Book(ISBN_1);
                result = await sub.Update(bookToUpdate);
            }

            Assert.Null(result.Result);
            Assert.Equal(RepositoryErrors.KeyNotExist, result.Error);
        }

        public void Dispose()
        {
        }
    }
}
