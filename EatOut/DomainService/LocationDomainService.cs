using EatOut.Common;
using EatOut.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EatOut
{
    public class LocationDomainService : ILocationDomainService
    {
        private readonly HttpClient _httpClient;

        private IConfigurationService configService;

        private IVendorRepository vendorRepository;

        public LocationDomainService(HttpClient httpClient, IConfigurationService configService, IVendorRepository vendorRepository)
        {
            _httpClient = httpClient;

            Validate.IsNotNull(configService, nameof(configService));

            Validate.IsNotNull(vendorRepository, nameof(vendorRepository));

            this.configService = configService;

            this.vendorRepository = vendorRepository;
        }

        private double CalculateDistance(Location point1, Location point2)
        {
            //GeoCoordinate.DistanceTo() not available in .net core
            var d1 = point1.Latitude * (Math.PI / 180.0);
            var num1 = point1.Longitude * (Math.PI / 180.0);
            var d2 = point2.Latitude * (Math.PI / 180.0);
            var num2 = point2.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        // Brute force approach to find all the vendors in ascending order of distance.
        // Ideal way to do it is to divide the vendors into a 2 dimensional grid for a city with each city divided into managable regions.
        // Each Country will have a list of cities..
        // Then given a lat:long find the country, then find the city , then find the grid where it belongs and run this algorithm.
        // When the user expands run a breadth first algorith to find more vendors.
        public async Task<List<Vendor>> FindNearBy(Location request, CancellationToken cancellationToken)
        {
            var vendors = await vendorRepository.GetAllVendors();

            cancellationToken.ThrowIfCancellationRequested();

            SortedDictionary<double, List<Vendor>> vendorsByDistance = new SortedDictionary<double, List<Vendor>>();
            
            foreach (var vendor in vendors)
            {
                //GeoCoordinate coordinate = new GeoCoordinate(coordinates.Latitude, coordinates.Longitude);
                // coordinate.DistanceTo(vendor.Latitude, vendor.Longitude)
                // This class is not supported in .net core and i have to implement my own distance calculation.
                var distance = CalculateDistance(request, new Location(vendor.Latitude, vendor.Longitude));

                if (vendorsByDistance.ContainsKey(distance))
                {
                    vendorsByDistance[distance].Add(vendor);
                }
                else
                {
                    vendorsByDistance.Add(CalculateDistance(request, new Location(vendor.Latitude, vendor.Longitude)), new List<Vendor> { vendor });
                }
            }

            List<Vendor> result = new List<Vendor>();

            int totalResults = 0;
            foreach(var pair in vendorsByDistance)
            {
                if(totalResults >= request.ResultSize)
                {
                    break;
                }

                totalResults++;
                result.AddRange(pair.Value);
            }

            return result;
        }
    }
}