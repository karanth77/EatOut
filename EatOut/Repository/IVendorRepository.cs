using System.Collections.Generic;
using System.Threading.Tasks;

namespace EatOut
{
    public interface IVendorRepository
    {
        Task<List<Vendor>> GetAllVendors();
    }
}