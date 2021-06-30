namespace EatOut.Configuration
{
    public interface IConfigurationService
    {
        IAuthorizationConfiguration Authorization { get; }

        string ReadSetting(string key, string defaultValue = null, bool throwIfNotFound = false);
    }
}

