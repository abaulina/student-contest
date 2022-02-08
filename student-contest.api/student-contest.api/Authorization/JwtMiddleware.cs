using student_contest.api.Services;

namespace student_contest.api.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var containsId = int.TryParse(context.Request.Path.ToString().Split('/').Last(), out var requestedId);
            var userId = jwtUtils.ValidateToken(token);
            
            if (containsId && userId==requestedId)
            {
                context.Items["User"] = userService.GetCurrentUserInfo(userId.Value);
            }

            await _next(context);
        }
    }
}
