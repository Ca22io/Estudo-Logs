using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()

    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding("LogType = 'UserAction'")
        .WriteTo.Console(
            outputTemplate: "{Timestamp:HH:mm:ss} [DEV] [{Level:u3}] {Message:l} {Properties:j}{NewLine}{Exception}")
        // .WriteTo.Seq("http://localhost:5341") // Para logs em tempo real
    )

    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly("LogType = 'UserAction'")
        .MinimumLevel.Information()
        .WriteTo.SQLite(
            sqliteDbPath: "../../../logs/LogDeAcoes.db",
            tableName: "Logs")
    )
    .CreateLogger();

Log.Information("--- Serilog estático iniciado (SEM HOST). ---");

try
{
    Console.WriteLine("\n--- Iniciando Simulação ---");

    var servico = new ServicoDeSimulacao();
    
    servico.ExecutarSimulacao();
    
    Console.WriteLine("--- Simulação Concluída ---");

}
catch (Exception ex)
{
    Log.Fatal(ex, "--- Aplicação falhou ao executar. ---");
}
finally
{
    Log.Information("--- Aplicação desligando. Fechando log. ---");
    Log.CloseAndFlush();
    Console.WriteLine("\nLogs finalizados. Pressione qualquer tecla para sair.");
    Console.ReadKey();
}

public class ServicoDeSimulacao
{
    private readonly Serilog.ILogger _logger;

    public ServicoDeSimulacao()
    {
        _logger = Log.ForContext<ServicoDeSimulacao>();
    }

    public void ExecutarSimulacao()
    {
        _logger.Debug("Iniciando o serviço de simulação...");
        _logger.Information("Verificando configurações do sistema.");
        
        var loggerDeAcao = _logger.ForContext("LogType", "UserAction");

        loggerDeAcao.Information(
            "Login de usuário. {Usuario} {IP}", 
            "ana.silva", "192.168.1.1");

        _logger.Debug("Procurando dados do usuário 'ana.silva' no cache...");

        loggerDeAcao.Information(
            "Produto adicionado ao carrinho. {Usuario} {ProdutoId} {Quantidade}", 
            "bruno.costa", 456, 2);

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