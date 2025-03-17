using HireSphereApi.core.DTO;
using HireSphereApi.core.entities;
using HireSphereApi.entities;
using Microsoft.EntityFrameworkCore;

namespace HireSphereApi.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<ExtractedDataEntity> ExtractedData { get; set; }
    }
}
