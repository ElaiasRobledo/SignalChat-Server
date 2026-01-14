using Application.Common.Interfaces.Utils;

namespace SignalChat_Server.Utils
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _context;
        public CurrentUserService(IHttpContextAccessor context)
        {
            _context = context;
        }
        public Guid UserId
        {
            get 
            {
                var id = _context.HttpContext?.User.Claims.First().Value;
                return Guid.TryParse(id, out var userId) ? userId : Guid.Empty;
            }
        }
    }
}
