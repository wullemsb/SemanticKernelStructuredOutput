using OllamaSharp;
using OllamaSharp.Models;
using System.Text;
using System.Text.Json;


var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri);

ollama.SelectedModel = "llama3.1:latest";

var request = new GenerateRequest()
{
    Prompt = "What are the ingredients needed to prepare a Christmas Turkey?",
    Format = JsonSchema.ToJsonSchema(typeof(Recipe))
};

var result = new StringBuilder();

await foreach (var stream in ollama.GenerateAsync(request))
{
    result.Append(stream.Response);
    Console.Write(stream.Response);
}

var recipe= JsonSerializer.Deserialize<Recipe>(result.ToString());

Console.ReadLine();

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

