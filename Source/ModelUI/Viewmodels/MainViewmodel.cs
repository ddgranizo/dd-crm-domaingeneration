using ModelUI.Commands;
using ModelUI.Models;
using ModelUI.Utilities;
using ModelUI.Viewmodels.Base;
using ModelUI.Views;
using Scm.Focus.Utils.ModelGenerator;
using Scm.Focus.Utils.ModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModelUI.Viewmodels
{
    public class MainViewmodel : BaseViewmodel
    {




        private bool _isDefaultOutputFile = true;
        public bool IsDefaultOutputFile
        {
            get
            {
                return _isDefaultOutputFile;
            }
            set
            {
                _isDefaultOutputFile = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsDefaultOutputFile"));
                if (_isDefaultOutputFile)
                {
                    SelectedEntity.OutputFile = SelectedEntity.DefaultOutputFile;
                }
            }
        }


        private string _messageDialog = null;
        public string MessageDialog
        {
            get
            {
                return _messageDialog;
            }
            set
            {
                _messageDialog = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("MessageDialog"));
            }
        }
        private bool _isDialogOpen = false;
        public bool IsDialogOpen
        {
            get
            {
                return _isDialogOpen;
            }
            set
            {
                _isDialogOpen = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsDialogOpen"));
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
                RaiseCanExecuteChanged();
            }
        }



        private bool _isFilterByEntityEnabled = false;
        public bool IsFilterByEntityEnabled
        {
            get
            {
                return _isFilterByEntityEnabled;
            }
            set
            {
                _isFilterByEntityEnabled = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsFilterByEntityEnabled"));
                UpdateFilteredTargetEntities();
            }
        }


        private Domain _selectedFilterDomain = null;
        public Domain SelectedFilterDomain
        {
            get
            {
                return _selectedFilterDomain;
            }
            set
            {
                var oldValue = _selectedFilterDomain;
                _selectedFilterDomain = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedFilterDomain"));
                RaiseCanExecuteChanged();
                UpdateFilteredTargetEntities();
                if (_selectedFilterDomain != null
                    && _selectedFilterDomain.DomainName != oldValue?.DomainName
                    && !IsFilterByEntityEnabled)
                {
                    IsFilterByEntityEnabled = true;
                }
            }
        }



        private bool _generateGlobalEnums = false;
        public bool GenerateGlobalEnums
        {
            get
            {
                return _generateGlobalEnums;
            }
            set
            {
                _generateGlobalEnums = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("GenerateGlobalEnums"));
            }
        }


        private CommonTarget _commonTargetData = null;
        public CommonTarget CommonTargetData
        {
            get
            {
                return _commonTargetData;
            }
            set
            {
                _commonTargetData = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CommonTargetData"));
                RaiseCanExecuteChanged();
                if (value != null)
                {
                    var mappings = new List<MappingEditable>();
                    foreach (var item in CommonTargetData.Mapping)
                    {
                        mappings.Add(new MappingEditable(item, false));
                    }
                    CommonMappingsEditables = mappings;

                    var enums = new List<MappingEditable>();
                    foreach (var item in CommonTargetData.GlobalEnums)
                    {
                        enums.Add(new MappingEditable(item, false));
                    }
                    CommonEnumsEditables = enums;
                }

            }
        }


        private EntityTarget _selectedEntity = null;
        public EntityTarget SelectedEntity
        {
            get
            {
                return _selectedEntity;
            }
            set
            {
                _selectedEntity = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedEntity"));
                RaiseCanExecuteChanged();
                if (value == null)
                {
                    MappingsEditables = null;
                    IsEntityDetailShowing = false;
                    
                }
                else
                {
                    var mappings = new List<MappingEditable>();

                    foreach (var item in CommonTargetData.Mapping)
                    {
                        mappings.Add(new MappingEditable(item, false));
                    }

                    foreach (var item in value.Mapping)
                    {
                        mappings.Add(new MappingEditable(item, true));
                    }
                    MappingsEditables = mappings;


                    var enums = new List<MappingEditable>();
                    foreach (var item in value.Enums)
                    {
                        enums.Add(new MappingEditable(item, false));
                    }
                    EnumsEditables = enums;

                    SelectedDomain = Domains.FirstOrDefault(k => k.DomainName == value.PluralNamespaceName);

                    IsEntityDetailShowing = true;
                    IsAddingNewEntity = false;
                    IsCommonDetailShowing = false;

                    
                }

            }
        }



        private List<MappingEditable> _commonMappingsEditables = null;
        public List<MappingEditable> CommonMappingsEditables
        {
            get
            {
                return _commonMappingsEditables;
            }
            set
            {
                _commonMappingsEditables = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CommonMappingsEditables"));
                if (value != null)
                {
                    var fileterd = GetFilteredEditablesMappings(value, FilterCommonMappingInput);
                    UpdateCollection(fileterd, CommonMappingsEditablesCollection);
                }
                else
                {
                    UpdateCollection(value, CommonMappingsEditablesCollection);
                }
            }
        }


        private readonly ObservableCollection<MappingEditable> _commonMappingsEditablesCollection = new ObservableCollection<MappingEditable>();
        public ObservableCollection<MappingEditable> CommonMappingsEditablesCollection
        {
            get
            {
                return _commonMappingsEditablesCollection;
            }
        }

        private List<MappingEditable> _commonEnumsEditables = null;
        public List<MappingEditable> CommonEnumsEditables
        {
            get
            {
                return _commonEnumsEditables;
            }
            set
            {
                _commonEnumsEditables = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CommonEnumsEditables"));
                if (value != null)
                {
                    var fileterd = GetFilteredEditablesMappings(value, FilterCommonEnumInput);
                    UpdateCollection(fileterd, CommonEnumsEditablesCollection);
                }
                else
                {
                    UpdateCollection(value, CommonEnumsEditablesCollection);
                }
            }
        }

        private readonly ObservableCollection<MappingEditable> _commonEnumsEditablesCollection = new ObservableCollection<MappingEditable>();
        public ObservableCollection<MappingEditable> CommonEnumsEditablesCollection
        {
            get
            {
                return _commonEnumsEditablesCollection;
            }
        }



        private List<MappingEditable> _enumsEditables = null;
        public List<MappingEditable> EnumsEditables
        {
            get
            {
                return _enumsEditables;
            }
            set
            {
                _enumsEditables = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("EnumsEditables"));
                if (value != null)
                {
                    var fileterd = GetFilteredEditablesMappings(value, FilterEnumInput);
                    UpdateCollection(fileterd, EnumsEditablesCollection);
                }
                else
                {
                    UpdateCollection(value, EnumsEditablesCollection);
                }
            }
        }

        private readonly ObservableCollection<MappingEditable> _enumsEditablesCollection = new ObservableCollection<MappingEditable>();
        public ObservableCollection<MappingEditable> EnumsEditablesCollection
        {
            get
            {
                return _enumsEditablesCollection;
            }
        }




        private List<MappingEditable> _mappingsEditables = null;
        public List<MappingEditable> MappingsEditables
        {
            get
            {
                return _mappingsEditables;
            }
            set
            {
                _mappingsEditables = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("MappingsEditables"));
                if (value != null)
                {
                    var fileterd = GetFilteredEditablesMappings(value, FilterMappingInput);
                    UpdateCollection(fileterd, MappingsEditablesCollection);
                }
                else
                {
                    UpdateCollection(value, MappingsEditablesCollection);
                }
            }
        }



        private string _filterCommonMappingInput = null;
        public string FilterCommonMappingInput
        {
            get
            {
                return _filterCommonMappingInput;
            }
            set
            {
                _filterCommonMappingInput = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FilterCommonMappingInput"));
                var fileterd = GetFilteredEditablesMappings(CommonMappingsEditables, value);
                UpdateCollection(fileterd, CommonMappingsEditablesCollection);
            }
        }


        private string _filterCommonEnumInput = null;
        public string FilterCommonEnumInput
        {
            get
            {
                return _filterCommonEnumInput;
            }
            set
            {
                _filterCommonEnumInput = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FilterCommonEnumInput"));
                var fileterd = GetFilteredEditablesMappings(CommonEnumsEditables, value);
                UpdateCollection(fileterd, CommonEnumsEditablesCollection);
            }
        }


        private string _filterEnumInput = null;
        public string FilterEnumInput
        {
            get
            {
                return _filterEnumInput;
            }
            set
            {
                _filterEnumInput = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FilterEnumInput"));
                var fileterd = GetFilteredEditablesMappings(EnumsEditables, value);
                UpdateCollection(fileterd, EnumsEditablesCollection);
            }
        }

        private string _filterMappingInput = null;
        public string FilterMappingInput
        {
            get
            {
                return _filterMappingInput;
            }
            set
            {
                _filterMappingInput = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FilterMappingInput"));
                var fileterd = GetFilteredEditablesMappings(MappingsEditables, value);
                UpdateCollection(fileterd, MappingsEditablesCollection);
            }
        }


        private readonly ObservableCollection<MappingEditable> _mappingsEditablesCollection = new ObservableCollection<MappingEditable>();
        public ObservableCollection<MappingEditable> MappingsEditablesCollection
        {
            get
            {
                return _mappingsEditablesCollection;
            }
        }



        private List<EntityTarget> _targetEntities = new List<EntityTarget>();
        public List<EntityTarget> TargetEntities
        {
            get
            {
                return _targetEntities;
            }
            set
            {
                _targetEntities = value;
                UpdateFilteredTargetEntities(value);
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("TargetEntities"));
                RaiseCanExecuteChanged();
            }
        }

        private void UpdateFilteredTargetEntities(List<EntityTarget> entities)
        {
            var entitiesForUpdate = entities;
            if (IsFilterByEntityEnabled && SelectedFilterDomain != null)
            {
                entitiesForUpdate = entities.Where(k => k.PluralNamespaceName == SelectedFilterDomain.DomainName).ToList();
            }
            UpdateCollection(entitiesForUpdate, TargetEntitiesCollection);
        }

        private void UpdateFilteredTargetEntities()
        {
            UpdateFilteredTargetEntities(TargetEntities);
        }

        private readonly ObservableCollection<EntityTarget> _targetEntitiesCollection = new ObservableCollection<EntityTarget>();
        public ObservableCollection<EntityTarget> TargetEntitiesCollection
        {
            get
            {
                return _targetEntitiesCollection;
            }
        }

        private readonly ObservableCollection<EntityTarget> _filteredTargetEntitiesCollection = new ObservableCollection<EntityTarget>();
        public ObservableCollection<EntityTarget> FilteredTargetEntitiesCollection
        {
            get
            {
                return _filteredTargetEntitiesCollection;
            }
        }

        private bool _isAddingNewEntity = false;
        public bool IsAddingNewEntity
        {
            get
            {
                return _isAddingNewEntity;
            }
            set
            {
                _isAddingNewEntity = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsAddingNewEntity"));
                RaiseCanExecuteChanged();
            }
        }


        private bool _isCommonDetailShowing = false;
        public bool IsCommonDetailShowing
        {
            get
            {
                return _isCommonDetailShowing;
            }
            set
            {
                _isCommonDetailShowing = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsCommonDetailShowing"));
                RaiseCanExecuteChanged();
            }
        }



        private bool _isEntityDetailShowing = false;
        public bool IsEntityDetailShowing
        {
            get
            {
                return _isEntityDetailShowing;
            }
            set
            {
                _isEntityDetailShowing = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsEntityDetailShowing"));
                RaiseCanExecuteChanged();
            }
        }


        private string _newEntityDisplayName = null;
        public string NewEntityDisplayName
        {
            get
            {
                return _newEntityDisplayName;
            }
            set
            {
                _newEntityDisplayName = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewEntityDisplayName"));
                RaiseCanExecuteChanged();
            }
        }

        private string _newEntityName = null;
        public string NewEntityName
        {
            get
            {
                return _newEntityName;
            }
            set
            {
                _newEntityName = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewEntityName"));
                RaiseCanExecuteChanged();
            }
        }



        private string _trimPrefixesInput = null;
        public string TrimPrefixesInput
        {
            get
            {
                return _trimPrefixesInput;
            }
            set
            {
                _trimPrefixesInput = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("TrimPrefixesInput"));
            }
        }


        private Window _window;

        public MainViewmodel()
        {

        }


        public void Initialize(Window window)
        {
            this._window = window;
            ReloadEntities();
            RegisterCommands();
        }



        private List<MappingEditable> GetFilteredEditablesMappings(List<MappingEditable> baseData, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return baseData;
            }
            return baseData
                    .Where(k =>
                        k.Key.ToLower().IndexOf(filter) > -1
                        || k.Key.ToLower().IndexOf(filter) > -1)
                    .ToList();
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

        private void ReloadEntities()
        {
            var entities = FileManager.GetEntitiesDefinitionsTargetEntities();
            foreach (var item in entities)
            {
                item.IsSelected = false;
            }
            this.TargetEntities = entities;
            this.CommonTargetData = FileManager.GetEntitiesDefinitionsCommonTarget();
            this.Domains = FileManager.GetDomains();
        }

        protected override void RegisterCommands()
        {
            Commands.Add("AddEntityRequestCommand", AddEntityRequestCommand);
            Commands.Add("AddEntityCommand", AddEntityCommand);
            Commands.Add("CancelAddEntityCommand", CancelAddEntityCommand);
            Commands.Add("DeleteEntityCommand", DeleteEntityCommand);
            Commands.Add("AddMappingCommand", AddMappingCommand);

            Commands.Add("CancelEditTargetEntityCommand", CancelEditTargetEntityCommand);
            Commands.Add("ConfirmEditTargetEntityCommand", ConfirmEditTargetEntityCommand);
            Commands.Add("EditCommonDataRequestCommand", EditCommonDataRequestCommand);
            Commands.Add("AddCommonMappingCommand", AddCommonMappingCommand);
            Commands.Add("CancelEditCommonTargetDataCommand", CancelEditCommonTargetDataCommand);
            Commands.Add("ConfirmEditCommonDataTargetCommand", ConfirmEditCommonDataTargetCommand);
            Commands.Add("AddEnumCommand", AddEnumCommand);
            Commands.Add("SelectUnselectAllCommand", SelectUnselectAllCommand);
            Commands.Add("StartGeneratingCommand", StartGeneratingCommand);

            Commands.Add("ShowDomainManagerCommand", ShowDomainManagerCommand);

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
                            DomainManager domainManager = new DomainManager();
                            domainManager.ShowDialog();
                            this.Domains = FileManager.GetDomains();
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

        private ICommand _startGeneratingCommand = null;
        public ICommand StartGeneratingCommand
        {
            get
            {
                if (_startGeneratingCommand == null)
                {
                    _startGeneratingCommand = new RelayCommand((object param) =>
                    {
                        try
                        {

                            SetDialog("Connection to CRM...");

                            new Thread(() =>
                            {
                                var targetEntities = TargetEntities.Where(k => k.IsSelected).Select(k => { return k.LogicalName; });
                                var stringConnection = GetStringConnectionFromKeyVault();
                                if (string.IsNullOrEmpty(stringConnection))
                                {
                                    UnsetDialog();
                                    RaiseError("Cannot access to the Azure Key Vault. Have you executed 'az login' in your computer?");
                                }
                                else
                                {
                                    EntityMappingSettings commonSettings = FileManager.GetCommonMappingSettings();

                                    ModelManager modelManager = new ModelManager(stringConnection, commonSettings);
                                    Console.WriteLine("Generating global enums...");
                                    if (GenerateGlobalEnums)
                                    {
                                        UpdateDialogMessage("Generating entities definitions...");
                                        modelManager.GenerateEntitiesDefinitions();
                                        UpdateDialogMessage("Generating roles definitions...");
                                        modelManager.GenerateRolesDefinitions();
                                        UpdateDialogMessage("Generating global enums...");
                                        modelManager.GenerateGlobalEnums();
                                    }

                                    foreach (var entity in targetEntities)
                                    {
                                        UpdateDialogMessage($"Generating entity '{entity}'...");
                                        Console.WriteLine($"Generating model for entity {entity}...");
                                        var jsonSettings = FileManager.GetEntitiesDefinitionsEntityMappingSettings(entity);
                                        EntityMappingSettings settings = MergeCommonMappings(commonSettings, jsonSettings);
                                        modelManager.GenerateEntityModel(entity, settings);
                                    }
                                    Console.WriteLine($"Completed generation");
                                    UnsetDialog();
                                }

                            }).Start();

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
                return _startGeneratingCommand;
            }

        }



        private void SetDialog(string message)
        {
            IsDialogOpen = true;
            this.MessageDialog = message;
        }

        private void UpdateDialogMessage(string message)
        {
            this.MessageDialog = message;

        }

        private void UnsetDialog()
        {
            IsDialogOpen = false;
        }


        private static EntityMappingSettings MergeCommonMappings(EntityMappingSettings commonSettings, EntityMappingSettings settings)
        {
            //EntityMappingSettings settings = GetSettings(jsonSettings);

            foreach (var item in commonSettings.Mapping)
            {
                if (!settings.Mapping.ContainsKey(item.Key))
                {
                    settings.Mapping.Add(item.Key, commonSettings.Mapping[item.Key]);
                }
            }

            return settings;
        }

        private string GetStringConnectionFromKeyVault()
        {
            string stringConnection = string.Empty;
            var keyValueName = SettingsManager.GetAppConfig("keyValueName");
            var keyValueSecret = SettingsManager.GetAppConfig("keyValueSecretName");
            try
            {
                stringConnection = KeyVaultService.GetValueSecretFromKeyVault(keyValueName, keyValueSecret);
            }
            catch (Exception)
            {
                //DO nothing
            }
            return stringConnection;
        }


        private ICommand _editCommonDataRequestCommand = null;
        public ICommand EditCommonDataRequestCommand
        {
            get
            {
                if (_editCommonDataRequestCommand == null)
                {
                    _editCommonDataRequestCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            SelectedEntity = null;
                            IsAddingNewEntity = false;

                            TrimPrefixesInput = string.Join(",", CommonTargetData.TrimPrefixes);
                            IsCommonDetailShowing = true;

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
                return _editCommonDataRequestCommand;
            }

        }

        private void CheckDuplicates(List<MappingEditable> compareWith, bool isField)
        {
            string type = isField ? "Campo" : "Enum";
            foreach (var item1 in compareWith)
            {
                var duplicatesKeys = compareWith.Where(k => k.Key == item1.Key);
                if (duplicatesKeys.ToList().Count >= 2)
                {
                    throw new Exception($"{type} de CRM duplicado: {item1.Key}");
                }


                var duplicatesValues = compareWith.Where(k => k.Value == item1.Value);
                if (duplicatesValues.ToList().Count >= 2)
                {
                    throw new Exception($"Propiedad de {type} en C# duplicada: {item1.Value}");
                }
            }
        }

        private void CheckDuplicatesWithCommon(List<MappingEditable> compareWith)
        {
            foreach (var item1 in CommonTargetData.Mapping)
            {
                foreach (var item2 in compareWith)
                {
                    if (item1.Key == item2.Key)
                    {
                        throw new Exception($"Campo de CRM duplicado: {item1.Key}");
                    }

                    if (item1.Value == item2.Value)
                    {
                        throw new Exception($"Propiedad de C# duplicada: {item1.Value}");
                    }
                }
            }
        }





        private ICommand _confirmEditTargetEntityCommand = null;
        public ICommand ConfirmEditTargetEntityCommand
        {
            get
            {
                if (_confirmEditTargetEntityCommand == null)
                {
                    _confirmEditTargetEntityCommand = new RelayCommand((object param) =>
                    {
                        try
                        {

                            var fieldsForUpdate = MappingsEditablesCollection.Where(k => k.IsFromEntity).ToList();
                            var enumsForUpdate = EnumsEditablesCollection.ToList();
                            //CheckDuplicatesWithCommon(dataForUpdate); 
                            CheckDuplicates(fieldsForUpdate, true);
                            SelectedEntity.Mapping = new Dictionary<string, string>();
                            foreach (var item in fieldsForUpdate)
                            {
                                SelectedEntity.Mapping.Add(item.Key, item.Value);
                            }
                            SelectedEntity.Enums = new Dictionary<string, string>();
                            foreach (var item in enumsForUpdate)
                            {
                                SelectedEntity.Enums.Add(item.Key, item.Value);
                            }
                            SelectedEntity.PluralNamespaceName = SelectedDomain.DomainName;
                            FileManager.UpdateEntitiesDefinitionsTargetEntity(SelectedEntity);
                            ReloadEntities();
                            SelectedEntity = null;
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
                return _confirmEditTargetEntityCommand;
            }

        }


        private ICommand _confirmEditCommonDataTargetCommand = null;
        public ICommand ConfirmEditCommonDataTargetCommand
        {
            get
            {
                if (_confirmEditCommonDataTargetCommand == null)
                {
                    _confirmEditCommonDataTargetCommand = new RelayCommand((object param) =>
                    {
                        try
                        {

                            var fieldsForUpdate = CommonMappingsEditablesCollection.Where(k => !k.IsFromEntity).ToList();
                            var enumsForUpdate = CommonEnumsEditablesCollection.ToList();
                            CheckDuplicates(fieldsForUpdate, true);
                            CheckDuplicates(enumsForUpdate, false);

                            CommonTargetData.Mapping = new Dictionary<string, string>();
                            foreach (var item in fieldsForUpdate)
                            {
                                CommonTargetData.Mapping.Add(item.Key, item.Value);
                            }

                            CommonTargetData.GlobalEnums = new Dictionary<string, string>();
                            foreach (var item in enumsForUpdate)
                            {
                                CommonTargetData.GlobalEnums.Add(item.Key, item.Value);
                            }


                            FileManager.UpdateEntityDefinitionsCommonDataTarget(CommonTargetData);
                            ReloadEntities();
                            IsCommonDetailShowing = false;
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
                return _confirmEditCommonDataTargetCommand;
            }

        }


        private ICommand _cancelEditCommonTargetDataCommand = null;
        public ICommand CancelEditCommonTargetDataCommand
        {
            get
            {
                if (_cancelEditCommonTargetDataCommand == null)
                {
                    _cancelEditCommonTargetDataCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsCommonDetailShowing = false;
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
                return _cancelEditCommonTargetDataCommand;
            }
        }

        private ICommand _cancelEditTargetEntityCommand = null;
        public ICommand CancelEditTargetEntityCommand
        {
            get
            {
                if (_cancelEditTargetEntityCommand == null)
                {
                    _cancelEditTargetEntityCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            SelectedEntity = null;
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
                return _cancelEditTargetEntityCommand;
            }
        }


        private ICommand _addEnumCommand = null;
        public ICommand AddEnumCommand
        {
            get
            {
                if (_addEnumCommand == null)
                {
                    _addEnumCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var mappingsEditable = EnumsEditables;
                            mappingsEditable.Add(new MappingEditable("logical_name", "LogicalName", false));
                            EnumsEditables = mappingsEditable;
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
                return _addEnumCommand;
            }
        }

        private ICommand _addCommonEnumCommand = null;
        public ICommand AddCommonEnumCommand
        {
            get
            {
                if (_addCommonEnumCommand == null)
                {
                    _addCommonEnumCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var mappingsEditable = CommonEnumsEditables;
                            mappingsEditable.Add(new MappingEditable("logical_name", "LogicalName", false));
                            CommonEnumsEditables = mappingsEditable;
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
                return _addCommonEnumCommand;
            }
        }


        private ICommand _addCommonMappingCommand = null;
        public ICommand AddCommonMappingCommand
        {
            get
            {
                if (_addCommonMappingCommand == null)
                {
                    _addCommonMappingCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var mappingsEditable = CommonMappingsEditables;
                            mappingsEditable.Add(new MappingEditable("logical_name", "LogicalName", false));
                            CommonMappingsEditables = mappingsEditable;
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
                return _addCommonMappingCommand;
            }
        }


        private ICommand _addMappingCommand = null;
        public ICommand AddMappingCommand
        {
            get
            {
                if (_addMappingCommand == null)
                {
                    _addMappingCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var mappingsEditable = MappingsEditables;
                            mappingsEditable.Add(new MappingEditable("logical_name", "LogicalName", true));
                            MappingsEditables = mappingsEditable;
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
                return _addMappingCommand;
            }
        }


        private ICommand _selectUnselectAllCommand = null;
        public ICommand SelectUnselectAllCommand
        {
            get
            {
                if (_selectUnselectAllCommand == null)
                {
                    _selectUnselectAllCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            if (TargetEntitiesCollection.FirstOrDefault(k => !k.IsSelected) != null)
                            {
                                var entities = TargetEntitiesCollection;
                                foreach (var item in entities)
                                {
                                    item.IsSelected = true;
                                }
                                //TargetEntities = entities;
                            }
                            else
                            {
                                var entities = TargetEntitiesCollection;
                                foreach (var item in entities)
                                {
                                    item.IsSelected = false;
                                }
                                //TargetEntities = entities;
                            }

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
                return _selectUnselectAllCommand;
            }
        }


        private ICommand _deleteEntityCommand = null;
        public ICommand DeleteEntityCommand
        {
            get
            {
                if (_deleteEntityCommand == null)
                {
                    _deleteEntityCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var responseMsgbox = MessageBox.Show($"Confirmas que quieres eliminar la entidad {SelectedEntity.LogicalName}?", "Eliminar entidad", MessageBoxButton.OKCancel);
                            if (responseMsgbox == MessageBoxResult.OK)
                            {
                                FileManager.DeleteEntitiesDefinitionsTargetEntity(SelectedEntity);
                                SelectedEntity = null;
                                ReloadEntities();
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SelectedEntity != null;
                    });
                }
                return _deleteEntityCommand;
            }
        }


        private ICommand _cancelAddEntityCommand = null;
        public ICommand CancelAddEntityCommand
        {
            get
            {
                if (_cancelAddEntityCommand == null)
                {
                    _cancelAddEntityCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsAddingNewEntity = false;
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
                return _cancelAddEntityCommand;
            }
        }

        private ICommand _addEntityCommand = null;
        public ICommand AddEntityCommand
        {
            get
            {
                if (_addEntityCommand == null)
                {
                    _addEntityCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var repeated = TargetEntities.FirstOrDefault(k => k.LogicalName == NewEntityName);
                            if (repeated != null)
                            {
                                throw new Exception("La entidad seleccionada ya está incluida");
                            }


                            EntityTarget entity = new EntityTarget();
                            entity.LogicalName = NewEntityName;
                            entity.EntityDomainName = NewEntityDisplayName;
                            entity.PluralNamespaceName = SelectedDomain.DomainName;
                            entity.OutputFile = entity.DefaultOutputFile;
                            FileManager.AddNewTargetEntity(entity);
                            ReloadEntities();
                            IsAddingNewEntity = false;

                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty(NewEntityName)
                            && SelectedDomain != null
                            && !string.IsNullOrEmpty(NewEntityDisplayName);
                    });
                }
                return _addEntityCommand;
            }
        }


        private ICommand _addEntityRequestCommand = null;
        public ICommand AddEntityRequestCommand
        {
            get
            {
                if (_addEntityRequestCommand == null)
                {
                    _addEntityRequestCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            NewEntityName = null;
                            SelectedEntity = null;
                            IsCommonDetailShowing = false;
                            IsAddingNewEntity = true;
                            NewEntityDisplayName = "EntityLogicalName";
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
                return _addEntityRequestCommand;
            }
        }


    }
}
