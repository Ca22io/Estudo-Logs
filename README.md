# 🚀 Projeto de Estudo: Roteamento de Logs com Serilog no .NET 8

Este projeto é um estudo prático sobre a implementação de um sistema de logging robusto em uma Aplicação Console .NET 8, utilizando a biblioteca Serilog. O foco principal é o conceito de **Roteamento de Logs**, onde diferentes tipos de eventos são direcionados para destinos (sinks) específicos com base em seu conteúdo e propósito.

A aplicação simula operações de um sistema (logins, adição de produtos, erros) e encaminha os logs de forma inteligente:
-   **Logs de Diagnóstico (para Desenvolvedores):** Eventos detalhados (`Debug`, `Information`, `Error`) são enviados para o **Console**, permitindo a depuração e monitorização em tempo real.
-   **Logs de Auditoria (Ações do Usuário):** Eventos de negócio específicos (como "Login realizado") são filtrados e guardados em um banco de dados **SQLite** para análise futura.

---

## 📚 Conceitos Estudados e Habilidades Adquiridas

Este projeto proporcionou uma jornada prática pelos seguintes conceitos:

-   **Log Estruturado (Structured Logging):** A principal habilidade adquirida. Em vez de registrar texto simples, aprendemos a registrar mensagens como dados estruturados (ex: `Log.Information("Login. {Usuario}", "ana")`), o que torna a filtragem e análise de logs muito mais poderosa.

-   **Serilog:** Configuração e implementação da biblioteca de logging mais popular do .NET, utilizando seu logger estático (`Log.Logger`).

-   **Sinks (Destinos):** Compreensão de que um único evento de log pode ser enviado para múltiplos destinos simultaneamente (Console, Arquivo, SQLite, etc.).

-   **Roteamento de Logs (Log Routing):** O conceito mais avançado do projeto. Utilizando `WriteTo.Logger()` e filtros, foi criada uma "central de triagem" que envia logs para sinks diferentes com base em suas propriedades.

-   **Filtros de Log:** Uso de expressões (`ByIncludingOnly`, `ByExcluding`) para controlar com precisão o fluxo de logs. A expressão `Filter.ByIncludingOnly("LogType = 'UserAction'")` foi a chave para isolar os logs de auditoria.

-   **Contexto e "Etiquetagem" (Tagging) de Logs:** Aplicação de `.ForContext("Propriedade", "Valor")` para "etiquetar" logs com dados personalizados (neste caso, `LogType = "UserAction"`), permitindo o roteamento eficaz.

-   **Armazenamento de Logs em Banco de Dados:** Implementação do `Serilog.Sinks.SQLite` para guardar logs de forma persistente.

---

## ⚙️ Pré-requisitos

-   **.NET 8 SDK**
-   **(Opcional) Seq:** Uma ferramenta de visualização de logs que pode ser usada para monitorar os logs enviados para o Seq sink.
-   **(Opcional) DB Browser for SQLite:** Uma ferramenta gráfica gratuita para abrir e inspecionar o arquivo de banco de dados `.db` que será gerado.

---

## ▶️ Como Executar o Projeto

1.  Abra um terminal na pasta raiz do projeto.
2.  Restaure os pacotes NuGet necessários:
    ```bash
    dotnet restore
    ```
3.  Execute a aplicação:
    ```bash
    dotnet run
    ```
O programa irá executar a simulação, exibir os logs de desenvolvimento no console e gerar o banco de dados com os logs de ação.

---

## 🖥️ O Que Esperar (Resultados)

Assim que o projeto for executado, você poderá observar o roteamento de logs em dois locais diferentes:

### 1. Resultado no Console (Logs de Desenvolvimento)

Seu terminal exibirá apenas os logs de diagnóstico (`Debug`, `Information` de sistema, `Error`). Note que os logs de "Login" e "Produto adicionado" **não aparecem aqui**, pois foram filtrados.

```
--- Serilog estático iniciado (SEM HOST). ---

--- Iniciando Simulação ---
10:30:01 [DEV] [DBG] Iniciando o serviço de simulação... {"SourceContext": "ServicoDeSimulacao"}
10:30:01 [DEV] [INF] Verificando configurações do sistema. {"SourceContext": "ServicoDeSimulacao"}
10:30:01 [DEV] [DBG] Procurando dados do usuário 'ana.silva' no cache... {"SourceContext": "ServicoDeSimulacao"}
10:30:01 [DEV] [ERR] Erro ao tentar processar pagamento.
System.InvalidOperationException: Falha simulada de conexão com API externa.
   em ServicoDeSimulacao.ExecutarSimulacao() em ...\Program.cs:line ...
 {"SourceContext": "ServicoDeSimulacao"}
--- Simulação Concluída ---
10:30:01 [DEV] [INF] --- Aplicação desligando. Fechando log. --- {}

Logs finalizados. Pressione qualquer tecla para sair.
```

### 2. Resultado no SQLite (Logs de Ação do Usuário)

Este é o resultado mais importante do nosso roteamento.

1.  Na pasta do seu projeto, uma nova pasta `logs/` será criada.
2.  Dentro dela, você encontrará um arquivo chamado `LogDeAcoes.db`.
3.  Abra este arquivo com o DB Browser for SQLite.
4.  Na aba "Procurar dados" (Browse Data), selecione a tabela `Logs`.
5.  Você verá **apenas os logs de Ação do Usuário** que foram filtrados.

| Id  | Timestamp           | Level       | MessageTemplate                                           | Properties (XML)                                                                                                                               |
| --- | ------------------- | ----------- | --------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| 1   | 2025-11-03...       | Information | Login de usuário. {Usuario} {IP}                          | `<properties><property key="Usuario">ana.silva</property>...<property key="LogType">UserAction</property></properties>`                         |
| 2   | 2025-11-03...       | Information | Produto adicionado ao carrinho. {Usuario} {ProdutoId} ... | `<properties><property key="Usuario">bruno.costa</property>...<property key="LogType">UserAction</property></properties>` |

---

## ⭐ Nota Importante: Mapeamento de Propriedades no Banco de Dados

O sink `Serilog.Sinks.SQLite` é excelente para um logging simples, mas ele armazena todas as propriedades personalizadas (`Usuario`, `IP`, `ProdutoId`, etc.) em uma **única coluna de texto (XML ou JSON)**. Isso torna difícil realizar consultas complexas e relacionais (ex: `JOIN` com uma tabela de `Usuarios` ou `COUNT` de produtos por `ProdutoId`).

Para cenários de produção onde consultas avançadas são necessárias, é recomendado utilizar um banco de dados mais robusto como **MySQL** ou **SQL Server**. Com sinks específicos para esses bancos (como `Serilog.Sinks.MSSqlServer` ou `Serilog.Sinks.MySQL`), é possível **mapear cada propriedade do log para uma coluna dedicada na tabela**, o que otimiza drasticamente a performance e a capacidade de análise dos dados.