using LibMgmt.Services.interfaces;
using System.Text.RegularExpressions;

namespace LibMgmt.Services.implementations
{
    public class IsbnValidator : IIsbnValidator
    {
        private const string ISBN_REGEX = "[0-9]*[-| ][0-9]*[-| ][0-9]*[-| ][0-9]*[-| ][0-9]*"; // Check for ISBN 13 (only) format. From: https://regexlib.com/Search.aspx?k=ISBN&c=-1&m=-1&ps=20
        private readonly Regex _isbnRegex;

        public IsbnValidator()
        {
            _isbnRegex = new Regex(ISBN_REGEX, RegexOptions.Compiled);
        }
        public bool IsValid(string isbn)
        {
            return _isbnRegex.IsMatch(isbn);
        }
    }
}
