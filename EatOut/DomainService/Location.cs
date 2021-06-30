using System;
using System.Collections.Generic;
using System.Text;

namespace EatOut
{
    public class Location
    {
        public double Latitude;

        public double Longitude;

        public int ResultSize = 5;

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        internal void ValidateRequiredValues()
        {
            if (Latitude < -90 || Latitude > 90)
                throw new ArgumentException("Latitude is not withing range -90 to 90 ");

            if (Longitude < -180 || Longitude > 180)
                throw new ArgumentException("Longitude is not withing range -90 to 90 ");
        }
    }

}
