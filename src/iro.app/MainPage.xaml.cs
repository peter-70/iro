using Iro.Core.Analysis;
namespace Iro.App;

public partial class MainPage : ContentPage
{
    private readonly AnalysisPresentation analysis = new();
    private bool requestingPermission;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = analysis;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshPermissionAsync();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (Window is not null)
        {
            Window.Resumed += OnWindowResumed;
            Window.Stopped += OnWindowStopped;
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        if (Window is not null)
        {
            Window.Resumed -= OnWindowResumed;
            Window.Stopped -= OnWindowStopped;
        }
        base.OnNavigatedFrom(args);
    }

    private void OnWindowStopped(object? sender, EventArgs e) => analysis.Cancel();

    private async void OnWindowResumed(object? sender, EventArgs e)
    {
        await RefreshPermissionAsync();
    }

    private async Task RefreshPermissionAsync()
    {
        try
        {
            ShowPermission(await Permissions.CheckStatusAsync<Permissions.Camera>());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            StatusLabel.Text = "Kameraberechtigung konnte nicht geprüft werden.";
        }
    }

    private void ShowPermission(PermissionStatus status)
    {
        bool granted = status == PermissionStatus.Granted;
        StatusLabel.Text = granted
            ? "Kamerazugriff erlaubt. Die Kameravorschau wird im nächsten Schritt ergänzt."
            : "Testbilder funktionieren ohne Kameraberechtigung. Für die spätere Live-Kamera wird sie benötigt.";
        PermissionButton.IsVisible = !granted;
        SettingsButton.IsVisible = !granted;
    }

    private async void OnRequestCameraClicked(object? sender, EventArgs e)
    {
        if (requestingPermission)
            return;

        requestingPermission = true;
        PermissionButton.IsEnabled = false;
        try
        {
            ShowPermission(await Permissions.RequestAsync<Permissions.Camera>());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            StatusLabel.Text = "Kamerazugriff konnte nicht angefordert werden. Bitte die Android-App-Einstellungen prüfen.";
            SettingsButton.IsVisible = true;
        }
        finally
        {
            requestingPermission = false;
            PermissionButton.IsEnabled = true;
        }
    }

    private async void OnTestImageClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string asset }) return;
        TestPreview.Source = ImageSource.FromStream(() => Android.App.Application.Context.Assets!.Open(asset));
        await analysis.AnalyzeAsync(token => TestImageLoader.LoadAsync(asset, token));
    }

    protected override void OnDisappearing()
    {
        analysis.Cancel();
        base.OnDisappearing();
    }

    private void OnOpenSettingsClicked(object? sender, EventArgs e)
    {
        AppInfo.Current.ShowSettingsUI();
    }
}



