using System.ComponentModel;

namespace UseCases.Queries.GetUserStreaks;

[DisplayName("ReadStreaks")]
public record ReadStreaksDto(int Current, int Max);