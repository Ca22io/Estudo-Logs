// using Microsoft.Extensions.Logging;

// public class ServicoDeSimulacao
// {
//     private readonly ILogger<ServicoDeSimulacao> _logger;

//     // O ILogger é injetado automaticamente pelo Host!
//     public ServicoDeSimulacao(ILogger<ServicoDeSimulacao> logger)
//     {
//         _logger = logger;
//     }

//     public void ExecutarSimulacao()
//     {
//         _logger.LogInformation("Teste de JSON. {Usuario} {Valor}", "ana", 123);

//         _logger.LogInformation("Iniciando o processo de simulação do sistema...");

//         // --- Log de Ação do Usuário (Para exibição futura) ---
//         // Usamos "Log Estruturado". Note as variáveis {Usuario} e {IP}.
//         _logger.LogInformation(
//             "Login de usuário realizado com sucesso. {Usuario} {IP}", 
//             "ana.silva", "192.168.1.10");

//         _logger.LogInformation(
//             "Login de usuário realizado com sucesso. {Usuario} {IP}", 
//             "bruno.costa", "10.0.0.5");

//         try
//         {
//             // --- Log de Ação do Usuário (Nível de Debug) ---
//             _logger.LogDebug(
//                 "Usuário está acessando o recurso 'Produtos'. {Usuario} {Recurso}", 
//                 "ana.silva", "/api/produtos");
//             // --- Log de Ação do Usuário (Log de Auditoria) ---
//             _logger.LogInformation(
//                 "Produto adicionado ao carrinho. {Usuario} {ProdutoId} {Quantidade}", 
//                 "ana.silva", 123, 2);

//             // Simula uma operação que pode falhar
//             _logger.LogDebug("Tentando acessar recurso restrito...");
//             ProcessoCriticoQueFalha(10);
//         }
//         catch (Exception ex)
//         {
//             // --- Log de Erro com a Exceção ---
//             // O Serilog captura todos os detalhes da exceção automaticamente.
//             _logger.LogError(ex, "Falha ao executar o processo crítico. {Parametro}", 10);
//         }

//         // --- Log de Aviso (Warning) ---
//         _logger.LogWarning(
//             "Espaço em disco está baixo: {EspacoDisponivelGB}GB", 5);

//         // --- Log de Falha de Segurança (Ação de Usuário) ---
//         _logger.LogWarning(
//             "Tentativa de login falhou. {Usuario} {Motivo}", 
//             "hacker", "Senha inválida");

//         _logger.LogInformation("Processo de simulação finalizado.");
//     }

//     private void ProcessoCriticoQueFalha(int id)
//     {
//         // Simula um erro que ocorre no sistema
//         throw new InvalidOperationException($"O parâmetro {id} não é válido para esta operação.");
//     }
// }