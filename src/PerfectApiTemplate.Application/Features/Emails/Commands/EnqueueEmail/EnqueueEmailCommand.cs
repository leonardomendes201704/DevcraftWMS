using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Emails.Commands.EnqueueEmail;

public sealed record EnqueueEmailCommand(
    string? From,
    string To,
    string Subject,
    string Body,
    bool IsHtml) : IRequest<RequestResult<EmailMessageDto>>;

