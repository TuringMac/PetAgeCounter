using CommunityToolkit.Mvvm.Messaging;

namespace AgeCounter;

public partial class AddEditItemPage : ContentPage
{
    PetItemDatabase database;
    public PetItem CurrentItem { get; set; }

    public AddEditItemPage(PetItemDatabase petItemDatabase, PetItem item = null)
    {
        InitializeComponent();
        database = petItemDatabase;
        CurrentItem = item ?? new PetItem() { BirthDate = DateTime.Now }; // If no item passed, create new
        BindingContext = CurrentItem;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await database.SaveItemAsync(CurrentItem);
        WeakReferenceMessenger.Default.Send(CurrentItem);
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}