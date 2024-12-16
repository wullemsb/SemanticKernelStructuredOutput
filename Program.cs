using Microsoft.Extensions.Configuration;

// Initialize kernel.
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using System.Text.Json;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets<Program>();

IConfiguration configuration = builder.Build();

Kernel kernel = Kernel.CreateBuilder()
   
   .AddAzureOpenAIChatCompletion(deploymentName: "gpt-4o", endpoint: configuration["OpenAI:apiUrl"], apiKey: configuration["OpenAI:apiKey"])
    //.AddOpenAIChatCompletion(modelId:"phi3.5",endpoint:new Uri("http://localhost:11434"),apiKey:null)
    .Build();

// Initialize ChatResponseFormat object with JSON schema of desired response format.
ChatResponseFormat chatResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
    jsonSchemaFormatName: "recipe",
    jsonSchema: BinaryData.FromString("""
        {
            "type": "object",
            "properties": {
                "Ingredients": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "Name": { "type": "string" },
                            "Quantity": { "type": "string" },
                            "Unit": { "type": "string" }
                        },
                        "required": ["Name", "Quantity", "Unit"],
                        "additionalProperties": false
                    }
                }
            },
            "required": ["Ingredients"],
            "additionalProperties": false
        }
        """),
    jsonSchemaIsStrict: true);

// Specify response format by setting ChatResponseFormat object in prompt execution settings.
var executionSettings = new OpenAIPromptExecutionSettings
{
    ResponseFormat = chatResponseFormat
};

// Send a request and pass prompt execution settings with desired response format.
var result = await kernel.InvokePromptAsync("What are the ingredients needed to prepare a Christmas Turkey?", new(executionSettings));

Console.WriteLine(result);

var recipe= JsonSerializer.Deserialize<Recipe>(result.ToString());

// Define response models
public class Recipe
{
    public List<Ingredient> Ingredients { get; set; }
}

public class Ingredient
{
    public string Name { get; set; }
    public string Quantity { get; set; }
    public string Unit { get; set; }
}

#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
