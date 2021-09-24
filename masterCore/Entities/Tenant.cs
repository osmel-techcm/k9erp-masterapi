namespace masterCore.Entities
{
    public class Tenant
    {
        public int Id { get; set; }
        public string TenantName { get; set; }
        public string ConnectionString { get; set; }
        public bool Inactive { get; set; }
        public int Customer { get; set; }
    }
}
