#:package Microsoft.Extensions.AI@10.3.0
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.3
#:package Microsoft.Extensions.Configuration.Json@10.0.3
#:package OpenAI@2.8.0

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.Text;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var endpoint = config["OpenAI:Endpoint"];
if (string.IsNullOrWhiteSpace(endpoint))
{
    throw new InvalidOperationException("Set OpenAI:Endpoint in appsettings.local.json. See appsettings.local.json.example.");
}

if (endpoint.Contains("/responses", StringComparison.OrdinalIgnoreCase) ||
    endpoint.Contains("/chat/completions", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("OpenAI:Endpoint must be the base OpenAI-compatible endpoint, such as https://api.openai.com/v1. Do not include /responses or /chat/completions.");
}

var apiKey = config["OpenAI:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException("Set OpenAI:ApiKey in appsettings.local.json. See appsettings.local.json.example.");
}

var model = config["OpenAI:Model"] ?? "gpt-5-mini";

OpenAIClientOptions options = new()
{
    Endpoint = new Uri(endpoint)    
};

IChatClient client = new OpenAIClient(new ApiKeyCredential(apiKey),options)
    .GetChatClient(model)
    .AsIChatClient();

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.GetResponseAsync(prompt.ToString());

// display the response
Console.WriteLine(response.Text);