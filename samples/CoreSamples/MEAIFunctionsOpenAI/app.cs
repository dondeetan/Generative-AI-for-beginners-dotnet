#:package Microsoft.Extensions.AI@10.3.0
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.3
#:package Microsoft.Extensions.Configuration.Json@10.0.3
#:project ../OpenAIChatClientShared/OpenAIChatClientShared.csproj
#:property UserSecretsId=genai-beginners-dotnet

using GenerativeAIForBeginners.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

IChatClient client = OpenAIChatClientFactory.Create(config)
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

[Description("Get the weather")]
static string GetWeather()
{
    var temperature = Random.Shared.Next(5, 20);
    var condition = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";
    return $"The weather is {temperature} degree C and {condition}";
}

var chatOptions = new ChatOptions
{
    Tools = [AIFunctionFactory.Create(GetWeather)]
};

var funcCallingResponseOne = await client.GetResponseAsync("What is today's date?", chatOptions);
var funcCallingResponseTwo = await client.GetResponseAsync("Why don't you tell me about today's temperature?", chatOptions);
var funcCallingResponseThree = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions);

Console.WriteLine($"Response 1: {funcCallingResponseOne}");
Console.WriteLine($"Response 2: {funcCallingResponseTwo}");
Console.WriteLine($"Response 3: {funcCallingResponseThree}");
