using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Users.DeleteProfile;

public sealed class DeleteProfileHandler(IDataContext db) : ICommandHandler<DeleteProfileCommand>
{
    public async Task<Result> Handle(DeleteProfileCommand req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == req.UserId, ct);

        return user.EnsureFound(ErrorResults.NotFound("User"))
            .Tap(u => db.Users.Remove(u))
            .ToResult();
    }
}