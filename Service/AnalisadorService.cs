// using System.Text.Json;
// using Microsoft.Extensions.Logging;

// public class AnalisadorService
// {
//     private const string CaminhoArquivoLog = "logs/log_sistema.txt";
//     private readonly ILogger<AnalisadorService> _logger;
//     private List<EntradaLogAnalisada> _entradasDeLog; // Cache em memória dos logs

//     public AnalisadorService(ILogger<AnalisadorService> logger)
//     {
//         _logger = logger;
//         _entradasDeLog = new List<EntradaLogAnalisada>();
        
//         // No construtor, já lemos e parseamos o arquivo.
//         ParsearArquivoDeLog();
//     }

//     /// <summary>
//     /// Lê o arquivo de log linha por linha e tenta "parsear" (analisar) cada uma.
//     /// </summary>
//     private void ParsearArquivoDeLog()
//     {
//         _logger.LogInformation("Iniciando parse do arquivo de log: {Caminho}", CaminhoArquivoLog);
//         try
//         {
//             // Usamos File.ReadLines para ler o arquivo linha por linha (eficiente em memória)
//             var linhas = File.ReadLines(CaminhoArquivoLog);

//             foreach (var linha in linhas)
//             {
//                 if (string.IsNullOrWhiteSpace(linha)) continue;

//                 var entradaLog = new EntradaLogAnalisada
//                 {
//                     LinhaCompleta = linha,
//                     Nivel = TryParseNivelLog(linha),
//                     Propriedades = TryParsePropriedadesJson(linha)
//                 };
//                 _entradasDeLog.Add(entradaLog);
//             }
//             _logger.LogInformation("Parse de {Contagem} linhas de log concluído.", _entradasDeLog.Count);
//         }
//         catch (FileNotFoundException)
//         {
//             _logger.LogError("Arquivo de log não encontrado em: {Caminho}", CaminhoArquivoLog);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro inesperado ao ler o arquivo de log.");
//         }
//     }

//     /// <summary>
//     /// Tenta extrair o Nível de Log (ex: "INF") da linha.
//     /// Formato esperado: "... [NIV] ..."
//     /// </summary>
//     private string? TryParseNivelLog(string linha)
//     {
//         int inicio = linha.IndexOf('[');
//         int fim = linha.IndexOf(']');
        
//         // Se achou os colchetes e eles estão na ordem correta
//         if (inicio != -1 && fim > inicio && (fim - inicio - 1) == 3)
//         {
//             // Pega o texto entre os colchetes (ex: "INF", "ERR")
//             return linha.Substring(inicio + 1, 3);
//         }
//         return null;
//     }

//     /// <summary>
//     /// Tenta encontrar e analisar o bloco JSON no final da linha de log.
//     /// </summary>
//     private Dictionary<string, object>? TryParsePropriedadesJson(string linha)
//     {
//         // O JSON é a última parte da linha, começando com '{'
//         int inicioJson = linha.IndexOf('{');
        
//         if (inicioJson == -1)
//         {
//             // Não há JSON nesta linha
//             return null;
//         }

//         // Pega a substring do início do JSON até o fim da linha
//         string jsonString = linha.Substring(inicioJson);

//         try
//         {
//             // Usa o System.Text.Json para converter a string JSON em um Dicionário
//             var propriedades = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
//             return propriedades;
//         }
//         catch (JsonException ex)
//         {
//             // A linha continha '{' mas não era um JSON válido (ex: na mensagem de erro)
//             _logger.LogDebug("Falha ao parsear JSON na linha: {Linha}. Erro: {Mensagem}", linha, ex.Message);
//             return null;
//         }
//     }


//     // --- MÉTODOS DE ANÁLISE PÚBLICA ---

//     /// <summary>
//     /// Filtra os logs em memória por um nível específico e os imprime no console.
//     /// </summary>
//     public void AnalisarLogsPorNivel(string nivel)
//     {
//         // A MÁGICA DO LINQ:
//         var logsFiltrados = _entradasDeLog
//             .Where(log => log.Nivel == nivel)
//             .ToList();

//         if (!logsFiltrados.Any())
//         {
//             Console.WriteLine($"Nenhum log encontrado com o nível '{nivel}'.");
//             return;
//         }

//         Console.WriteLine($"Encontrados {logsFiltrados.Count} logs com nível '{nivel}':");
//         foreach (var log in logsFiltrados)
//         {
//             Console.WriteLine($"  -> {log.LinhaCompleta}");
//         }
//     }

//     /// <summary>
//     /// Filtra os logs em memória por uma propriedade de usuário específica.
//     /// </summary>
//     public void AnalisarLogsPorUsuario(string usuario)
//     {
//         // A MÁGICA DO LINQ (Mais avançada):
//         var logsFiltrados = _entradasDeLog
//             .Where(log => log.Propriedades != null && // 1. Garante que há propriedades
//                           log.Propriedades.ContainsKey("Usuario") && // 2. Garante que a chave "Usuario" existe
//                           log.Propriedades["Usuario"]?.ToString() == usuario) // 3. Compara o valor
//             .ToList();

//         if (!logsFiltrados.Any())
//         {
//             Console.WriteLine($"Nenhum log encontrado para o usuário '{usuario}'.");
//             return;
//         }

//         Console.WriteLine($"Encontradas {logsFiltrados.Count} ações para o usuário '{usuario}':");
//         foreach (var log in logsFiltrados)
//         {
//             // Para este relatório, não mostramos a linha inteira,
//             // apenas as partes relevantes (Mensagem + JSON)
            
//             // Encontra a mensagem principal (depois do segundo ']')
//             int inicioMensagem = log.LinhaCompleta.IndexOf(']') + 2; 
//             int fimMensagem = log.LinhaCompleta.IndexOf('{');
//             if (fimMensagem == -1) fimMensagem = log.LinhaCompleta.Length;

//             string mensagem = log.LinhaCompleta.Substring(inicioMensagem, fimMensagem - inicioMensagem).Trim();
            
//             Console.WriteLine($"  -> {mensagem} {log.Propriedades.Values.Last()}");
//         }
//     }
// }