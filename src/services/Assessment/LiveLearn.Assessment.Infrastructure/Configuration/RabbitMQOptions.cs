
namespace LiveLearn.Assessment.Infrastructure.Configuration;

internal class RabbitMQOptions
{
    public static string SectionName => "RabbitMQ";
    public string Host { get; set; } = string.Empty;

    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

