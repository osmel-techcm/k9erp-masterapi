namespace masterCore.Entities
{
    public class Config
    {
        public int Id { get; set; }
        public string propName { get; set; }
        public string propValue { get; set; }
        public bool propPublic { get; set; }
        public bool propProtected { get; set; }
        public string propDescription { get; set; }
    }
}
