using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Commands.DeactivateUom;

public sealed record DeactivateUomCommand(Guid Id) : IRequest<RequestResult<UomDto>>;
