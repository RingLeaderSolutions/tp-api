namespace Theta.Platform.Domain
{
    public class Instrument
    {
        public AssetClassType AssetClass { get; set; }

        public InstrumentIdType IdType { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}