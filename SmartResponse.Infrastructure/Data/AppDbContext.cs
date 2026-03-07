using Microsoft.EntityFrameworkCore;
using SmartResponse.Core.Entities;
using System.Linq.Expressions;

namespace SmartResponse.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<IncidentType> IncidentTypes { get; set; }
        public DbSet<IncidentMedia> IncidentMedias { get; set; }
        public DbSet<IncidentVerification> IncidentVerifications { get; set; }
        public DbSet<IncidentVote> IncidentVotes { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceCategory> ResourceCategories { get; set; }
        public DbSet<ExternalApiSyncLog> SyncLogs { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<DonationCategory> DonationCategories { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<DonationItem> DonationItems { get; set; }
        public DbSet<Ngo> Ngos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Core.Entities.ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                        ConvertFilterExpression(entityType.ClrType)
                    );
                }
            }
            
            modelBuilder.Entity<Incident>(entity =>
            {
                entity.HasMany(i => i.Media).WithOne(m => m.Incident).HasForeignKey(m => m.IncidentId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(i => i.Verifications).WithOne(v => v.Incident).HasForeignKey(v => v.IncidentId);
                entity.HasMany(i => i.Votes).WithOne(v => v.Incident).HasForeignKey(v => v.IncidentId);
                
                entity.HasOne(i => i.Reporter).WithMany(u => u.ReportedIncidents).HasForeignKey(i => i.ReporterId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasOne(r => r.Category).WithMany(c => c.Resources).HasForeignKey(r => r.CategoryId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId);
            });
            modelBuilder.Entity<User>(entity => {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.CNIC).IsUnique();
            });
            modelBuilder.Entity<Incident>(entity => {
                entity.Property(i => i.TargetLat).HasPrecision(18, 6);
                entity.Property(i => i.TargetLong).HasPrecision(18, 6);
            });
            
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            var adminRoleId = new Guid("8db2260d-405a-406a-8451-f2f171097223");
            var volunteerRoleId = new Guid("41f235d9-7221-4f6c-8438-e6d877f8045f");
            var publicRoleId = new Guid("e00949d0-60f1-4682-9658-0051010b9122");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, Name = "Admin", CreatedAt = seedDate },
                new Role { Id = volunteerRoleId, Name = "Volunteer", CreatedAt = seedDate },
                new Role { Id = publicRoleId, Name = "Public", CreatedAt = seedDate }
            );
            
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("f1e2d3c4-b5a6-4f7e-8d9c-0b1a2c3d4e5f"),
                    FullName = "System Admin",
                    Email = "admin@smartresponse.com",
                    PasswordHash = "$2a$11$mC8m6b8m6b8m6b8m6b8m6eu8y3iR2/mU8u8u8u8u8u8u8u8u8u8u8", // Hash for 'Admin@123'
                    PhoneNumber = "03001234567",
                    CNIC = "3520100000000",
                    Address = "Admin Office, Islamabad",
                    BloodGroup = "O+",
                    EmergencyContact = "03007654321",
                    TrustScore = 100,
                    RoleId = adminRoleId,
                    CreatedAt = seedDate
                }
            );

            
            modelBuilder.Entity<IncidentType>().HasData(
                new IncidentType { Id = new Guid("79c72076-905c-426c-829d-6401f8d4e41f"), Name = "Flood", Icon = "water", CreatedAt = seedDate },
                new IncidentType { Id = new Guid("2a32c253-339c-4475-926c-132b84236e7a"), Name = "Fire", Icon = "flame", CreatedAt = seedDate },
                new IncidentType { Id = new Guid("67616616-d35d-4f0f-8b54-933e7284897f"), Name = "Earthquake", Icon = "grid", CreatedAt = seedDate }
            );
            
            modelBuilder.Entity<ResourceCategory>().HasData(
                new ResourceCategory { Id = new Guid("577f10b7-41ec-43e3-82c5-8f6a9c687063"), Name = "Medical", CreatedAt = seedDate },
                new ResourceCategory { Id = new Guid("9675841e-6e84-4861-a5f9-67d643867622"), Name = "Food", CreatedAt = seedDate },
                new ResourceCategory { Id = new Guid("b40f13f0-4966-4c4f-801a-877202720f4c"), Name = "Shelter", CreatedAt = seedDate }
            );
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Core.Entities.ISoftDelete>())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Added:                        
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        private static LambdaExpression ConvertFilterExpression(Type type)
        {
            var parameter = Expression.Parameter(type, "it");
            var property = Expression.Property(parameter, nameof(Core.Entities.ISoftDelete.IsDeleted));
            var comparison = Expression.Equal(property, Expression.Constant(false));
            return Expression.Lambda(comparison, parameter);
        }
    }
}