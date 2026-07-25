using System.Text.Json;

using CommunityToolkit.Mvvm.Messaging;

namespace PetAgeCounter;

public partial class SettingsPage : ContentPage
{
    readonly PetItemDatabase database;

    public SettingsPage(PetItemDatabase petItemDatabase)
    {
        InitializeComponent();
        database = petItemDatabase;
    }

    async void OnExportClicked(object? sender, EventArgs e)
    {
        try
        {
            var items = await database.GetItemsAsync();
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            var file = Path.Combine(FileSystem.CacheDirectory, "pets_export.json");
            await File.WriteAllTextAsync(file, json);
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Экспорт списка",
                File = new ShareFile(file)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ошибка", $"Не удалось экспортировать список: {ex.Message}", "OK");
        }
    }

    async void OnImportClicked(object? sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Выберите файл",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                })
            });

            if (result == null)
                return;

            await using var stream = await result.OpenReadAsync();
            var items = await JsonSerializer.DeserializeAsync<List<PetItem>>(stream);
            if (items == null || items.Count == 0)
            {
                await DisplayAlertAsync("Импорт", "Файл не содержит записей.", "OK");
                return;
            }

            await database.ImportItemsAsync(items);
            WeakReferenceMessenger.Default.Send(new ItemsImportedMessage());
            await DisplayAlertAsync("Импорт", $"Импортировано записей: {items.Count}", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ошибка", $"Не удалось импортировать список: {ex.Message}", "OK");
        }
    }
}

public record ItemsImportedMessage();
