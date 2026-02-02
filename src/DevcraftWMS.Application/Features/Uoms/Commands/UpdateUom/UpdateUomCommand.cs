using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Commands.UpdateUom;

public sealed record UpdateUomCommand(Guid Id, string Code, string Name, UomType Type) : IRequest<RequestResult<UomDto>>;
