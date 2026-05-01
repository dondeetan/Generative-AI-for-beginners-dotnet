#:package Microsoft.Extensions.AI@10.3.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.3
#:package Microsoft.Extensions.Configuration.Json@10.0.3
#:project ../OpenAIChatClientShared/OpenAIChatClientShared.csproj

using GenerativeAIForBeginners.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

IChatClient client = OpenAIChatClientFactory.Create(config);

// Create a conversation history list
// This is the CORRECT way to initialize a conversation with a system message
List<ChatMessage> chatHistory =
[
    new ChatMessage(ChatRole.System, "You are a good assistance with short and smart answers")
];

bool loopCheck = true;

while (loopCheck)
{
    Console.WriteLine("Type 'conversation' to start a new conversation or press any key to Exit app");
    var askCommand = Console.ReadLine();

    if (askCommand == "conversation")
    {
        Console.Write("Q: ");
        chatHistory.Add(new(ChatRole.User, Console.ReadLine()));

        List<ChatResponseUpdate> updates = [];
        await foreach (ChatResponseUpdate update in client.GetStreamingResponseAsync(chatHistory))
        {
            Console.Write(update.Text);
            updates.Add(update);
        }
        Console.WriteLine();

        // Add the streamed response to history
        chatHistory.AddMessages(updates);
    }
    else    
    {
        loopCheck = false;
    }
}
