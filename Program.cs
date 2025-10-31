using Serilog; // Para Log.Logger, ForContext, LoggerConfiguration


// --- CONFIGURAÇÃO DO SERILOG (O ROTEADOR) ---
// Configura o Log.Logger estático
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext() // Importante para o ForContext<T>()

    // --- Sub-Logger A: Log de Desenvolvedor (Console e Seq) ---
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding("LogType = 'UserAction'") // Filtra
        .WriteTo.Console(
            outputTemplate: "{Timestamp:HH:mm:ss} [DEV] [{Level:u3}] {Message:l} {Properties:j}{NewLine}{Exception}")
        // .WriteTo.Seq("http://localhost:5341")
    )

    // --- Sub-Logger B: Log de Ações do Usuário (SQLite) ---
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly("LogType = 'UserAction'") // Filtra
        .MinimumLevel.Information()
        .WriteTo.SQLite(
            sqliteDbPath: "../../../logs/LogDeAcoes.db",
            tableName: "Logs")
    )
    .CreateLogger();

Log.Information("--- Serilog estático iniciado (SEM HOST). ---");

try
{
    // --- EXECUÇÃO DA SIMULAÇÃO (SEM HOST) ---
    Console.WriteLine("\n--- Iniciando Simulação ---");

    // 1. Não há 'Host' ou 'DI'. Criamos o serviço manualmente.
    var servico = new ServicoDeSimulacao();
    
    // 2. Executamos o método
    servico.ExecutarSimulacao();
    
    Console.WriteLine("--- Simulação Concluída ---");

}
catch (Exception ex)
{
    Log.Fatal(ex, "--- Aplicação falhou ao executar. ---");
}
finally
{
    // --- FINALIZAÇÃO (MUITO IMPORTANTE) ---
    Log.Information("--- Aplicação desligando. Fechando log. ---");
    Log.CloseAndFlush();
    Console.WriteLine("\nLogs finalizados. Pressione qualquer tecla para sair.");
    Console.ReadKey();
}


// --- NOSSA CLASSE DE SIMULAÇÃO (MODIFICADA) ---
public class ServicoDeSimulacao
{
    // 1. Usamos a interface ILogger do *Serilog*, não da Microsoft
    private readonly Serilog.ILogger _logger;

    // 2. O construtor está vazio, mas cria seu próprio logger
    public ServicoDeSimulacao()
    {
        // 3. Criamos um logger com o contexto desta classe
        // Isto garante que o "SourceContext" (nome da classe)
        // seja adicionado aos logs de dev, como antes.
        _logger = Log.ForContext<ServicoDeSimulacao>();
    }

    public void ExecutarSimulacao()
    {
        // Daqui para baixo, o código é IDÊNTICO ao anterior,
        // pois _logger (do Serilog) e ILogger<T> (da Microsoft)
        // têm os mesmos nomes de método.
        
        // --- LOG DE DEV (Vai para o Console e Seq) ---
        _logger.Debug("Iniciando o serviço de simulação...");
        _logger.Information("Verificando configurações do sistema.");
        
        // --- LOG DE AÇÃO (Vai APENAS para o SQLite) ---
        // 1. Criamos o logger "etiquetado"
        var loggerDeAcao = _logger.ForContext("LogType", "UserAction");

        // 2. Usamos o logger etiquetado
        loggerDeAcao.Information(
            "Login de usuário. {Usuario} {IP}", 
            "ana.silva", "192.168.1.1");

        // --- LOG DE DEV (Vai para o Console e Seq) ---
        _logger.Debug("Procurando dados do usuário 'ana.silva' no cache...");

        // --- LOG DE AÇÃO (Vai APENAS para o SQLite) ---
        loggerDeAcao.Information(
            "Produto adicionado ao carrinho. {Usuario} {ProdutoId} {Quantidade}", 
            "bruno.costa", 456, 2);

        // --- LOG DE ERRO (Vai para o Console e Seq) ---
        try
        {
            throw new InvalidOperationException("Falha simulada de conexão com API externa.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao tentar processar pagamento.");
        }
    }
}