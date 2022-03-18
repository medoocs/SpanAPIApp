using System.ComponentModel.DataAnnotations;

namespace SpanAPI
{
    public class Person
    {
        public Person(string name, string lastName, int postalCode, string city, string number)
        {
            Name = name;
            LastName = lastName;
            PostalCode = postalCode;
            City = city;
            Number = number;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(16)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(16)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public int PostalCode { get; set; }
        [Required]
        [StringLength(32)]
        public string City { get; set; } = string.Empty;
        [Required]
        [StringLength(16)]
        public string Number { get; set; } = string.Empty;

    }
}
