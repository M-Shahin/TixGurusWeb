using System;

namespace DataAnnotationsExtensions
{
    internal class IntegerAttribute : Attribute
    {
        public string ErrorMessage { get; set; }
    }
}