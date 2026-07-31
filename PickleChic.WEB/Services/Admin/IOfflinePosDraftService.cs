using PickleChic.WEB.Model;

namespace PickleChic.WEB.Services.Admin
{
    public interface IOfflinePosDraftService
    {
        const int MaxTabs = 15;

        Task<OfflinePosDraftStore> LoadAsync();

        Task SaveAsync(OfflinePosDraftStore store);

        Task<OfflinePosDraftStore> EnsureInitializedAsync();

        Task PersistTabsAsync(List<OfflinePosTab> tabs, Guid activeTabId, int tabSequence);

        Task RemoveTabAsync(Guid tabId);
    }
}
