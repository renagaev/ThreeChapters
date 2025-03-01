using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UseCases.Queries.GetUserDailyChaptersRead;

[DisplayName("DayChaptersReadDto")]
public record DayChaptersReadDto([Required] DateOnly Date, [Required] int Count);