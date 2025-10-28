using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Application.Models;

namespace SIGEBI.Application.Interfaces;

public interface IAdminService
{
    Task<AdminSummary> ObtenerResumenAsync(CancellationToken ct = default);
}
