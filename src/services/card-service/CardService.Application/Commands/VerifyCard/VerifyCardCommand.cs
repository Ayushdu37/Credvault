using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.VerifyCard
{
    public record VerifyCardCommand(
    Guid UserId,
    Guid CardId
) : IRequest<ApiResponse<bool>>;
}
