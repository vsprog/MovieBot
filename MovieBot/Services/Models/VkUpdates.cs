using System.Text.Json.Nodes;

namespace MovieBot.Services.Models;

[Serializable]
public class VkUpdates
{
    public string Type { get; set; }

    public JsonObject? Object { get; set; }

    public long GroupId { get; set; }
}
