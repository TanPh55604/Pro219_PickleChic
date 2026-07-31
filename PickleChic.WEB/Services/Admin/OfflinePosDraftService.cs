using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Storage;
using WebConstant = PickleChic.WEB.Constant.Constant;

namespace PickleChic.WEB.Services.Admin
{
    public class OfflinePosDraftService : IOfflinePosDraftService
    {
        private readonly ILocalStorageService _localStorage;

        public OfflinePosDraftService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<OfflinePosDraftStore> LoadAsync()
        {
            var store = await _localStorage.GetItemAsync<OfflinePosDraftStore>(
                WebConstant.OfflinePos.DraftStorageKey);

            if (store is null)
            {
                return new OfflinePosDraftStore();
            }

            store.Tabs ??= new List<OfflinePosTab>();
            if (store.Tabs.Count > IOfflinePosDraftService.MaxTabs)
            {
                store.Tabs = store.Tabs.Take(IOfflinePosDraftService.MaxTabs).ToList();
            }

            return store;
        }

        public async Task SaveAsync(OfflinePosDraftStore store)
        {
            store.Tabs ??= new List<OfflinePosTab>();
            if (store.Tabs.Count > IOfflinePosDraftService.MaxTabs)
            {
                store.Tabs = store.Tabs.Take(IOfflinePosDraftService.MaxTabs).ToList();
            }

            await _localStorage.SetItemAsync(WebConstant.OfflinePos.DraftStorageKey, store);
        }

        public async Task<OfflinePosDraftStore> EnsureInitializedAsync()
        {
            var store = await LoadAsync();

            if (store.Tabs.Count == 0)
            {
                var tab = CreateBlankTab(1);
                store.Tabs.Add(tab);
                store.ActiveTabId = tab.Id;
                store.TabSequence = 2;
                await SaveAsync(store);
            }
            else if (store.ActiveTabId == Guid.Empty
                     || store.Tabs.All(t => t.Id != store.ActiveTabId))
            {
                store.ActiveTabId = store.Tabs[0].Id;
                await SaveAsync(store);
            }

            if (store.TabSequence < store.Tabs.Count + 1)
            {
                store.TabSequence = store.Tabs.Count + 1;
            }

            return store;
        }

        public async Task PersistTabsAsync(List<OfflinePosTab> tabs, Guid activeTabId, int tabSequence)
        {
            await SaveAsync(new OfflinePosDraftStore
            {
                Tabs = tabs,
                ActiveTabId = activeTabId,
                TabSequence = tabSequence
            });
        }

        public async Task RemoveTabAsync(Guid tabId)
        {
            var store = await LoadAsync();
            store.Tabs.RemoveAll(t => t.Id == tabId);

            if (store.Tabs.Count == 0)
            {
                var tab = CreateBlankTab(Math.Max(1, store.TabSequence));
                store.Tabs.Add(tab);
                store.ActiveTabId = tab.Id;
                store.TabSequence = Math.Max(store.TabSequence, 2);
            }
            else if (store.ActiveTabId == tabId)
            {
                store.ActiveTabId = store.Tabs[0].Id;
            }

            await SaveAsync(store);
        }

        public static OfflinePosTab CreateBlankTab(int sequence) => new()
        {
            Id = Guid.NewGuid(),
            Title = $"Hóa đơn {sequence}",
            CreatedAt = DateTime.Now,
            IsGuest = true,
            IsShipping = false,
            PaymentMethod = "cash",
            Items = new List<OfflinePosLineItem>()
        };
    }
}
