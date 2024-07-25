namespace ModularSystem.Core
{
    public class BusinessData
    {
        public string LegalName { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public Address Address { get; set; } = new Address();
        public PhoneNumber PhoneNumber { get; set; } = new PhoneNumber();
        public LegalDocument RegistrationDocument { get; set; } = new LegalDocument();
    }
}
