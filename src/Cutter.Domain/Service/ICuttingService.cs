using Cutter.Domain.Data;

namespace Cutter.Domain.Service
{
    public interface ICuttingService
    {
        CutResults Optimize(CutParameters parameters);
    }
}