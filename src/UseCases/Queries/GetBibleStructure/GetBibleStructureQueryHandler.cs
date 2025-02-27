using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UseCases.Queries.GetBibleStructure;

public class GetBibleStructureQueryHandler(IDbContext context)
    : IRequestHandler<GetBibleStructureQuery, ICollection<StructureTestament>>
{
    public async Task<ICollection<StructureTestament>> Handle(GetBibleStructureQuery request,
        CancellationToken cancellationToken)
    {
        var books = await context.Books.ToListAsync(cancellationToken);

        return
        [
            new StructureTestament("Ветхий завет", GetTestament(Testament.Old)),
            new StructureTestament("Новый завет", GetTestament(Testament.New))
        ];

        ICollection<BookGroup> GetTestament(Testament testament) => books
            .Where(x => x.Testament == testament)
            .GroupBy(x => x.GroupTitle)
            .Select(x => new BookGroup(x.Key,
                x.OrderBy(x => x.Id).Select(x => new StructureBook(x.Id, x.Title, x.ChaptersCount)).ToList()))
            .OrderBy(x => x.Books.First().Id)
            .ToList();
    }
}