using SmartResponse.Core.Entities;

namespace SmartResponse.Core.Interfaces
{
    // Core/Interfaces/IUnitOfWork.cs
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Incident> Incidents { get; }
        IGenericRepository<IncidentType> IncidentTypes { get; }
        IGenericRepository<IncidentMedia> IncidentMedias { get; }
        IGenericRepository<IncidentVerification> IncidentVerifications { get; }
        IGenericRepository<IncidentVote> IncidentVotes { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<Role> Roles { get; }
        IGenericRepository<Resource> Resources { get; }
        IGenericRepository<ResourceCategory> ResourceCategories { get; }
        IGenericRepository<ExternalApiSyncLog> SyncLogs { get; }
        IGenericRepository<Donation> Donations { get; }
        IGenericRepository<DonationCategory> DonationCategories { get; }
        IGenericRepository<PaymentMethod> PaymentMethods { get; }
        IGenericRepository<DonationItem> DonationItems { get; }
        IGenericRepository<Ngo> Ngos { get; }
        IGenericRepository<HomeSlider> HomeSlider { get; }

        Task<int> CompleteAsync();
    }
}
