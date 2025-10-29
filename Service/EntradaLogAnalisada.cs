public class EntradaLogAnalisada
{
    /// <summary>
    /// O texto completo da linha de log original.
    /// </summary>
    public required string LinhaCompleta { get; set; }

    /// <summary>
    /// O Nível de Log extraído (ex: "INF", "ERR", "WRN").
    /// </summary>
    public string? Nivel { get; set; }

    /// <summary>
    /// As propriedades estruturadas, convertidas do JSON.
    /// Usamos Dictionary para armazenar chave/valor (ex: "Usuario", "ana.silva").
    /// </summary>
    public Dictionary<string, object>? Propriedades { get; set; }
}