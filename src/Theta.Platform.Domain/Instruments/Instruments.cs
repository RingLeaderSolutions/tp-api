using System;
using System.Collections.Generic;

namespace Theta.Platform.Domain.Instruments
{
    public class InstrumentSampleData
    {
        public static List<Instrument> All = new List<Instrument>
        {
            new Instrument
            {
                Id = Guid.Parse("c11d5a3e-786a-4522-abef-558449014b69"),
                AssetClass = AssetClassType.FixedIncome,
                Ids = new List<InstrumentId>
                {
                    new InstrumentId { Id = "GB00B16NNR78", IdType = InstrumentIdType.ISIN },
                    new InstrumentId { Id = "0088279A7", IdType = InstrumentIdType.CUSIP },
                    new InstrumentId { Id = "B16NNR7", IdType = InstrumentIdType.SEDOL },
                    new InstrumentId { Id = "A0GZPU", IdType = InstrumentIdType.WKN },
                    new InstrumentId { Id = "TR27", IdType = InstrumentIdType.Symbol },
                }.ToArray(),
                Name = "UK 10 Year Gilt",
                Category = "Government Bond",
                CouponRate = 4.250m,
                CouponType = CouponType.Fixed,
                CouponFrequency = CouponFrequency.SemiAnnually,
                AccruedInterest = 1.87m,
                AccruedDays = 160,
                IssueDate = new DateTimeOffset(2006, 09, 06, 0, 0, 0, TimeSpan.Zero),
                MaturityDate = new DateTimeOffset(2027, 12, 07, 0, 0, 0, TimeSpan.Zero),
                CouponPaymentDate = new DateTimeOffset(2018, 12, 07, 0, 0, 0, TimeSpan.Zero),
                MinimumDenomination = 0.01m,
                DenominationCcy = "GBP"
            },
            new Instrument
            {
                Id = Guid.Parse("fb48b596-4d6c-4b52-8172-6ab22912df09"),
                AssetClass = AssetClassType.FixedIncome,
                Ids = new List<InstrumentId>
                {
                    new InstrumentId { Id = "XS0158715713", IdType = InstrumentIdType.ISIN },
                    new InstrumentId { Id = "EC7667958", IdType = InstrumentIdType.CUSIP },
                    new InstrumentId { Id = "3230097", IdType = InstrumentIdType.SEDOL },
                    new InstrumentId { Id = "249002", IdType = InstrumentIdType.WKN },
                    new InstrumentId { Id = "VO32", IdType = InstrumentIdType.Symbol },
                }.ToArray(),
                Name = "Vodafone Group Plc",
                Category = "Corporate Bond",
                CouponRate = 5.900m,
                CouponType = CouponType.Fixed,
                CouponFrequency = CouponFrequency.Annually,
                AccruedInterest = 0m,
                AccruedDays = 0,
                IssueDate = new DateTimeOffset(2002, 11, 02, 0, 0, 0, TimeSpan.Zero),
                MaturityDate = new DateTimeOffset(2032, 11, 26, 0, 0, 0, TimeSpan.Zero),
                CouponPaymentDate = new DateTimeOffset(2018, 11, 26, 0, 0, 0, TimeSpan.Zero),
                MinimumDenomination = 1000,
                DenominationCcy = "GBP"
            }
        };

    }
}