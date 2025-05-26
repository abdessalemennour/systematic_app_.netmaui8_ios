namespace SmartPharma5.Model
{
    public class ShearchItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ShearchItem() { }

        public ShearchItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static List<ShearchItem> getAllSheachItem()
        {
            var List = new List<ShearchItem>();
            List.Add(new ShearchItem(1, "Name employe"));
            List.Add(new ShearchItem(2, "Description"));
            List.Add(new ShearchItem(3, "Create date"));
            List.Add(new ShearchItem(4, "Start date"));
            List.Add(new ShearchItem(5, "End date"));
            List.Add(new ShearchItem(6, "STAT : ACCEPTER"));
            List.Add(new ShearchItem(7, "STAT : EN ATTENT"));
            List.Add(new ShearchItem(8, "STAT : REFUSER"));
            return List;

        }
    }
}
