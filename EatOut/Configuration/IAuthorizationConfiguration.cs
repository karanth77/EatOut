namespace EatOut.Configuration
{
    public interface IAuthorizationConfiguration
    {
        /// <summary>
        /// The name of the variable for list of application that is allowed.
        /// </summary>
        string WhitelistAppsLiteral { get; }

        /// <summary>
        /// Bypass security check.
        /// </summary>
        /// <returns>The retrieved value.</returns>
        bool BypassSecurity();

        /// <summary>
        /// The list of apps that is allowed.
        /// </summary>
        /// <returns>The retrieved value.</returns>
        string WhitelistApps();
    }
}