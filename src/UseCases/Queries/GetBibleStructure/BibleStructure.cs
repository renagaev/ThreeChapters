using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UseCases.Queries.GetBibleStructure;

[DisplayName("StructureBook")]
public record StructureBook([Required] int Id, [Required] string Title, [Required] int ChaptersCount);

[DisplayName("BookGroup")]
public record BookGroup([Required] string Title, [Required] ICollection<StructureBook> Books);

[DisplayName("StructureTestament")]
public record StructureTestament([Required] string Title, [Required] ICollection<BookGroup> Groups);