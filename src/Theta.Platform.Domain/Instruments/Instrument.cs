using System;


namespace Theta.Platform.Domain.Instruments
{
    public class Instrument
    {
        public Guid Id { get; set; }

        public AssetClassType AssetClass { get; set; }

        public InstrumentId[] Ids { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public decimal CouponRate { get; set; }

        public CouponType CouponType { get; set; }

        public CouponFrequency CouponFrequency { get; set; }

        public decimal AccruedInterest { get; set; }

        public decimal AccruedDays { get; set; }

        public DateTimeOffset IssueDate { get; set; }

        public DateTimeOffset MaturityDate { get; set; }

        public DateTimeOffset CouponPaymentDate { get; set; }

        public decimal MinimumDenomination { get; set; }

        public string DenominationCcy { get; set; }

        // more...
    }

}