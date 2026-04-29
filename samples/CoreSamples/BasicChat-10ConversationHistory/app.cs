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
List<ChatMessage> conversation = new()
{
    new ChatMessage(ChatRole.System, "You are a good assistance with short and smart answers")
};

bool loopCheck = true;

while (loopCheck)
{
    Console.WriteLine("Type 'conversation' to start a new conversation or press any key to Exit app");
    var askCommand = Console.ReadLine();

    if (askCommand == "conversation")
    {
        // Get user input
        string question = Console.ReadLine() ?? "";
        
        // Add user message to conversation history
        conversation.Add(new ChatMessage(ChatRole.User, question));

        // Get response from the AI model
        var response = await client.GetResponseAsync(conversation);

        // IMPORTANT: The response object has a .Text property, NOT .Messages
        // To add the assistant's response to the conversation history:
        // CORRECT way:
        conversation.Add(new ChatMessage(ChatRole.Assistant, response.Text));

        // INCORRECT way (this will cause a compile error):
        // conversation.Add(response.Messages);  // ❌ response doesn't have .Messages property
        
        Console.WriteLine($"AI: {response.Text}");
    }
    else
    {
        loopCheck = false;
    }
}
