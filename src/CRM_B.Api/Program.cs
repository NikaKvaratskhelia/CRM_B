using CRM_B.Api.Extensions.Core;
using CRM_B.Infrastructure.Localization;

// AddCoreServices registers (in order): Application + Infrastructure DI, options, MVC + filters,
// GraphQL, HttpContext-bound services (ICurrentUser, ICorrelationContext, IIdempotencyKeyAccessor),
// JWT auth + authorization policies, rate limiting, CORS, API versioning, OpenTelemetry,
// health checks, Swagger.
// UseCoreApplication composes the pipeline:
//   Swagger (dev) -> CorrelationId -> CORS -> Localization -> Exception handling -> HSTS/HTTPS
//   -> JWT auth -> Rate limiter -> Hangfire dashboard -> Health checks -> Controllers -> GraphQL.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.Services.VerifyErrorResources();

app.UseCoreApplication();

app.Run();

public partial class Program;