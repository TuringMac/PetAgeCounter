using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;

namespace PetAgeCounter
{
    public partial class MainPage : ContentPage
    {
        enum ListFilter
        {
            Alive,
            Deceased
        }

        readonly PetItemDatabase petItemDatabase;
        ListFilter currentFilter = ListFilter.Alive;

        public ObservableCollection<PetItem> Persons { get; set; } = new();

        public MainPage(PetItemDatabase Database)
        {
            InitializeComponent();
            petItemDatabase = Database;
            BindingContext = this;
            WeakReferenceMessenger.Default.Register<PetItem>(this, async (_, _) => await LoadPetItemsAsync());
            WeakReferenceMessenger.Default.Register<ItemsImportedMessage>(this, async (_, _) => await LoadPetItemsAsync());
            UpdateFilterUi();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPetItemsAsync();
        }


        async void OnDeleteSwipeItemInvoked(object? sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is PetItem item)
            {
                await petItemDatabase.DeleteItemAsync(item);
                await LoadPetItemsAsync();
            }
        }

        async void OnEditSwipeItemInvoked(object? sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is PetItem item)
                await Navigation.PushAsync(new AddEditItemPage(petItemDatabase, item));
        }

        async void OnAddItemClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditItemPage(petItemDatabase));
        }

        async void OnAliveFilterClicked(object? sender, EventArgs e)
        {
            currentFilter = ListFilter.Alive;
            UpdateFilterUi();
            await LoadPetItemsAsync();
        }

        async void OnDeceasedFilterClicked(object? sender, EventArgs e)
        {
            currentFilter = ListFilter.Deceased;
            UpdateFilterUi();
            await LoadPetItemsAsync();
        }

        async void OnSettingsClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage(petItemDatabase));
        }

        void UpdateFilterUi()
        {
            var activeColor = Color.FromArgb("#E8E8E8");
            var inactiveColor = Colors.Transparent;

            AliveFilterButton.BackgroundColor = currentFilter == ListFilter.Alive ? activeColor : inactiveColor;
            DeceasedFilterButton.BackgroundColor = currentFilter == ListFilter.Deceased ? activeColor : inactiveColor;
            AddButton.IsVisible = currentFilter == ListFilter.Alive;
        }

        bool MatchesFilter(PetItem item) =>
            currentFilter == ListFilter.Alive ? item.IsAlive : item.IsDeceased;

        async Task LoadPetItemsAsync()
        {
            Persons.Clear();
            var itemsFromDb = await petItemDatabase.GetItemsAsync();
            foreach (var item in itemsFromDb.Where(MatchesFilter))
                Persons.Add(item);

            var sorted = Persons.OrderByDescending(item => item.Offset.diff).ToList();
            for (int i = 0; i < sorted.Count; i++)
                Persons.Move(Persons.IndexOf(sorted[i]), i);
        }
    }
}
