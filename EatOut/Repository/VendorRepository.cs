using EatOut.Common;
using EatOut.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EatOut
{
    public class VendorRepository : IVendorRepository
    {
        private readonly HttpClient _httpClient;

        private IConfigurationService configService;

        private readonly ILogger logger;

        private List<Vendor> vendors;

        private DateTime RefreshTime;

        public VendorRepository(HttpClient httpClient, IConfigurationService configService, ILoggerFactory loggerFactory)
        {
            _httpClient = httpClient;

            Validate.IsNotNull(configService, nameof(configService));

            Validate.IsNotNull(loggerFactory, nameof(loggerFactory));

            this.configService = configService;

            this.logger = (loggerFactory ?? throw new ArgumentNullException($"{nameof(loggerFactory)} is null"))
                .CreateLogger(nameof(VendorRepository)) ?? throw new ArgumentNullException($"{nameof(logger)} is null");

            vendors = new List<Vendor>();
        }

        // Another option is to read all vendors and store it in no sql so that way we can enable offline access.
        // We can then read it from no sql and serve it from redis cache.
        // There could be another job that refreshes the no sql.
        public async Task<List<Vendor>> GetAllVendors()
        {
            if(this.vendors.Count() > 0 && !ShouldRefresh())
            {
                return this.vendors;
            }

            var url = configService.ReadSetting("FoodTruckUrl", "https://data.sfgov.org/api/views/rqzj-sfat/rows.csv");

            string result = await _httpClient.GetStringAsync(url);

            string[] lines = result.Split(
                                new[] { "\r\n", "\r", "\n" },
                                StringSplitOptions.None);
            
            foreach (var line in lines.Skip(1))
            {
                vendors.Add(GetVendorsFromLine(line));
            }

            RefreshTime = DateTime.UtcNow;

            return vendors;
        }

        private bool ShouldRefresh()
        {
            double ri = Double.Parse(configService.ReadSetting("RefreshInterval", "30"));
            return RefreshTime.AddMinutes(ri) < DateTime.UtcNow;
        }

        private Vendor GetVendorsFromLine(string line)
        {
            Vendor v = new Vendor();

            string[] fields = CsvHelper.SplitCSV(line); 

            int i = 0;

            try
            {
                for (; i < fields.Length; i++)
                {
                    switch (i)
                    {
                        case 0: v.LocationId = Int32.Parse(fields[i]); break;
                        case 1: v.Applicant = fields[i]; break;
                        case 2: v.FacilityType = fields[i]; break;
                        case 3: v.Cnn = Int32.Parse(fields[i]); break;
                        case 4: v.LocationDescription = fields[i]; break;
                        case 5: v.Address = fields[i]; break;
                        case 6: v.Blocklot = Int32.Parse(fields[i]); break;
                        case 7: v.Block = Int32.Parse(fields[i]); break;
                        case 8: v.Lot = Int32.Parse(fields[i]); break;
                        case 9: v.Permit = fields[i]; break;
                        case 10: v.Status = fields[i]; break;
                        case 11: v.FoodItems = fields[i]; break;
                        case 12: v.X = fields[i]; break;
                        case 13: v.Y = fields[i]; break;
                        case 14: v.Latitude = Double.Parse(fields[i]); break;
                        case 15: v.Longitude = Double.Parse(fields[i]); break;
                        case 16: v.Schedule = fields[i]; break;
                        case 17: v.DaysHours = fields[i]; break;
                        case 18: v.NOISent = fields[i]; break;
                        case 19: v.Approved = fields[i]; break;
                        case 20: v.Received = fields[i]; break;
                        case 21: v.PriorPermit = fields[i]; break;
                        case 22: v.ExiprationDate = DateTime.Parse(fields[i]); break;
                        default: break;
                    }
                }
            }
            catch(Exception ex)
            {
                // We can add alert based on this log.
                logger.LogError($"Encounterd exception while parsing {ex}");
            }

            return v;
        }
    }
}