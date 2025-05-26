namespace SmartPharma5.Model
{
    public class Empolye
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Empolye() { }
        public Empolye(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
