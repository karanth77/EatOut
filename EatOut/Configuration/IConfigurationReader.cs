namespace EatOut.Configuration
{
    public interface IConfigurationReader
    {
        string ReadSetting(string key, string defaultValue = null, bool throwIfNotFound = false);
    }

}

