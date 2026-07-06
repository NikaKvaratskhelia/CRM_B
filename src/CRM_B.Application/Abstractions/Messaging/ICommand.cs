using CRM_B.Domain.Kernel.Results;
using MediatR;

namespace CRM_B.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;