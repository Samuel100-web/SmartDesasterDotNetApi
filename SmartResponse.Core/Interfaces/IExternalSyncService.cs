using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.Interfaces
{
    public interface IExternalSyncService
    {
        Task SyncAllApis();
    }
}
