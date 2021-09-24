namespace masterCore.Entities
{
    public class AspNetUsersGroup
    {
        public int id { get; set; }

        public string name { get; set; }

        public bool inactive { get; set; }

        public bool? administrator { get; set; }

        public decimal? maximumDiscount { get; set; }

        public int? idCustomer { get; set; }
    }
}
