using System.Text.Json;
using System.Text.Json.Serialization;

// Este é o "molde" que representa uma linha do nosso ficheiro de log JSON
public class LogEntry
{
    // O atributo [JsonPropertyName] diz ao C# para ligar a
    // propriedade "Timestamp" ao campo "@t" do JSON.
    [JsonPropertyName("@t")]
    public DateTime Timestamp { get; set; }

    // Liga "Level" ao campo "@l"
    [JsonPropertyName("@l")]
    public string Level { get; set; }

    // Liga "MessageTemplate" ao campo "@mt"
    [JsonPropertyName("@mt")]
    public string MessageTemplate { get; set; }

    // Liga "Exception" ao campo "@x"
    [JsonPropertyName("@x")]
    public string Exception { get; set; }

    // ESTA É A PARTE MAIS IMPORTANTE!
    // [JsonExtensionData] é um "saco mágico".
    // Ele captura TODAS as outras propriedades do JSON que não definimos acima
    // (como "NomeUtilizador", "TempoExecucaoMs", e o nosso objeto "Dados").
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; set; }
}