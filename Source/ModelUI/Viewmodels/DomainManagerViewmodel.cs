using ModelUI.Commands;
using ModelUI.Models;
using ModelUI.Utilities;
using ModelUI.Viewmodels.Base;
using Scm.Focus.Utils.ModelGenerator;
using Scm.Focus.Utils.ModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModelUI.Viewmodels
{
    public class DomainManagerViewmodel : BaseViewmodel
    {
        public string NewDomainPath
        {
            get
            {
                if (!string.IsNullOrEmpty(NewDomainName))
                {
                    return FileManager.GetDestionationDomainPath(NewDomainName);
                }
                return null;
            }
        }


        private string _newDomainName = null;
        public string NewDomainName
        {
            get
            {
                return _newDomainName;
            }
            set
            {
                _newDomainName = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewDomainName"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewDomainPath"));
                RaiseCanExecuteChanged();
            }
        }


        private bool _isAddingNewDomain = false;
        public bool IsAddingNewDomain
        {
            get
            {
                return _isAddingNewDomain;
            }
            set
            {
                _isAddingNewDomain = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsAddingNewDomain"));
                RaiseCanExecuteChanged();
            }
        }

        private bool _isEditingDomain = false;
        public bool IsEditingDomain
        {
            get
            {
                return _isEditingDomain;
            }
            set
            {
                _isEditingDomain = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsEditingDomain"));
                RaiseCanExecuteChanged();
            }
        }

        private bool _isDeletingDomain = false;
        public bool IsDeletingDomain
        {
            get
            {
                return _isDeletingDomain;
            }
            set
            {
                _isDeletingDomain = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsDeletingDomain"));
                RaiseCanExecuteChanged();
            }
        }

        private List<Domain> _domains = null;
        public List<Domain> Domains
        {
            get
            {
                return _domains;
            }
            set
            {
                _domains = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Domains"));
                UpdateDomainsCollection();
            }
        }

        private readonly ObservableCollection<Domain> _domainsCollection = new ObservableCollection<Domain>();
        public ObservableCollection<Domain> DomainsCollection
        {
            get
            {
                return _domainsCollection;
            }
        }



        private Domain _selectedDomain = null;
        public Domain SelectedDomain
        {
            get
            {
                return _selectedDomain;
            }
            set
            {
                _selectedDomain = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedDomain"));
                if (_selectedDomain != null)
                {
                    ModifyEntityRequest();
                }
                RaiseCanExecuteChanged();
            }
        }



        private Window _window;

        public DomainManagerViewmodel()
        {
            ReloadDomains();
        }

        private void ReloadDomains()
        {
            Domains = FileManager.GetDomains();
        }

        public void Initialize(Window window)
        {
            this._window = window;
            RegisterCommands();
        }



        private void UpdateDomainsCollection()
        {
            DomainsCollection.Clear();
            if (Domains != null)
            {
                foreach (var item in Domains)
                {
                    DomainsCollection.Add(item);
                }
            }
        }

        protected override void RegisterCommands()
        {
            Commands.Add("ShowDomainManagerCommand", ShowDomainManagerCommand);
            Commands.Add("AddNewDomainRequestCommand", AddNewDomainRequestCommand);
            Commands.Add("ModifyDomainRequestCommand", ModifyDomainRequestCommand);
            Commands.Add("DeleteDomainRequestCommand", DeleteDomainRequestCommand);
            Commands.Add("CancelRequestCommand", CancelRequestCommand);
            Commands.Add("ConfirmNewDomainRequestCommand", ConfirmNewDomainRequestCommand);

        }


        private ICommand _confirmNewDomainRequestCommand = null;
        public ICommand ConfirmNewDomainRequestCommand
        {
            get
            {
                if (_confirmNewDomainRequestCommand == null)
                {
                    _confirmNewDomainRequestCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            FileManager.CreateNewDomain(NewDomainName);
                            NewDomainName = null;
                            IsAddingNewDomain = false;
                            ReloadDomains();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty(NewDomainName)
                            && Domains.FirstOrDefault(k => k.DomainName == NewDomainName) == null;
                    });
                }
                return _confirmNewDomainRequestCommand;
            }
        }

        private ICommand _addNewDomainRequestCommand = null;
        public ICommand AddNewDomainRequestCommand
        {
            get
            {
                if (_addNewDomainRequestCommand == null)
                {
                    _addNewDomainRequestCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsAddingNewDomain = true;
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return true;
                    });
                }
                return _addNewDomainRequestCommand;
            }
        }

        private ICommand _cancelRequestCommand = null;
        public ICommand CancelRequestCommand
        {
            get
            {
                if (_cancelRequestCommand == null)
                {
                    _cancelRequestCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsDeletingDomain = false;
                            IsEditingDomain = false;
                            IsAddingNewDomain = false;
                            NewDomainName = null;
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return true;
                    });
                }
                return _cancelRequestCommand;
            }
        }

        private ICommand _modifyDomainRequestCommand = null;
        public ICommand ModifyDomainRequestCommand
        {
            get
            {
                if (_modifyDomainRequestCommand == null)
                {
                    _modifyDomainRequestCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsEditingDomain = true;
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return true;
                    });
                }
                return _modifyDomainRequestCommand;
            }
        }

        private ICommand _deleteDomainRequestCommand = null;
        public ICommand DeleteDomainRequestCommand
        {
            get
            {
                if (_deleteDomainRequestCommand == null)
                {
                    _deleteDomainRequestCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var response =
                                MessageBox.Show(
                                    $"Confirmas que deseas eliminar el dominio {SelectedDomain.DomainName}. Se eliminarán todas las carpetas y archivos asociados al dominio",
                                    "Eliminar dominio", 
                                    MessageBoxButton.OKCancel);
                            if (response == MessageBoxResult.OK)
                            {
                                FileManager.DeleteDomain(SelectedDomain.DomainName);
                                ReloadDomains();
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SelectedDomain != null;
                    });
                }
                return _deleteDomainRequestCommand;
            }
        }


        private ICommand _showDomainManagerCommand = null;
        public ICommand ShowDomainManagerCommand
        {
            get
            {
                if (_showDomainManagerCommand == null)
                {
                    _showDomainManagerCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return true;
                    });
                }
                return _showDomainManagerCommand;
            }
        }


        private void ModifyEntityRequest()
        {
            ICommand c = ModifyDomainRequestCommand;
            if (c.CanExecute(null))
            {
                c.Execute(null);
            }
        }


    }
}
