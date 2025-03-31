using LibMgmt.Services;
using LibMgmt.Services.implementations;

namespace LibMgmt.Tests
{
    public class IsbnValidatorTests
    {
        [Theory]
        [InlineData("978-1-86197-876-9", true)]
        [InlineData("978-1-86197-xxx-9 ", false)]
        public async Task Validate_ISBN(string toValidate, bool expectation)
        {
            var sub = new IsbnValidator();
            var result = sub.IsValid(toValidate);
            Assert.Equal(expectation, result);
        }
    }
}
