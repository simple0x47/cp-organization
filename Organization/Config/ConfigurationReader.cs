namespace Organization.Config;

public class ConfigurationReader
{
    private readonly IConfiguration _configuration;

    public ConfigurationReader(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public double GetDoubleOrDefault(string key, double defaultValue)
    {
        try
        {
            return double.Parse(_configuration[key]);
        }
        catch (Exception e)
        {
            return defaultValue;
        }
    }
}