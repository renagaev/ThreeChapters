using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UseCases.Queries.GetUserBibleProgress;

[DisplayName("BibleProgressStats")]
public record BibleProgressStats([Required] int ReadTimes, float CurrentPercentage, ICollection<ReadBookChapters> ReadBookChapters);

[DisplayName("ReadBookChapters")]
public record ReadBookChapters([Required] int BookId, [Required] ICollection<int> Chapters);