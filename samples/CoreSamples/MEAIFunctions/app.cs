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

ChatOptions options = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(GetTheWeather)
    ]
};

IChatClient client = OpenAIChatClientFactory.Create(config)
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var question = "Do I need an umbrella today?";
Console.WriteLine($"question: {question}");
var response = await client.GetResponseAsync(question, options);
Console.WriteLine($"response: {response}");


[Description("Get the weather")]
static string GetTheWeather()
{
    var temperature = Random.Shared.Next(5, 20);
    var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";
    var weatherInfo = $"The weather is {temperature} degrees C and {conditions}.";
    Console.WriteLine($"\tFunction Call - Returning weather info: {weatherInfo}");
    return weatherInfo;
}
