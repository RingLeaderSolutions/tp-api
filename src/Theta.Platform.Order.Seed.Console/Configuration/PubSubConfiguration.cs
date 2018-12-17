using System;
using System.Collections.Generic;
using System.Text;

namespace Theta.Platform.Order.Seed.Console.Configuration
{
    public class PubSubConfiguration
    {
        public string Endpoint { get; set; }

        public string CreateOrderEntityPath { get; set; }
        public string PutDownOrderEntityPath { get; set; }
        public string PickUpOrderEntityPath { get; set; }
        public string CompleteOrderEntityPath { get; set; }
        public string RseOrderEntityPath { get; set; }
        public string RejectOrderEntityPath { get; set; }
        public string FillOrderEntityPath { get; set; }
    }
}
