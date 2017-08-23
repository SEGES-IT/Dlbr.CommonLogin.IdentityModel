namespace Dlbr.CommonLogin.IdentityModel
{
    /// <summary>
    /// Class representing additional WCF Header info that can be applied to the WCF call using WcfServiceWrapper.
    /// </summary>
    public class WcfHeaderInfo
    {
        /// <summary>
        /// The key of this particular WCF Header.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Namespace for this WCF Header.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// The value provided.
        /// </summary>
        public string Value { get; set; }
    }
}
