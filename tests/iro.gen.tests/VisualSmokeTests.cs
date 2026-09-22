using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using IroGen;

namespace IroGenTests;

public class VisualSmokeTests
{
    [Fact]
    public void WindowGeneratesPreviewAndCanRenderAtMinimumSize()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
                var application = new App();
                application.InitializeComponent();
                application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                string testRoot = Path.Combine(Path.GetTempPath(), "iro-ui-analysis-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(testRoot);
                File.WriteAllText(Path.Combine(testRoot, "iro.slnx"), "<Solution/>");
                File.WriteAllText(Path.Combine(testRoot, "IRO-KONSOLIDIERTER-PLAN.md"), "Isolierter UI-Test");
                var window = new MainWindow(testRoot);
                window.Show();
                var status = (TextBlock)window.FindName("Status");
                var loop = new DispatcherFrame();
                DateTime deadline = DateTime.UtcNow.AddSeconds(25);
                var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                timer.Tick += (_, _) => { if (status.Text.StartsWith("Vorschau fertig") || DateTime.UtcNow > deadline) loop.Continue = false; };
                timer.Start(); Dispatcher.PushFrame(loop); timer.Stop();
                Assert.StartsWith("Vorschau fertig", status.Text);
                Assert.NotNull(((System.Windows.Controls.Image)window.FindName("Preview")).Source);
                Assert.Equal(7, ((DataGrid)window.FindName("Results")).Items.Count);
                ((TextBox)window.FindName("SeriesCount")).Text = "11";
                ((Button)window.FindName("GenerateButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                var seriesLoop = new DispatcherFrame();
                deadline = DateTime.UtcNow.AddSeconds(45);
                var seriesTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                seriesTimer.Tick += (_, _) => { if ((status.Text.StartsWith("Vorschau fertig") && ((ComboBox)window.FindName("BatchPicker")).Items.Count == 11) || DateTime.UtcNow > deadline) seriesLoop.Continue = false; };
                seriesTimer.Start(); Dispatcher.PushFrame(seriesLoop); seriesTimer.Stop();
                Assert.Equal(11, ((ComboBox)window.FindName("BatchPicker")).Items.Count);
                Assert.Contains("11/11", ((TextBlock)window.FindName("CoverageSummary")).Text);
                Assert.True(((Button)window.FindName("ExportButton")).IsEnabled);
                Assert.True(((Button)window.FindName("SendTestsButton")).IsEnabled);
                ((ComboBox)window.FindName("BatchPicker")).SelectedIndex = 10;
                Assert.Contains("Seed", ((TextBlock)window.FindName("ImageInfo")).Text);
                ((Button)window.FindName("SendTestsButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                var analysisLoop = new DispatcherFrame();
                deadline = DateTime.UtcNow.AddSeconds(45);
                var analysisTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                analysisTimer.Tick += (_, _) =>
                {
                    if (((TextBlock)window.FindName("HandoffStatus")).Text.StartsWith("Analyse abgeschlossen") || DateTime.UtcNow > deadline) analysisLoop.Continue = false;
                };
                analysisTimer.Start(); Dispatcher.PushFrame(analysisLoop); analysisTimer.Stop();
                Assert.Equal(Visibility.Visible, ((FrameworkElement)window.FindName("TestCompletion")).Visibility);
                Assert.Equal(Visibility.Collapsed, ((FrameworkElement)window.FindName("TestBusyIndicator")).Visibility);
                Assert.StartsWith("Tests vollständig durchgelaufen: 11 Bilder", ((TextBlock)window.FindName("TestCompletionText")).Text);
                Assert.True(((Button)window.FindName("OpenTestResultsButton")).IsEnabled);
                Assert.StartsWith("Analyse abgeschlossen: 11/11", ((TextBlock)window.FindName("HandoffStatus")).Text);
                Assert.Single(Directory.GetFiles(Path.Combine(testRoot, "tests", "runs"), "results.json", SearchOption.AllDirectories));
                var rows = ((DataGrid)window.FindName("Results")).Items;
                Assert.Contains(rows.Cast<object>(), r => (string?)r.GetType().GetProperty("ActualText")!.GetValue(r) != "–");
                ((ComboBox)window.FindName("BatchPicker")).SelectedIndex = 0;
                Assert.Contains(rows.Cast<object>(), r => (string?)r.GetType().GetProperty("ActualText")!.GetValue(r) != "–");
                foreach (var size in new[] { (1340, 900, "normal"), (1000,650,"small") })
                {
                    window.Width = size.Item1; window.Height = size.Item2; window.UpdateLayout();
                    window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
                    window.UpdateLayout();
                    var content = (FrameworkElement)window;
                    var bitmap = new RenderTargetBitmap((int)content.ActualWidth,(int)content.ActualHeight,96,96,PixelFormats.Pbgra32);
                    bitmap.Render(content);
                    string? output = Environment.GetEnvironmentVariable("IROGEN_VISUAL_OUTPUT");
                    if (output != null)
                    {
                        Directory.CreateDirectory(output);
                        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        using var stream = File.Create(Path.Combine(output,"ui-"+size.Item3+".png")); encoder.Save(stream);
                    }
                }
                // Reproduce the reported workflow with the real 113-image plan and an older run present.
                var plan = TestPlan.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "testplans", "bildqualitaet-und-abstand.json")));
                var generate = (Task)typeof(MainWindow).GetMethod("GenerateSeriesAsync",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                    .Invoke(window, new object?[] { null, plan })!;
                var planLoop = new DispatcherFrame();
                deadline = DateTime.UtcNow.AddSeconds(90);
                var planTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                planTimer.Tick += (_, _) => { if (generate.IsCompleted || DateTime.UtcNow > deadline) planLoop.Continue = false; };
                planTimer.Start(); Dispatcher.PushFrame(planLoop); planTimer.Stop();
                Assert.True(generate.IsCompletedSuccessfully);
                Assert.Equal(113, ((ComboBox)window.FindName("BatchPicker")).Items.Count);
                Assert.Equal("113", ((TextBox)window.FindName("SeriesCount")).Text);

                Exception? reviewFailure = null;
                bool reviewed = false;
                var reviewLoop = new DispatcherFrame();
                deadline = DateTime.UtcNow.AddSeconds(90);
                var reviewTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                reviewTimer.Tick += (_, _) =>
                {
                    var review = application.Windows.OfType<TestResultsWindow>().FirstOrDefault();
                    if (review != null)
                    {
                        try
                        {
                            Assert.Equal(113, ((DataGrid)review.FindName("Rows")).Items.Count);
                            Assert.StartsWith("Aktuelle Serie · 113 Bilder", ((TextBlock)review.FindName("Summary")).Text);
                            var selectedRows = ((DataGrid)review.FindName("Rows")).Items.Cast<TestRunRow>().ToArray();
                            Assert.Single(selectedRows.Select(r => r.RunId).Distinct());
                            Assert.All(selectedRows, r => Assert.Equal("0.2.0", r.AnalyzerVersion));
                            Assert.Contains("Bilder insgesamt: 113", TestReviewExport.Create(selectedRows, 1, false));
                            reviewed = true;
                        }
                        catch (Exception error) { reviewFailure = error; }
                        finally { review.Close(); reviewLoop.Continue = false; }
                    }
                    if (DateTime.UtcNow > deadline) reviewLoop.Continue = false;
                };
                reviewTimer.Start();
                ((Button)window.FindName("ReviewTestsButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                if (!reviewed && reviewFailure == null) Dispatcher.PushFrame(reviewLoop);
                reviewTimer.Stop();
                if (reviewFailure != null) throw reviewFailure;
                Assert.True(reviewed, "Current-series review did not open.");
                Assert.Equal(2, Directory.GetFiles(Path.Combine(testRoot, "tests", "runs"), "results.json", SearchOption.AllDirectories).Length);
                string cacheRoot = Directory.GetParent(((BatchItem)((ComboBox)window.FindName("BatchPicker")).Items[0]).Folder)!.FullName;
                ((TextBox)window.FindName("SeriesCount")).Text = "500";
                ((Button)window.FindName("GenerateButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                window.Close();
                var closeLoop = new DispatcherFrame();
                deadline = DateTime.UtcNow.AddSeconds(10);
                var closeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                closeTimer.Tick += (_, _) => { if (!window.IsVisible || DateTime.UtcNow > deadline) closeLoop.Continue = false; };
                closeTimer.Start(); Dispatcher.PushFrame(closeLoop); closeTimer.Stop();
                Assert.False(window.IsVisible);
                Assert.False(Directory.Exists(cacheRoot));
                application.Shutdown();
                Directory.Delete(testRoot, true);
            }
            catch(Exception e) { failure=e; }
        });
        thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();
        if(failure!=null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }

    [Fact]
    public void CombinedScenariosProduceReviewablePackages()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            string? requested = Environment.GetEnvironmentVariable("IROGEN_VISUAL_OUTPUT");
            string output = requested ?? Path.Combine(Path.GetTempPath(),"irogen-review-"+Guid.NewGuid().ToString("N"));
            try
            {
                var normal = new GeneratorOptions();
                var cases = new Dictionary<string,GeneratorOptions>
                {
                    ["normal"] = normal,
                    ["horizontal"] = normal with { Orientation=StripOrientation.Horizontal, Position=StripPosition.Bottom,WallDifference=WallDifference.Opposite,Seed=837 },
                    ["perspective"] = normal with { Perspective=Severity.Medium,RotationDegrees=12,Position=StripPosition.Center },
                    ["glare-shadow"] = normal with { Glare=Severity.Strong,Shadows=Severity.Medium,Texture=Severity.Light },
                    ["blur-dirt"] = normal with { Blur=Severity.Medium,Dirt=Severity.Medium,MotionBlur=Severity.Light },
                    ["close"] = normal with { Distance=CameraDistance.TooClose },
                    ["far"] = normal with { Distance=CameraDistance.TooFar }
                };
                foreach(var (name,options) in cases)
                {
                    var scene = SceneGenerator.Generate(options);
                    string package = SceneExport.Save(scene,Path.Combine(output,name));
                    Assert.Single(Directory.GetFiles(package,"*.png"));
                    Assert.Single(Directory.GetFiles(package,"*.json"));
                }
            }
            catch(Exception e) { failure=e; }
            finally { if(requested==null && Directory.Exists(output)) Directory.Delete(output,true); }
        });
        thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();
        if(failure!=null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }
}







