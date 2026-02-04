using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Queries.GetPutawayTaskSuggestions;

public sealed record GetPutawayTaskSuggestionsQuery(Guid Id, int Limit = 5)
    : IRequest<RequestResult<IReadOnlyList<PutawaySuggestionDto>>>;
