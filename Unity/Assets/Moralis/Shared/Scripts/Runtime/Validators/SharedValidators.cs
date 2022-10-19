using System.Text.RegularExpressions;

namespace MoralisUnity.Samples.Shared.Utilities
{
    /// <summary>
    /// Provides runtime validators
    /// </summary>
    public static class SharedValidators
    {
        
        /// <summary>
        /// A regular expression for matching Ethereum addresses must check for a
        /// leading 0x followed by a random string of 40 hexadecimal characters
        /// (lowercase a-f, uppercase A-F, and numbers 0-9). These expressions
        /// are not case sensitive, although a capitalized checksum version
        /// exists that refers to the same account but provides an added layer
        /// of security.
        /// </summary>
        /// <param name="tokenAddress"></param>
        /// <returns></returns>
        public static bool IsValidWeb3Address(string tokenAddress)
        {
            // 66 Total Characters
            tokenAddress = tokenAddress.ToLower(); //case insensitive
            Regex regex = new Regex("^0x[a-fA-F0-9]{40}$");
            return regex.Match(tokenAddress).Success;
        }
    }
}