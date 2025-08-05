using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentApi.Controllers
{
    /// <summary>
    /// Base controller that provides access to MediatR for all derived API controllers.
    /// </summary>
    /// <remarks>
    /// This controller serves as the base for all versioned API controllers and sets up the MediatR mediator 
    /// through dependency injection using the current HTTP request scope.
    /// </remarks>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;

        /// <summary>
        /// Provides access to the MediatR mediator instance from the current request services.
        /// </summary>
        protected IMediator Mediator => _mediator ??= HttpContext!.RequestServices.GetService<IMediator>()!;
    }
}
// This code defines a base API controller for an ASP.NET Core application that uses MediatR for handling requests.