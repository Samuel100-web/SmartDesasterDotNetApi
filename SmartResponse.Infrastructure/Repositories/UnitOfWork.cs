using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;
using SmartResponse.Infrastructure.Data;

namespace SmartResponse.Infrastructure.Repositories
{
    // Infrastructure/Repositories/UnitOfWork.cs
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Incidents = new GenericRepository<Incident>(_context);
            IncidentTypes = new GenericRepository<IncidentType>(_context);
            IncidentMedias = new GenericRepository<IncidentMedia>(_context);
            IncidentVerifications = new GenericRepository<IncidentVerification>(_context);
            IncidentVotes = new GenericRepository<IncidentVote>(_context);
            Users = new GenericRepository<User>(_context);
            Roles = new GenericRepository<Role>(_context);
            Resources = new GenericRepository<Resource>(_context);
            ResourceCategories = new GenericRepository<ResourceCategory>(_context);
            SyncLogs = new GenericRepository<ExternalApiSyncLog>(_context);
            Donations = new GenericRepository<Donation>(_context);
            DonationCategories = new GenericRepository<DonationCategory>(_context);
            PaymentMethods = new GenericRepository<PaymentMethod>(_context);
            DonationItems = new GenericRepository<DonationItem>(_context);
            Ngos = new GenericRepository<Ngo>(_context);
            HomeSlider = new GenericRepository<HomeSlider>(_context);
        }

        public IGenericRepository<Incident> Incidents { get; private set; }
        public IGenericRepository<IncidentType> IncidentTypes { get; private set; }
        public IGenericRepository<IncidentMedia> IncidentMedias { get; private set; }
        public IGenericRepository<IncidentVerification> IncidentVerifications { get; private set; }
        public IGenericRepository<IncidentVote> IncidentVotes { get; private set; }
        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Role> Roles { get; private set; }
        public IGenericRepository<Resource> Resources { get; private set; }
        public IGenericRepository<ResourceCategory> ResourceCategories { get; private set; }
        public IGenericRepository<ExternalApiSyncLog> SyncLogs { get; private set; }
        public IGenericRepository<Donation> Donations { get; private set; }
        public IGenericRepository<DonationCategory> DonationCategories { get; private set; }
        public IGenericRepository<PaymentMethod> PaymentMethods { get; private set; }
        public IGenericRepository<DonationItem> DonationItems { get; private set; }
        public IGenericRepository<Ngo> Ngos { get; private set; }       

        public IGenericRepository<HomeSlider> HomeSlider { get; private set; }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
