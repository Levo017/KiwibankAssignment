using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibMgmt.Services
{
    public interface IIsbnValidator
    {
        public bool IsValid(string isbn);
    }

    public class IsbnValidator : IIsbnValidator
    {
        private const string ISBN_REGEX = "(ISBN[-]*(1[03])*[ ]*(: ){0,1})*(([0-9Xx][- ]*){13}|([0-9Xx][- ]*){10})"; // Check for ISBN 10 or 13 format. From: https://regexlib.com/Search.aspx?k=ISBN&c=-1&m=-1&ps=20
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
