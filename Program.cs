using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);


// --- 3. Configuração do Serilog (Onde a mágica acontece) ---
var logger = new LoggerConfiguration()
    // Define o nível mínimo. Debug é o mais verboso.
    // Em produção, você talvez queira usar Information ou Warning.
    .MinimumLevel.Debug()
    // Permite "enriquecer" o log com dados extras (como o nome da classe)
    .Enrich.FromLogContext() 
    // Configura o "Sink" (destino) para um arquivo.
    .WriteTo.File(
        path: "../../../logs/log_sistema.log", // Onde salvar
        rollingInterval: RollingInterval.Day, // Cria um novo arquivo todo dia
        retainedFileCountLimit: 7, // Guarda os logs dos últimos 7 dias
        
        // ESTE É O PONTO MAIS IMPORTANTE PARA ANÁLISE:
        // O formato (template) do nosso log em texto.
        // {Properties:j} <-- Isso salva nossas variáveis estruturadas como JSON
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .CreateLogger();

// 4. Limpa os provedores de log padrão (como o Console padrão)
builder.Logging.ClearProviders();
// Adiciona o Serilog como o único provedor de log
builder.Logging.AddSerilog(logger);


// --- 5. Simulação da Aplicação ---
// Podemos registrar "serviços" que fazem o trabalho
builder.Services.AddTransient<ServicoDeSimulacao>();

var host = builder.Build();

// Pega o serviço que criamos e o executa
var servico = host.Services.GetRequiredService<ServicoDeSimulacao>();
servico.ExecutarSimulacao();

// Mensagem final
Console.WriteLine("\nSimulação concluída.");
Console.WriteLine("Verifique a pasta 'logs' para ver o arquivo 'log_sistema.txt'.");
Console.ReadKey();