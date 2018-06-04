using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string UserName { get; set; }
        public string password { get; set; }
        public int Age { get; set; }
        public string Qualification { get; set; }
    }
}