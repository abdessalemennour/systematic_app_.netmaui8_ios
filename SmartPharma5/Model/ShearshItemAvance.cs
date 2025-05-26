namespace SmartPharma5.Model
{
    public class ShearchItemAvance
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ShearchItemAvance() { }

        public ShearchItemAvance(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static List<ShearchItemAvance> getAllSheachItem()
        {
            var List = new List<ShearchItemAvance>();
            List.Add(new ShearchItemAvance(1, "Name employe"));
            List.Add(new ShearchItemAvance(2, "Description"));
            List.Add(new ShearchItemAvance(3, "Create date"));

            List.Add(new ShearchItemAvance(5, "STAT : ACCEPTER"));
            List.Add(new ShearchItemAvance(6, "STAT : EN ATTENT"));
            List.Add(new ShearchItemAvance(7, "STAT : REFUSER"));
            return List;

        }
    }

    public class ShearchItemStatProfile
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public ShearchItemStatProfile() { }

        public ShearchItemStatProfile(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static List<ShearchItemStatProfile> getAllSheachItem()
        {
            var List = new List<ShearchItemStatProfile>();
            List.Add(new ShearchItemStatProfile(1, "All"));
            List.Add(new ShearchItemStatProfile(2, "Current"));
            List.Add(new ShearchItemStatProfile(3, "Accepted"));
            List.Add(new ShearchItemStatProfile(4, "Refused"));
            return List;

        }
    }

    public class ShearchItemAttribute
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public ShearchItemAttribute() { }

        public ShearchItemAttribute(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public static List<ShearchItemAttribute> getAllSheachItem()
        {
            var List = new List<ShearchItemAttribute>();
            List.Add(new ShearchItemAttribute(1, "Category"));
            List.Add(new ShearchItemAttribute(2, "Name Partner"));
            List.Add(new ShearchItemAttribute(3, "Country"));
            List.Add(new ShearchItemAttribute(4, "City"));
            List.Add(new ShearchItemAttribute(5, "Stat"));
            List.Add(new ShearchItemAttribute(6, "Email"));
            List.Add(new ShearchItemAttribute(7, "Phone"));
            List.Add(new ShearchItemAttribute(8, "Mobile"));
            return List;

        }


    }
}
