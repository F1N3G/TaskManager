using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives; // ✅ AICI
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace TaskManager.Wpf;

public partial class MainWindow : Window
{
    private sealed class TaskItem : INotifyPropertyChanged
    {
        private string _title = "";
        private bool _isDone;

        public string Title
        {
            get => _title;
            set { if (_title != value) { _title = value; OnPropertyChanged(); } }
        }

        public bool IsDone
        {
            get => _isDone;
            set { if (_isDone != value) { _isDone = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private readonly ObservableCollection<TaskItem> _tasks = new()
    {
        new TaskItem { Title = "Tema LFC", IsDone = false },
        new TaskItem { Title = "Laborator MIP", IsDone = true },
        new TaskItem { Title = "Proiect licență", IsDone = false }
    };

    private ListView? _list;
    private TextBox? _input;

    public MainWindow()
    {
        EnsureConsole();
        Console.WriteLine("✅ MainWindow pornit.");

        // prinde orice excepție WPF neafișată și o scrie în consolă
        if (Application.Current != null)
        {
            Application.Current.DispatcherUnhandledException += (_, e) =>
            {
                Console.WriteLine("❌ DispatcherUnhandledException:");
                Console.WriteLine(e.Exception);
                e.Handled = true; // ca să nu îți închidă aplicația instant
            };
        }

        Title = "Task Manager";
        Width = 900;
        Height = 600;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F6FB"));

        Content = BuildUi();
    }

    private UIElement BuildUi()
    {
        var root = new Grid { Margin = new Thickness(16) };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        // Header
        var header = Card(new StackPanel
        {
            Children =
            {
                new TextBlock
                {
                    Text = "Task Manager",
                    FontSize = 22,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111827"))
                },
                new TextBlock
                {
                    Text = "Gestionează sarcinile zilnice",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))
                }
            }
        });
        Grid.SetRow(header, 0);
        root.Children.Add(header);

        // Toolbar
        var toolbar = new DockPanel { Margin = new Thickness(0, 12, 0, 12) };

        _input = new TextBox { MinWidth = 320, Height = 30, Margin = new Thickness(0, 0, 12, 0) };
        _input.KeyDown += (_, e) => { if (e.Key == Key.Enter) AddTask(); };
        DockPanel.SetDock(_input, Dock.Left);
        toolbar.Children.Add(_input);

        var btnAdd = new Button
        {
            Content = "+ Adaugă task",
            Padding = new Thickness(14, 6, 14, 6),
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2563EB")),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            Cursor = Cursors.Hand,
            FontWeight = FontWeights.SemiBold
        };
        btnAdd.Click += (_, __) => AddTask();
        DockPanel.SetDock(btnAdd, Dock.Left);
        toolbar.Children.Add(btnAdd);

        var btnToggle = new Button
        {
            Content = "Bifează / Debifează",
            Padding = new Thickness(14, 6, 14, 6),
            Margin = new Thickness(12, 0, 0, 0),
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            Cursor = Cursors.Hand,
            FontWeight = FontWeights.SemiBold
        };
        btnToggle.Click += (_, __) => ToggleSelectedDone();
        DockPanel.SetDock(btnToggle, Dock.Right);
        toolbar.Children.Add(btnToggle);

        var btnDelete = new Button
        {
            Content = "Șterge selectat",
            Padding = new Thickness(14, 6, 14, 6),
            Margin = new Thickness(12, 0, 0, 0),
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            Cursor = Cursors.Hand,
            FontWeight = FontWeights.SemiBold
        };
        btnDelete.Click += (_, __) => DeleteSelected();
        DockPanel.SetDock(btnDelete, Dock.Right);
        toolbar.Children.Add(btnDelete);

        Grid.SetRow(toolbar, 1);
        root.Children.Add(toolbar);

        // List
        _list = new ListView
        {
            ItemsSource = _tasks,
            ItemTemplate = TaskTemplate()
        };

        // (opțional) dublu-click doar dacă există item selectat
        _list.MouseDoubleClick += (_, __) =>
        {
            if (_list.SelectedItem != null)
                ToggleSelectedDone();
        };

        var listCard = Card(_list);
        Grid.SetRow(listCard, 2);
        root.Children.Add(listCard);

        return root;
    }

    private DataTemplate TaskTemplate()
    {
        var template = new DataTemplate(typeof(TaskItem));

        var panel = new FrameworkElementFactory(typeof(DockPanel));
        panel.SetValue(DockPanel.LastChildFillProperty, true);
        panel.SetValue(FrameworkElement.MarginProperty, new Thickness(6, 4, 6, 4));

        var cb = new FrameworkElementFactory(typeof(CheckBox));
        cb.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 10, 0));
        cb.SetValue(DockPanel.DockProperty, Dock.Left);
        cb.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(TaskItem.IsDone))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });

        var txt = new FrameworkElementFactory(typeof(TextBlock));
        txt.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
        txt.SetValue(TextBlock.FontSizeProperty, 14.0);
        txt.SetBinding(TextBlock.TextProperty, new Binding(nameof(TaskItem.Title)));

        var textStyle = new Style(typeof(TextBlock));
        textStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty,
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111827"))));

        var doneTrigger = new DataTrigger
        {
            Binding = new Binding(nameof(TaskItem.IsDone)),
            Value = true
        };
        doneTrigger.Setters.Add(new Setter(TextBlock.ForegroundProperty,
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))));
        doneTrigger.Setters.Add(new Setter(TextBlock.TextDecorationsProperty, TextDecorations.Strikethrough));

        textStyle.Triggers.Add(doneTrigger);
        txt.SetValue(FrameworkElement.StyleProperty, textStyle);

        panel.AppendChild(cb);
        panel.AppendChild(txt);

        template.VisualTree = panel;
        return template;
    }

    private Border Card(UIElement child) => new()
    {
        Background = Brushes.White,
        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB")),
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(12),
        Padding = new Thickness(12),
        Child = child
    };

    private void AddTask()
    {
        if (_input == null) return;
        var title = (_input.Text ?? "").Trim();
        if (title.Length == 0) return;

        _tasks.Add(new TaskItem { Title = title, IsDone = false });
        Console.WriteLine($"➕ Added: {title}");

        _input.Text = "";
        _input.Focus();
    }

    private void ToggleSelectedDone()
    {
        if (_list?.SelectedItem is not TaskItem item) return;
        item.IsDone = !item.IsDone; // cu INotifyPropertyChanged, UI se actualizează singur
        Console.WriteLine($"✅ Toggled: {item.Title} -> {item.IsDone}");
    }

    private void DeleteSelected()
    {
        if (_list?.SelectedItem is not TaskItem item) return;
        _tasks.Remove(item);
        Console.WriteLine($"🗑️ Deleted: {item.Title}");
    }

    // Console for WPF
    private static void EnsureConsole()
    {
        if (!AttachConsole(ATTACH_PARENT_PROCESS))
            AllocConsole();

        try
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
        }
        catch { }
    }

    private const int ATTACH_PARENT_PROCESS = -1;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();
}
