namespace SmartPharma5.Model
{
    public class Form_Type
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Memo { get; set; }

        public Form_Type(uint id, string name, DateTime createDate, string memo)
        {
            Id = id;
            this.Name = name;
            CreateDate = createDate;
            this.Memo = memo;
        }
    }
}
