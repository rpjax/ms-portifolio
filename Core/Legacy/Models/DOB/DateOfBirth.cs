namespace ModularSystem.Core
{
    public class DateOfBirth
    {
        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string ToStringBrazil()
        {
            var day = Day < 10 ? $"0{Day}" : $"{Day}";
            var month = Month < 10 ? $"0{Month}" : $"{Month}";
            return $"{day}/{month}/{Year}";
        }
    }
}
