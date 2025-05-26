namespace SmartPharma5.Model
{
    public class humain
    {
        public string Name { get; set; }
        public string LastName { get; set; }

        public humain(string name, string lastName)
        {
            Name = name;
            LastName = lastName;
        }
    }
}
