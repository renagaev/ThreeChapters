using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UseCases.Queries.GetUserReadChapters;

[DisplayName("ReadBookChapters")]
public record ReadBookChapters([Required] int BookId, [Required] ICollection<int> Chapters);