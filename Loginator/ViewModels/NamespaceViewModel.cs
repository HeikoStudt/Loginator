using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Common;
using Loginator.ViewModels;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;

namespace LogApplication.ViewModels {

    public class NamespaceViewModel : INotifyPropertyChanged {

        private bool isChecked;
        public bool IsChecked {
            get { return isChecked; }
            set {
                isChecked = value;
                lock (ViewModelConstants.SYNC_OBJECT) {
                    if (ApplicationViewModel != null) {
                        ApplicationViewModel.UpdateByNamespaceChange(this);
                    }
                }
                
                if (Children != null) {
                    foreach (var child in Children) {
                        child.IsChecked = isChecked;
                    }
                }
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        private bool isExpanded;
        public bool IsExpanded {
            get {
                return isExpanded;
            }
            set {
                isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        private int count;
        public int Count {
            get {
                return count;
            }
            set {
                count = value;
                OnPropertyChanged(nameof(Count));
            }
        }

        private int countTrace;
        public int CountTrace {
            get {
                return countTrace;
            }
            set {
                countTrace = value;
                OnPropertyChanged(nameof(CountTrace));
            }
        }

        private int countDebug;
        public int CountDebug {
            get {
                return countDebug;
            }
            set {
                countDebug = value;
                OnPropertyChanged(nameof(CountDebug));
            }
        }

        private int countInfo;
        public int CountInfo {
            get {
                return countInfo;
            }
            set {
                countInfo = value;
                OnPropertyChanged(nameof(CountInfo));
            }
        }

        private int countWarn;
        public int CountWarn {
            get {
                return countWarn;
            }
            set {
                countWarn = value;
                OnPropertyChanged(nameof(CountWarn));
            }
        }

        private int countError;
        public int CountError {
            get {
                return countError;
            }
            set {
                countError = value;
                OnPropertyChanged(nameof(CountError));
            }
        }

        private int countFatal;

        public int CountFatal {
            get {
                return countFatal;
            }
            set {
                countFatal = value;
                OnPropertyChanged(nameof(CountFatal));
            }
        }

        private bool isHighlighted;
        public bool IsHighlighted
        {
            get
            {
                return isHighlighted;
            }
            set
            {
                isHighlighted = value;
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }

        public string Name { get; set; }
        public NamespaceViewModel Parent { get; set; }
        public ObservableCollection<NamespaceViewModel> Children { get; set; }

        private ApplicationViewModel ApplicationViewModel { get; set; }
        private LoginatorViewModel LoginatorViewModel { get; }

        public NamespaceViewModel(string name, ApplicationViewModel applicationViewModel, LoginatorViewModel loginatorViewModel) {
            IsChecked = true;
            IsExpanded = true;
            Name = name;
            Children = new ObservableCollection<NamespaceViewModel>();
            ApplicationViewModel = applicationViewModel;
            LoginatorViewModel = loginatorViewModel;
        }

        public string Fullname {
            get {
                string fullname = Name;
                var parent = Parent;
                while (parent != null) {
                    fullname = parent.Name + Constants.NAMESPACE_SPLITTER + fullname;
                    parent = parent.Parent;
                }
                return fullname;
            }
        }

        public string NamespaceName {
            get {
                string fullname = Name;
                var parent = Parent;
                while (parent != null && parent.Name != ApplicationViewModel.Name) {
                    fullname = parent.Name + Constants.NAMESPACE_SPLITTER + fullname;
                    parent = parent.Parent;
                }
                return fullname;
            }
        }

        private ICommand disableAsDefaultCommand;
        public ICommand DisableAsDefaultCommand {
            get {
                return disableAsDefaultCommand ?? (disableAsDefaultCommand = new RelayCommand<NamespaceViewModel>(DisableAsDefault, CanDisableAsDefault));
            }
        }
        private bool CanDisableAsDefault(NamespaceViewModel serverRule) {
            return true;
        }
        private void DisableAsDefault(NamespaceViewModel serverRule) {
            try {
                LoginatorViewModel.CentrallyDisabledNamespacesViewModel.AddAsDefault(ApplicationViewModel.Name, NamespaceName);
                IsChecked = false;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK);
            }
        }

        private ICommand enableDisableCommand;
        public ICommand EnableDisableCommand {
            get {
                return enableDisableCommand ?? (enableDisableCommand = new RelayCommand<NamespaceViewModel>(EnableDisable, CanEnableDisable));
            }
        }
        private bool CanEnableDisable(NamespaceViewModel serverRule) {
            return true;
        }
        private void EnableDisable(NamespaceViewModel serverRule) {
            IsChecked = !IsChecked;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
