using MyPersonalDiary.Server.Repositories;

namespace MyPersonalDiary.Server.Middleware
{
    public class RequireAdminMiddleware
    {
        private readonly RequestDelegate _next;

        public RequireAdminMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
        {
            var publicId = context.Session.GetString("PublicId");

            if (string.IsNullOrEmpty(publicId))
            {
                context.Response.StatusCode = 401;

                await context.Response.WriteAsync("Unauthorized.");

                return;
            }

            var user = await userRepository.GetUserByPublicIdAsync(publicId);

            if (user == null || user.Role != Models.UserRole.Admin)
            {
                context.Response.StatusCode = 403;

                await context.Response.WriteAsync("Your role is not admin. Access denied.");

                return;
            }

            await _next(context);
        }
    }
}
