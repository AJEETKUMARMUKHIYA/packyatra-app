
namespace MoversAndPackerApi.Models
{
    public class GoodsConsignmentNote
    {
        public int Id { get; set; }

        // Consigner
        public string ConsignerName { get; set; }
        public string PickupAddress { get; set; }
        public string ConsignerPhone { get; set; }
        public string ConsignerEmail { get; set; }

        // Consignee
        public string ConsigneeName { get; set; }
        public string DeliveryAddress { get; set; }
        public string ConsigneePhone { get; set; }
        public string ConsigneeEmail { get; set; }

        // Transport Details
        public string VehicleRegNo { get; set; }
        public DateTime PickupDate { get; set; }
        public string VehicleType { get; set; }

        // Order Details
        public int TotalPackages { get; set; }
        public decimal OrderValue { get; set; }
        public string GcNumber { get; set; }
        public decimal BookingAmount { get; set; }
        public string SacCode { get; set; } = "996511";

        // Consignment Info
        public string ConsignmentType { get; set; } // By Road
        public string GoodsDescription { get; set; } // Household goods
        public string GrossWeightOrVolume { get; set; }

        // Delivery Info
        public bool DoorDelivery { get; set; }
        public bool GodownDelivery { get; set; }
        public bool UnloadingByCustomer { get; set; }
        public bool UnloadingByTransporter { get; set; }

        public DateTime GeneratedDate { get; set; }
    }
}

