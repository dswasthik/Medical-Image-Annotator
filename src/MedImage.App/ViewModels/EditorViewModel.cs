using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedImage.App.Services;
using MedImage.Domain.Entities;
using MedImage.Domain.Imaging;
using MedImage.Domain.Interfaces;

namespace MedImage.App.ViewModels;

public partial class EditorViewModel : ObservableObject
{
    private readonly IImageProcessingService _imaging;
    private readonly IUnitOfWork _uow;
    private readonly ISessionService _session;
    private readonly IDialogService _dialog;
    private readonly INavigationService _nav;

    private bool _suspendRender;

    public EditorViewModel(
        IImageProcessingService imaging,
        IUnitOfWork uow,
        ISessionService session,
        IDialogService dialog,
        INavigationService nav)
    {
        _imaging = imaging;
        _uow = uow;
        _session = session;
        _dialog = dialog;
        _nav = nav;
        Welcome = _session.CurrentUser is { } u ? $"Signed in as {u.Username}" : string.Empty;
        _ = RefreshHistoryAsync();
    }

    // ---- displayed data ----
    [ObservableProperty] private ImageSource? _displayImage;
    [ObservableProperty] private string _welcome = string.Empty;
    [ObservableProperty] private string _studyTitle = string.Empty;
    [ObservableProperty] private string _patientRef = string.Empty;
    [ObservableProperty] private int _imageWidth;
    [ObservableProperty] private int _imageHeight;

    public bool HasImage => _imaging.HasImage;

    // ---- adjustments (each change triggers a re-render) ----
    [ObservableProperty] private bool _grayscale;
    [ObservableProperty] private double _contrast = 1.0;
    [ObservableProperty] private double _brightness = 0.0;
    [ObservableProperty] private bool _windowLevelEnabled;
    [ObservableProperty] private double _window = 255;
    [ObservableProperty] private double _level = 127;
    [ObservableProperty] private bool _histogramEqualize;
    [ObservableProperty] private bool _sharpen;
    [ObservableProperty] private bool _invert;

    partial void OnGrayscaleChanged(bool v) => Rerender();
    partial void OnContrastChanged(double v) => Rerender();
    partial void OnBrightnessChanged(double v) => Rerender();
    partial void OnWindowLevelEnabledChanged(bool v) => Rerender();
    partial void OnWindowChanged(double v) => Rerender();
    partial void OnLevelChanged(double v) => Rerender();
    partial void OnHistogramEqualizeChanged(bool v) => Rerender();
    partial void OnSharpenChanged(bool v) => Rerender();
    partial void OnInvertChanged(bool v) => Rerender();

    public ObservableCollection<MeasurementItem> Measurements { get; } = new();
    public ObservableCollection<StudyListItem> History { get; } = new();

    private string? _originalPath;

    public EditorViewModel Init()
    {
        Welcome = _session.CurrentUser is { } u ? $"Signed in as {u.Username}" : string.Empty;
        return this;
    }

    private ImageAdjustments BuildAdjustments() => new()
    {
        Grayscale = Grayscale,
        Contrast = Contrast,
        Brightness = Brightness,
        WindowLevelEnabled = WindowLevelEnabled,
        Window = Window,
        Level = Level,
        HistogramEqualize = HistogramEqualize,
        Sharpen = Sharpen,
        Invert = Invert
    };

    private void Rerender()
    {
        if (_suspendRender || !_imaging.HasImage) return;
        try
        {
            byte[] png = _imaging.RenderPng(BuildAdjustments());
            DisplayImage = BitmapHelper.FromPng(png);
        }
        catch (Exception ex)
        {
            _dialog.Error("Render failed: " + ex.Message);
        }
    }

