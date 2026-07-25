using CommunityToolkit.Mvvm.Messaging;

namespace PetAgeCounter;

public partial class AddEditItemPage : ContentPage
{
    readonly PetItemDatabase database;

    public PetItem CurrentItem { get; set; }

    public AddEditItemPage(PetItemDatabase petItemDatabase, PetItem? item = null)
    {
        InitializeComponent();
        database = petItemDatabase;
        CurrentItem = item ?? new PetItem { BirthDate = DateTime.Today };
        BindingContext = CurrentItem;
        UpdateDeceasedUi();
    }

    void UpdateDeceasedUi()
    {
        MarkDeceasedButton.Opacity = CurrentItem.IsDeceased ? 1.0 : 0.35;
        DeathDatePicker.MinimumDate = CurrentItem.BirthDate;
        DeathDatePicker.MaximumDate = DateTime.Today;
    }

    void OnMarkDeceasedClicked(object? sender, EventArgs e)
    {
        CurrentItem.IsDeceased = !CurrentItem.IsDeceased;
        if (CurrentItem.IsDeceased &&
            (CurrentItem.DeathDate == DateTime.MaxValue || CurrentItem.DeathDate < CurrentItem.BirthDate))
        {
            CurrentItem.DeathDate = DateTime.Today;
        }
        UpdateDeceasedUi();
    }

    async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (CurrentItem.IsDeceased && CurrentItem.DeathDate < CurrentItem.BirthDate)
        {
            await DisplayAlertAsync("Ошибка", "Дата смерти не может быть раньше даты рождения.", "OK");
            return;
        }

        if (!CurrentItem.IsDeceased)
            CurrentItem.DeathDate = DateTime.MaxValue;

        await database.SaveItemAsync(CurrentItem);
        WeakReferenceMessenger.Default.Send(CurrentItem);
        await Navigation.PopAsync();
    }

    async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
