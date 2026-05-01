# BasicChat-10StreamingConversationHistory

This sample demonstrates how to keep conversation history while streaming responses with `GetStreamingResponseAsync()` from Microsoft.Extensions.AI.

## What This Sample Shows

The code in [app.cs](./app.cs) keeps a single `List<ChatMessage>` named `chatHistory` for the whole console session. Each time you type `conversation`, the app:

1. Adds the user's question to `chatHistory`.
2. Streams the assistant response update by update.
3. Writes each streamed text chunk to the console as it arrives.
4. Adds the completed assistant response back into `chatHistory`.

That final step is what lets the model answer later questions with context from earlier turns.

## Streaming Conversation Pattern

The sample starts the history with a system message:

```csharp
List<ChatMessage> chatHistory =
[
    new ChatMessage(ChatRole.System, "You are a good assistance with short and smart answers")
];
```

When the user enters a question, the app adds it to the same history list:

```csharp
chatHistory.Add(new(ChatRole.User, Console.ReadLine()));
```

The response is streamed with `GetStreamingResponseAsync(chatHistory)`. Each `ChatResponseUpdate` contains the next part of the assistant response:

```csharp
List<ChatResponseUpdate> updates = [];
await foreach (ChatResponseUpdate update in client.GetStreamingResponseAsync(chatHistory))
{
    Console.Write(update.Text);
    updates.Add(update);
}
```

After streaming finishes, the sample uses `AddMessages(updates)` to convert the collected streaming updates into assistant message history:

```csharp
chatHistory.AddMessages(updates);
```

## Why `AddMessages(updates)` Matters

Streaming APIs return a sequence of partial response updates instead of one complete `ChatResponse`. If you only write the streamed text to the console, the user can see the answer, but the next model request will not remember it.

`AddMessages(updates)` appends the assistant response represented by the streamed updates to `chatHistory`, so the next call to `GetStreamingResponseAsync(chatHistory)` includes both the user turns and assistant turns from the current session.

## Configuration

The app uses the shared OpenAI chat client factory from [../OpenAIChatClientShared](../OpenAIChatClientShared). Configure these values through `appsettings.local.json`, user secrets, or environment variables:

```json
{
  "OpenAI": {
    "Endpoint": "https://api.openai.com/v1",
    "ApiKey": "<your-api-key>",
    "Model": "gpt-5-mini"
  }
}
```

Do not commit real API keys. For local development, prefer user secrets or an ignored `appsettings.local.json` file.

## Running This Example

From this folder, run:

```bash
dotnet run app.cs
```

Then type `conversation` to send a prompt. Repeat `conversation` to keep chatting with the same in-memory history, or press any other key to exit.

## Streaming vs. Non-Streaming History

Use this pattern for streaming responses:

```csharp
List<ChatResponseUpdate> updates = [];
await foreach (ChatResponseUpdate update in client.GetStreamingResponseAsync(chatHistory))
{
    Console.Write(update.Text);
    updates.Add(update);
}

chatHistory.AddMessages(updates);
```

For non-streaming responses, see [../BasicChat-10ConversationHistory](../BasicChat-10ConversationHistory), which uses `GetResponseAsync()` and adds the completed assistant message after the response is returned.

## Common Mistakes to Avoid

- Do not create a new history list for each prompt if you want multi-turn context.
- Do not forget to add the streamed assistant response back into history after streaming completes.
- Do not add a `ChatResponseUpdate` directly to `List<ChatMessage>`; collect the updates and call `chatHistory.AddMessages(updates)`.

## See Also

- [Microsoft.Extensions.AI documentation](https://learn.microsoft.com/dotnet/ai/ai-extensions?wt.mc_id=academic-105485-koreyst)
- [BasicChat-10ConversationHistory](../BasicChat-10ConversationHistory) - Non-streaming conversation history
- [OpenAIChatClientShared](../OpenAIChatClientShared) - Shared OpenAI client configuration used by this sample
