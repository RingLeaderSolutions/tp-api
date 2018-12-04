using System;

namespace Theta.Platform.UI.Pricing.Streaming.Model
{
    public class Instrument
    {
        public Guid Id { get; set; }

        public AssetClassType AssetClass { get; set; }

        public InstrumentId[] Ids { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }
    }

    public enum AssetClassType
    {
        FixedIncome,
        ForeignExchange,
        Equities
    }

    public class InstrumentId
    {
        public InstrumentIdType IdType { get; set; }

        public string Id { get; set; }
    }

    public enum InstrumentIdType
    {
        ISIN,
        CUSIP,
        SEDOL,
        WKN,
        Symbol,
        Ticker
    }
}