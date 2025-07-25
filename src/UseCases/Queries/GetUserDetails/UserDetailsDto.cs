using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UseCases.Queries.GetUserDetails;

[DisplayName("UserDetails")]
public record UserDetailsDto(
    [Required] long Id, 
    [Required] string Name, 
    [Required] DateOnly MemberFrom, 
    [Required] bool IsActive,
    [Required] string? Avatar);