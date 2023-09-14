namespace ModularSystem.Core
{
    public class LegalDocument
    {
        public string Number { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public DateTime ValidUntil { get; set; }
    }
}
