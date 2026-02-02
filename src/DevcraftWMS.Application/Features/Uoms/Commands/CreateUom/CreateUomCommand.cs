using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Commands.CreateUom;

public sealed record CreateUomCommand(string Code, string Name, UomType Type) : IRequest<RequestResult<UomDto>>;
