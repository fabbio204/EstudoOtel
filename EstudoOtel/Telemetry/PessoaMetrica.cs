using System.Diagnostics.Metrics;

namespace EstudoOtel.Telemetry
{
    public class PessoaMetrica
    {
        private readonly Counter<int> _cadastrosRealizados;
        public const string METRICA_PESSOA = "PessoaMetrica";

        public PessoaMetrica(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create(METRICA_PESSOA, "1.0.0");

            _cadastrosRealizados = meter.CreateCounter<int>(name: "pessoa_cadastros_realizados",
                unit: "cadastros",
                description: "Número de cadastros de pessoas realizados.");
        }

        public void RegistrarCadastro()
        {
            _cadastrosRealizados.Add(1);
        }
    }
}
