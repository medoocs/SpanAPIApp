using System.ComponentModel.DataAnnotations;

namespace SpanAPI
{
    public record PersonDto([Required, StringLength(16)] string Name, [Required, StringLength(16)] string LastName, [Required] int PostalCode, [Required, StringLength(32)] string City, [Required, StringLength(16)] String Number);
    public record PersonCsvDto([Required, StringLength(16)] string Name, [Required, StringLength(16)] string LastName, [Required, StringLength(16)] string PostalCode, [Required, StringLength(32)] string City, [Required, StringLength(16)] String Number, bool correctFormat);
}
