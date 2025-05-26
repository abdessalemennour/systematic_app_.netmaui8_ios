namespace SmartPharma5.Model
{
    public class Question_Type
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Memo { get; set; }

        public Question_Type(uint id, string name, DateTime createDate, string memo)
        {
            Id = id;
            Name = name;
            CreateDate = createDate;
            Memo = memo;
        }
    }
}
