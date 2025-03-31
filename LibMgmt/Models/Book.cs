using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMgmt.Models
{
    public class Book
    {
        public string ISBN { get;  }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        public Book(string isbn) { ISBN = isbn; }

        public override string ToString()
        {
            return $"Title: {Title} \\n ISBN: {ISBN} \\n Author: {Author} \\n {Description}";
        }
    }
}
