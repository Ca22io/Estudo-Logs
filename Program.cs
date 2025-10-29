using System.Text.Json;
using Serilog;
using Serilog.Formatting.Compact; // Precisamos disto para o formatador JSON

public class Program
{
    public static void Main(string[] args)
    {
        // 1. CONFIGURAÇÃO DO LOGGER
        // Usamos LoggerConfiguration para construir o nosso logger
        // Precisamos disto para o CompactJsonFormatter

        // 1. CONFIGURAÇÃO DO LOGGER (Versão Corrigida)
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()

            // Sink da Consola (este já estava correto)
            // .WriteTo.Console(new CompactJsonFormatter())

            // SINK DO FICHEIRO (AQUI ESTÁ A CORREÇÃO)
            .WriteTo.File(
                formatter: new CompactJsonFormatter(), // <-- 1. USAMOS O FORMATTER JSON
                path: "../../../logs/meu-log-.json",           // <-- 2. MUDAMOS A EXTENSÃO para .json
                rollingInterval: RollingInterval.Day   // <-- 3. Removemos o 'outputTemplate'
            )

            .Enrich.FromLogContext()
            .CreateLogger();

        // 2. USAR O LOGGER
        // Envolvemos o nosso código num try/catch/finally
        try
        {
            Log.Information("Aplicação está a iniciar..."); // Log simples

            // Exemplo de log estruturado com propriedades
            string userName = "Alice";
            int tempoMs = 125;
            Log.Information("Utilizador {NomeUtilizador} demorou {TempoExecucaoMs}ms a carregar a página.", userName, tempoMs);

            // Exemplo de log estruturado com um objeto (o '@' é a magia!)
            var dadosUtilizador = new
            {
                Id = 123,
                Email = "alice@exemplo.com",
                Carrinho = new { Itens = 5, Total = 150.99 }
            };
            Log.Warning("Utilizador {@Dados} tentou aceder a uma zona restrita.", dadosUtilizador);

            // Exemplo de um erro
            try
            {
                int a = 10;
                int b = 0;
                int resultado = a / b;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ocorreu uma divisão por zero!");
            }

            Log.Information("Aplicação a terminar.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "A aplicação falhou de forma inesperada!");
        }
        finally
        {
            // 3. FECHAR E DESCARREGAR (Flush)
            // Isto é MUITO importante. Garante que todos os logs são escritos
            // antes de a aplicação fechar.
            Log.CloseAndFlush();
        }

        // 1. Define o caminho para o ficheiro de log
        // (Muda isto para o nome exato do teu ficheiro!)
        string logFilePath = "logs/meu-log-20251029.json"; // ATENÇÃO AQUI

        Console.WriteLine($"A analisar o ficheiro: {logFilePath}...");
        Console.WriteLine("A procurar por 'Alertas de Acesso'...");
        Console.WriteLine("---");

        // 2. Verifica se o ficheiro existe
        if (!File.Exists(logFilePath))
        {
            Console.WriteLine($"ERRO: Ficheiro de log não encontrado em '{logFilePath}'");
            return;
        }

        // 3. Lê o ficheiro linha a linha (eficiente em termos de memória)
        foreach (string jsonLine in File.ReadLines(logFilePath))
        {
            // Ignora linhas em branco que possam existir
            if (string.IsNullOrWhiteSpace(jsonLine))
            {
                continue;
            }

            try
            {
                // 4. Deserializa o texto JSON para o nosso objeto LogEntry
                var log = JsonSerializer.Deserialize<LogEntry>(jsonLine);

                if (log == null) continue;

                // 5. FILTRA E FORMATA!
                // Verifica se este é o log que procuramos
                if (log.MessageTemplate == "Utilizador {@Dados} tentou aceder a uma zona restrita.")
                {
                    // Encontrámos! Vamos formatar uma mensagem legível.
                    string mensagemLegivel = FormatarAlertaDeAcesso(log);
                    Console.WriteLine(mensagemLegivel);
                }
                
                // Podes adicionar mais 'if' ou 'switch' aqui para formatar outros tipos de log

            }
            catch (JsonException ex)
            {
                // Apanha erros se uma linha não for JSON válido
                Console.WriteLine($"[AVISO] Ignorando linha com JSON inválido: {ex.Message}");
            }
        }

        Console.WriteLine("---");
        Console.WriteLine("Análise terminada.");
    }

    /// <summary>
    /// Recebe um LogEntry e formata-o numa string legível para humanos.
    /// </summary>
    public static string FormatarAlertaDeAcesso(LogEntry log)
    {
        try
        {
            // 1. Formata a Data/Hora
            string dataHora = log.Timestamp.ToLocalTime().ToString("dd/MM/yyyy 'às' HH:mm:ss");

            // 2. Extrai os dados de dentro da propriedade "Dados"
            // Usamos o "saco mágico" Properties e navegamos dentro dele
            if (log.Properties != null && log.Properties.TryGetValue("Dados", out JsonElement dadosUtilizador))
            {
                // Navega dentro do objeto "Dados" para apanhar o "Email" e "Id"
                string email = dadosUtilizador.GetProperty("Email").GetString();
                int id = dadosUtilizador.GetProperty("Id").GetInt32();

                // 3. Monta a string final
                return $"{dataHora} O usuario Email: {email} tentou acessar a uma zona restrita.";
            }
        }
        catch (Exception ex)
        {
            // Salvaguarda caso a estrutura do JSON mude
            return $"[ERRO AO FORMATAR LOG] Data: {log.Timestamp.ToLocalTime()} - {ex.Message}";
        }
        
        return $"[ALERTA em {log.Timestamp.ToLocalTime()}] Um utilizador (dados não puderam ser lidos) tentou aceder a uma zona restrita.";
    }
        
}
