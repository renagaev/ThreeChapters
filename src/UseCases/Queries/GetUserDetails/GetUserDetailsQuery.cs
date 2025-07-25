using MediatR;

namespace UseCases.Queries.GetUserDetails;

public record GetUserDetailsQuery(long UserId) : IRequest<UserDetailsDto>;