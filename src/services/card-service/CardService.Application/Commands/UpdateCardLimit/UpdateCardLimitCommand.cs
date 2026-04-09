using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.UpdateCardLimit
{
    public record UpdateCardLimitCommand(
    Guid UserId,
    Guid CardId,
    decimal NewCreditLimit
) : IRequest<ApiResponse<bool>>;
}
