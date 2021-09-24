using System;

namespace masterCore.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FIEN { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Website { get; set; }

        public int? Licences { get; set; }

        public int? NAICS { get; set; }

        public int? LeadSource { get; set; }

        public DateTime? LeadDate { get; set; }

        public decimal? CPL { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZIP { get; set; }

        public string Country { get; set; }

        public string BillingName { get; set; }

        public string BillingLastName { get; set; }

        public string BillingContactTitle { get; set; }

        public string BillingEmail { get; set; }

        public string BillingCellForText { get; set; }

        public string TechnicalName { get; set; }

        public string TechnicalLastName { get; set; }

        public string TechnicalContactTitle { get; set; }

        public string TechnicalEmail { get; set; }

        public string TechnicalCellForText { get; set; }

        public bool? Active { get; set; }

        public bool? TwoFactor { get; set; }
    }
}