    [RelayCommand]
    private void OpenImage()
    {
        var path = _dialog.OpenImage();
        if (path is null) return;
        try
        {
            _imaging.LoadOriginal(path);
            _originalPath = path;
            ImageWidth = _imaging.Width;
            ImageHeight = _imaging.Height;
            Measurements.Clear();
            ResetAdjustments();
            OnPropertyChanged(nameof(HasImage));
            if (string.IsNullOrWhiteSpace(StudyTitle))
                StudyTitle = Path.GetFileNameWithoutExtension(path);
            Rerender();
        }
        catch (Exception ex)
        {
            _dialog.Error("Could not open image: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Reset()
    {
        ResetAdjustments();
        Rerender();
    }

    private void ResetAdjustments()
    {
        _suspendRender = true;
        Grayscale = false;
        Contrast = 1.0;
        Brightness = 0.0;
        WindowLevelEnabled = false;
        Window = 255;
        Level = 127;
        HistogramEqualize = false;
        Sharpen = false;
        Invert = false;
        _suspendRender = false;
    }

    // Called by the view when the user drags a measurement line (image pixels).
    public void AddMeasurement(double x1, double y1, double x2, double y2)
    {
        Measurements.Add(new MeasurementItem { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2 });
    }

    [RelayCommand]
    private void ClearMeasurements() => Measurements.Clear();

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!_imaging.HasImage || _originalPath is null)
        {
            _dialog.Error("Open an image first.");
            return;
        }
        if (string.IsNullOrWhiteSpace(StudyTitle))
        {
            _dialog.Error("Please enter a study title.");
            return;
        }

        var draws = Measurements.Select(m =>
            new AnnotationDraw(AnnotationType.Measurement, m.X1, m.Y1, m.X2, m.Y2, m.Label));

        var adjustments = BuildAdjustments();
        byte[] png = _imaging.ExportWithAnnotations(adjustments, draws);

        var savePath = _dialog.SavePng(StudyTitle + "_processed.png");
        if (savePath is null) return;

        try
        {
            _imaging.SavePng(png, savePath);

            var study = new Study
            {
                UserId = _session.CurrentUser!.Id,
                Title = StudyTitle.Trim(),
                PatientRef = string.IsNullOrWhiteSpace(PatientRef) ? null : PatientRef.Trim(),
                OriginalImagePath = _originalPath,
                ProcessedImagePath = savePath,
                AdjustmentsJson = JsonSerializer.Serialize(adjustments)
            };
            foreach (var m in Measurements)
            {
                study.Annotations.Add(new Annotation
                {
                    Type = AnnotationType.Measurement,
                    GeometryJson = JsonSerializer.Serialize(new { m.X1, m.Y1, m.X2, m.Y2 }),
                    Label = m.Label,
                    LengthPixels = m.LengthPixels
                });
            }

            await _uow.Studies.AddAsync(study);
            await _uow.SaveChangesAsync();
            _dialog.Info("Study saved.");
            await RefreshHistoryAsync();
        }
        catch (Exception ex)
        {
            _dialog.Error("Save failed: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task RefreshHistoryAsync()
    {
        if (_session.CurrentUser is null) return;
        History.Clear();
        var studies = await _uow.Studies.GetForUserAsync(_session.CurrentUser.Id);
        foreach (var s in studies)
            History.Add(new StudyListItem(s.Id, s.Title, s.PatientRef, s.CreatedAt));
    }

    [RelayCommand]
    private async Task OpenStudyAsync(StudyListItem? item)
    {
        if (item is null) return;
        var study = await _uow.Studies.GetWithAnnotationsAsync(item.Id);
        if (study is null) return;

        try
        {
            _imaging.LoadOriginal(study.OriginalImagePath);
            _originalPath = study.OriginalImagePath;
            ImageWidth = _imaging.Width;
            ImageHeight = _imaging.Height;
            StudyTitle = study.Title;
            PatientRef = study.PatientRef ?? string.Empty;

            var adj = JsonSerializer.Deserialize<ImageAdjustments>(study.AdjustmentsJson) ?? new ImageAdjustments();
            _suspendRender = true;
            Grayscale = adj.Grayscale; Contrast = adj.Contrast; Brightness = adj.Brightness;
            WindowLevelEnabled = adj.WindowLevelEnabled; Window = adj.Window; Level = adj.Level;
            HistogramEqualize = adj.HistogramEqualize; Sharpen = adj.Sharpen; Invert = adj.Invert;
            _suspendRender = false;

            Measurements.Clear();
            foreach (var a in study.Annotations)
            {
                var g = JsonSerializer.Deserialize<Geo>(a.GeometryJson);
                if (g is not null)
                    Measurements.Add(new MeasurementItem { X1 = g.X1, Y1 = g.Y1, X2 = g.X2, Y2 = g.Y2 });
            }
            OnPropertyChanged(nameof(HasImage));
            Rerender();
        }
        catch (Exception ex)
        {
            _dialog.Error("Could not open study (the original image may have moved): " + ex.Message);
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _session.CurrentUser = null;
        _nav.NavigateTo<LoginViewModel>();
    }

    private record Geo(double X1, double Y1, double X2, double Y2);
}
