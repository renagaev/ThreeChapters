using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UseCases.Queries.GetUsers;

[DisplayName("UserDto")]
public record UserListItemDto(
    [Required] long Id,
    [Required] string Name,
    [Required] string? Avatar);