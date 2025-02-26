using MediatR;
using Microsoft.AspNetCore.Mvc;
using UseCases.Queries.GetBibleStructure;

namespace Controllers;

[Route("api/v1/bible")]
public class BibleController(ISender sender) : ControllerBase
{
    [HttpGet(Name = "getStructure")]
    public async Task<ICollection<StructureTestament>> GetStructure(CancellationToken cancellationToken)
    {
        return await sender.Send(new GetBibleStructureQuery(), cancellationToken);
    }
}