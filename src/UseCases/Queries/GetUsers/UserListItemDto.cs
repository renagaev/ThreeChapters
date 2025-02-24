using System.ComponentModel;

namespace UseCases.Queries.GetUsers;

[DisplayName("User")]
public record UserListItemDto(long Id, string Name);