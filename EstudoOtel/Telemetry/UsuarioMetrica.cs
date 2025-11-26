using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace EstudoOtel.Telemetry
{
    public class UsuarioMetrica
    {
        private readonly ConcurrentDictionary<string, DateTime> _usuariosLogados;
        public const string METRICA_USUARIO = "UsuarioMetrica";

        public UsuarioMetrica()
        {
            _usuariosLogados = new ConcurrentDictionary<string, DateTime>();
            var meter = new Meter(METRICA_USUARIO, "1.0.0");

            meter.CreateObservableGauge("usuarios_ativos", () => new Measurement<int>(GetUsuariosAtivos()));
        }

        private int GetUsuariosAtivos()
        {
            var threshold = DateTime.UtcNow.AddMinutes(-5);
            return _usuariosLogados.Count(kvp => kvp.Value >= threshold);
        }

        public void AtualizarUsuario(string email)
        {
            _usuariosLogados[email] = DateTime.UtcNow;
        }

        public void RemoverUsuario(string email)
        {
            _usuariosLogados.TryRemove(email, out _);
        }
    }
}
