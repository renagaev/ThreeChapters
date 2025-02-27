using MediatR;

namespace UseCases.Queries.GetUserDetails;

public record GetUserDetailsQuery(int UserId) : IRequest<UserDetailsDto>;