using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TaskManager.Core.Dtos;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Core.Storage;
using TaskManager.Wpf.Commands;

namespace TaskManager.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly TaskRepository _repo = new();
    private readonly TaskService _service;

    public ObservableCollection<WorkItem> Items { get; } = new();

    private WorkItem? _selected;
    public WorkItem? Selected
    {
        get => _selected;
        set { _selected = value; OnPropertyChanged(); RefreshCommands(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); ApplyFilter(); }
    }

    public ICommand AddTaskCommand { get; }
    public ICommand AddReminderCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ToggleCompleteCommand { get; }
    public ICommand ExportTextCommand { get; }
    public ICommand ImportTextCommand { get; }
    public ICommand ExportBinaryCommand { get; }
    public ICommand ImportBinaryCommand { get; }

    private readonly RelayCommand _delete;
    private readonly RelayCommand _toggle;

    public MainViewModel()
    {
        _service = new TaskService(_repo);

        // ✅ demo data (ca să vezi instant ceva în UI)
        _service.CreateTask(new TaskCreateDto
        {
            Title = "Finalizează UI WPF",
            Deadline = DateTime.Now.AddDays(2),
            Priority = PriorityLevel.High,
            Notes = "DataGrid + Commands + Import/Export"
        });

        _service.CreateTask(new TaskCreateDto
        {
            Title = "Pregătește prezentarea",
            Deadline = DateTime.Now.AddDays(4),
            Priority = PriorityLevel.Medium
        });

        _repo.Add(new ReminderItem(_service.GenerateId(), "Backup proiect", DateTime.Now.AddHours(6)));

        ReloadFromRepo();

        AddTaskCommand = new RelayCommand(_ => AddTask());
        AddReminderCommand = new RelayCommand(_ => AddReminder());

        _delete = new RelayCommand(_ => DeleteSelected(), _ => Selected != null);
        DeleteCommand = _delete;

        _toggle = new RelayCommand(_ => ToggleComplete(), _ => Selected is TaskItem);
        ToggleCompleteCommand = _toggle;

        ExportTextCommand = new RelayCommand(_ => ExportText());
        ImportTextCommand = new RelayCommand(_ => ImportText());
        ExportBinaryCommand = new RelayCommand(_ => ExportBinary());
        ImportBinaryCommand = new RelayCommand(_ => ImportBinary());
    }

    private void AddTask()
    {
        try
        {
            _service.CreateTask(new TaskCreateDto
            {
                Title = "Task nou",
                Deadline = DateTime.Now.AddDays(1),
                Priority = PriorityLevel.Medium
            });
            ReloadFromRepo();
        }
        catch (TaskManagerException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddReminder()
    {
        try
        {
            _repo.Add(new ReminderItem(_service.GenerateId(), "Reminder nou", DateTime.Now.AddHours(2)));
            ReloadFromRepo();
        }
        catch (TaskManagerException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteSelected()
    {
        if (Selected == null) return;
        _repo.Remove(Selected.Id);
        ReloadFromRepo();
    }

    private void ToggleComplete()
    {
        try
        {
            if (Selected is not TaskItem t) return;
            _service.ToggleComplete(t.Id);
            ReloadFromRepo();
        }
        catch (TaskManagerException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportText()
    {
        try
        {
            var dlg = new SaveFileDialog { Filter = "Text (*.txt)|*.txt", FileName = "tasks.txt" };
            if (dlg.ShowDialog() == true)
                TextFileStorage.Save(dlg.FileName, _repo.Items);
        }
        catch (TaskManagerException ex)
        {
            MessageBox.Show(ex.Message, "Export TXT error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ImportText()
    {
        try
        {
            var dlg = new OpenFileDialog { Filter = "Text (*.txt)|*.txt" };
            if (dlg.ShowDialog() == true)
            {
                var items = TextFileStorage.Load(dlg.FileName);
                ReplaceAll(items);
            }
        }
        catch (TaskManagerException ex)
        {
            MessageBox.Show(ex.Message, "Import TXT error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportBinary()
    {
        try
        {
            var dlg = new SaveFileDialog { Filter = "Binary (*.bin)|*.bin", FileName = "tasks.bin" };
            if (dlg.ShowDialog() == true)
                BinaryFileStorage.Save(dlg.FileName, _repo.Items);
        }
        catch (TaskManagerException ex)
        {
            MessageBox.Show(ex.Message, "Export BIN error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ImportBinary()
    {
        try
        {
            var dlg = new OpenFileDialog { Filter = "Binary (*.bin)|*.bin" };
            if (dlg.ShowDialog() == true)
            {
                var items = BinaryFileStorage.Load(dlg.FileName);
                ReplaceAll(items);
            }
        }
        catch (TaskManagerException ex)
        {
            MessageBox.Show(ex.Message, "Import BIN error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ReplaceAll(IEnumerable<WorkItem> items)
    {
        foreach (var it in _repo.Items.ToList())
            _repo.Remove(it.Id);

        foreach (var it in items)
            _repo.Add(it);

        ReloadFromRepo();
    }

    private void ReloadFromRepo()
    {
        Items.Clear();
        foreach (var it in _repo.Items)
            Items.Add(it);

        ApplyFilter();
        RefreshCommands();
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _repo.Items
            : _repo.Search(SearchText);

        Items.Clear();
        foreach (var it in filtered)
            Items.Add(it);
    }

    private void RefreshCommands()
    {
        _delete.RaiseCanExecuteChanged();
        _toggle.RaiseCanExecuteChanged();
    }
}
