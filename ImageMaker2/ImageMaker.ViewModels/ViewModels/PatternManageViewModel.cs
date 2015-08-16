using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;
using ImageMaker.ViewModels.ViewModels.Patterns;
using Microsoft.Win32;

namespace ImageMaker.ViewModels.ViewModels
{
    public class PatternManageViewModel : BaseViewModel
    {
        private readonly IPatternRepository _repository;
        private readonly IMappingEngine _mappingEngine;
        private bool _canRemove;
        private bool _canSave;

        private RelayCommand _addCommand;
        private RelayCommand _removeCommand;
        private RelayCommand _saveChangesCommand;
        private RelayCommand<FilterEventArgs> _filterCommand;
        private RelayCommand _checkCommand;

        public PatternViewModel Pattern { get; private set; }

        public PatternManageViewModel(PatternViewModel pattern, IPatternRepository repository, IMappingEngine mappingEngine)
        {
            _repository = repository;
            _mappingEngine = mappingEngine;
            Pattern = pattern;
            Children = new ObservableCollection<CheckablePatternDataViewModel>(pattern.Children.Select(x => x.ToCheckable()));
        }

        public ObservableCollection<CheckablePatternDataViewModel> Children { get; private set; }

        public RelayCommand SaveChangesCommand
        {
            get { return _saveChangesCommand ?? (_saveChangesCommand = new RelayCommand(SaveChanges, CanSaveChanges)); }
        }

        public RelayCommand CheckCommand
        {
            get { return _checkCommand ?? (_checkCommand = new RelayCommand(CheckItem)); }
        }

        private void CheckItem()
        {
            _canRemove = Children.Any(x => x.IsChecked);
            _canSave = Children.Any(x => x.State != ItemState.Unchanged);

            UpdateCommands();
        }

        public bool HasChanges { get { return _canSave; } }

        private bool CanSaveChanges()
        {
            return _canSave;
        }

        private void SaveChanges()
        {
            var removed = Children.Where(x => x.State == ItemState.Removed).ToList();
            removed.ForEach(x => Children.Remove(x));

            var added = Children.Where(x => x.State == ItemState.Added).ToList();
            added.ForEach(x => x.State = ItemState.Unchanged);

            _repository.RemovePatternsData(removed.Select(_mappingEngine.Map<PatternData>));
            _repository.AddPatternsData(added.Select(_mappingEngine.Map<PatternData>));
            _repository.Commit();

            CheckItem();
        }

        public RelayCommand RemoveCommand
        {
            get { return _removeCommand ?? (_removeCommand = new RelayCommand(RemovePatterns, CanRemovePatterns)); }
        }

        private bool CanRemovePatterns()
        {
            return _canRemove;
        }

        private void RemovePatterns()
        {
            foreach (var child in Children.Where(x => x.IsChecked).ToList())
            {
                child.IsChecked = false;
                if (child.State == ItemState.Added)
                    Children.Remove(child);
                else
                {
                    child.State = ItemState.Removed;
                }
            }

            CheckItem();
        }

        public RelayCommand AddCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(AddPatterns)); }
        }

        public RelayCommand<FilterEventArgs> FilterCommand
        {
            get { return _filterCommand ?? (_filterCommand = new RelayCommand<FilterEventArgs>(Filter)); }
        }

        private void Filter(FilterEventArgs args)
        {
            CheckablePatternDataViewModel item = (CheckablePatternDataViewModel) args.Item;
            args.Accepted = item.State != ItemState.Removed;
        }

        private void AddPatterns()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true,
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg"
            };

            Func<string, bool> isValidFile = file =>
            {
                string[] extensions =
                {
                    ".png",
                    ".jpeg",
                    ".jpg"
                };

                string ext = Path.GetExtension(file);
                return extensions.Contains(ext);
            };

            fileDialog.FileOk += (sender, args) =>
            {
                if (!fileDialog.FileNames.All(isValidFile))
                    args.Cancel = true;
            };

            bool? dialogResult = fileDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                try
                {
                    Stream[] files = fileDialog.OpenFiles();
                    for (int i = 0; i < fileDialog.FileNames.Length; i++)
                    {
                        string filePath = fileDialog.FileNames[i];
                        Stream file = files[i];
                        byte[] fileData = null;

                        using (var stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            fileData = stream.ToArray();
                        }

                        CheckablePatternDataViewModel viewModel = new PatternDataViewModel(Path.GetFileName(filePath), Pattern.PatternType, fileData)
                            .ToCheckable(ItemState.Added);
                        
                        Children.Add(viewModel);
                    }
                }
                catch (Exception ex)
                {
                }


                CheckItem();
            }
        }

        private void UpdateCommands()
        {
            RemoveCommand.RaiseCanExecuteChanged();
            SaveChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public class CheckablePatternDataViewModel : PatternDataViewModel
    {
        private bool _isChecked;
        private ItemState _state;

        public CheckablePatternDataViewModel(PatternDataViewModel viewModel)
            : base(viewModel.Name, viewModel.PatternType, viewModel.Data)
        {
            Id = viewModel.Id;
        }

        public CheckablePatternDataViewModel(PatternDataViewModel viewModel, ItemState state)
            : this(viewModel)
        {
            State = state;
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }

        public ItemState State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged();
            }
        }
    }

    public enum ItemState
    {
        Unchanged = 0,
        Added = 1,
        Removed = 2
    }

    public static class Extensions
    {
        public static CheckablePatternDataViewModel ToCheckable(this PatternDataViewModel source)
        {
            return new CheckablePatternDataViewModel(source);
        }

        public static CheckablePatternDataViewModel ToCheckable(this PatternDataViewModel source, ItemState state)
        {
            return new CheckablePatternDataViewModel(source, state);
        }
    }
}
