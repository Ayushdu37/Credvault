using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.SetDefaultCard
{
    public record SetDefaultCardCommand(
    Guid UserId,
    Guid CardId
) : IRequest<ApiResponse<bool>>;
}
