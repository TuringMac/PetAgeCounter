using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AgeCounter
{
    public partial class MainPage : ContentPage
    {
        PetItemDatabase petItemDatabase;
        public ObservableCollection<PetItem> Persons { get; set; } = new ObservableCollection<PetItem>();

        public MainPage(PetItemDatabase Database)
        {
            InitializeComponent();
            petItemDatabase = Database;
            BindingContext = this;
            WeakReferenceMessenger.Default.Register<PetItem>(this, (_, item) => Persons.Add(item));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPetItemsAsync();
        }

        private async void OnDeleteSwipeItemInvoked(object? sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem)
            {
                PetItem item = swipeItem.BindingContext as PetItem;
                if (item != null)
                {
                    await petItemDatabase.DeleteItemAsync(item);
                    await LoadPetItemsAsync();
                }
            }
        }

        private async void OnEditSwipeItemInvoked(object? sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem)
            {
                PetItem item = swipeItem.BindingContext as PetItem;
                if (item != null)
                    await Navigation.PushAsync(new AddEditItemPage(petItemDatabase, item));
            }
        }
        private async void OnAddItemClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditItemPage(petItemDatabase));
        }

        private async Task LoadPetItemsAsync()
        {
            Persons.Clear();
            var contactsFromDb = await petItemDatabase.GetItemsAsync();
            foreach (var contact in contactsFromDb)
            {
                Persons.Add(contact);
            }
            var sorted = Persons.OrderByDescending(item => item.Offset.diff).ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                Persons.Move(Persons.IndexOf(sorted[i]), i);
            }
        }
    }
}
