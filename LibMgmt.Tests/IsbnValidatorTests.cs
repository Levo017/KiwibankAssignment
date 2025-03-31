using LibMgmt.Services;

namespace LibMgmt.Tests
{
    public class IsbnValidatorTests
    {
        [Theory]
        [InlineData("978-7-6499-1995-5", true)]
        [InlineData("978-7-6499-xxxx-5", false)]
        public async Task Validate_ISBN13(string toValidate, bool expectation)
        {
            var sub = new IsbnValidator();
            var result = sub.IsValid(toValidate);
            Assert.Equal(expectation, result);
        }
    }
}
