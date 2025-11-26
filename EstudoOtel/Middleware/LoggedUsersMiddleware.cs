using EstudoOtel.Telemetry;

namespace EstudoOtel.Middleware
{
    public class LoggedUsersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UsuarioMetrica _usuarioMetrica;

        public LoggedUsersMiddleware(RequestDelegate next, UsuarioMetrica usuarioMetrica)
        {
            _next = next;
            _usuarioMetrica = usuarioMetrica;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(context.User?.Identity.Name)) { 

                _usuarioMetrica.AtualizarUsuario(context.User.Identity.Name!);
            }

            await _next(context);
        }
    }
}