using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Users.EditProfile;

public sealed class EditProfileHandler(IDataContext db, TimeProvider time) : ICommandHandler<EditProfileCommand>
{
    public async Task<Result> Handle(EditProfileCommand req, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == req.UserId, ct);

        return user.EnsureFound(ErrorResults.NotFound("User"))
            .Bind(u => Apply(u, req, time.GetUtcNow().UtcDateTime));
    }

    private static Result Apply(User user, EditProfileCommand req, DateTime now)
    {
        var results = new List<Result>();

        if (!string.IsNullOrWhiteSpace(req.FullName))
            results.Add(user.Rename(req.FullName));

        return ResultExtensions.FirstFailureOrSuccess(results.ToArray());
    }
}