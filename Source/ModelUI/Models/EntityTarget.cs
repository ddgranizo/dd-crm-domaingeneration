using ModelUI.Viewmodels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUI.Models
{
    public class EntityTarget : BaseViewmodel
    {

        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsSelected"));
            }
        }
        public Dictionary<string, string> Mapping { get; set; }
        public Dictionary<string, string> Enums { get; set; }





        private string _outputFile = null;
        public string OutputFile
        {
            get
            {
                return _outputFile;
            }
            set
            {
                _outputFile = value;
                OnPropertyChanged("OutputFile");
            }
        }



        public string DefaultOutputFile
        {
            get
            {
                return $"../../../../Main/Source/Scm.Focus.Domain.{PluralNamespaceName}/Models/{EntityDomainName}.cs";
            }
        }


        private string _pluralNamespaceName = null;
        public string PluralNamespaceName
        {
            get { return _pluralNamespaceName; }
            set
            {
                _pluralNamespaceName = value;
                OnPropertyChanged("PluralNamespaceName");
                OnPropertyChanged("OutputFile");
            }
        }

        private string _entityDomainName = null;
        public string EntityDomainName
        {
            get { return _entityDomainName; }
            set
            {
                _entityDomainName = value;
                OnPropertyChanged("EntityDomainName");
                OnPropertyChanged("OutputFile");
            }
        }
        public string LogicalName { get; set; }
        public EntityTarget()
        {
            Mapping = new Dictionary<string, string>();
            Enums = new Dictionary<string, string>();
        }


        public void UpdatePropertyMapping()
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Mapping"));
        }

        public override string ToString()
        {
            return this.LogicalName;
        }
    }
}
