namespace ModularSystem.Core
{
    public class PersonData
    {
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public PersonName Name { get; set; } = new PersonName();
        public DateOfBirth DateOfBirth { get; set; } = new DateOfBirth();
    }
    public class LegalPersonData : PersonData
    {
        public Address Address { get; set; } = new Address();
        public PhoneNumber PhoneNumber { get; set; } = new PhoneNumber();
        public LegalDocument Document { get; set; } = new LegalDocument();
    }
}
