using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EatOut
{
    public interface ILocationDomainService
    {
        Task<List<Vendor>> FindNearBy(Location request, CancellationToken cancellationToken);
    }
}