using System;

namespace EatOut
{
    public class Vendor
    {

        public int Lot { get; internal set; }
        public int LocationId { get; internal set; }
        public string Applicant { get; internal set; }
        public string FacilityType { get; internal set; }
        public int Cnn { get; internal set; }
        public string LocationDescription { get; internal set; }
        public string Address { get; internal set; }
        public int Blocklot { get; internal set; }
        public int Block { get; internal set; }
        public string Permit { get; internal set; }
        public string Status { get; internal set; }
        public string FoodItems { get; internal set; }
        public string X { get; internal set; }
        public string Y { get; internal set; }
        public double Latitude { get; internal set; }
        public double Longitude { get; internal set; }
        public string Schedule { get; internal set; }
        public string DaysHours { get; internal set; }
        public string NOISent { get; internal set; }
        public string Approved { get; internal set; }
        public string Received { get; internal set; }
        public string PriorPermit { get; internal set; }
        public DateTime ExiprationDate { get; internal set; }
    }
}
