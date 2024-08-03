using mem0.NET.Service.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace mem0.NET.Service;

public class MasterDbContext(DbContextOptions<MasterDbContext> options) : Mem0DbContext<MasterDbContext>(options)
{
}