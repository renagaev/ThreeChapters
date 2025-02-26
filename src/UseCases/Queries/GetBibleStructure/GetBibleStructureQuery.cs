using MediatR;

namespace UseCases.Queries.GetBibleStructure;

public record GetBibleStructureQuery : IRequest<ICollection<StructureTestament>>;