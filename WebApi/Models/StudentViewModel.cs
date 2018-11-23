using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiConsumer.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string FirstNameStu { get; set; }
        public string LastNameStu { get; set; }

        public AddressViewModel Address { get; set; }
        public StandardViewModel Standard { get; set; }
    }
}