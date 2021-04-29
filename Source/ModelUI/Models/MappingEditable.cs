
using ModelUI.Viewmodels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUI.Models
{
    public class MappingEditable : BaseViewmodel
    {


        public string Source
        {
            get
            {
                if (IsFromEntity)
                {
                    return "Fichero de entidad";
                }
                return "Fichero common.json";
            }
        }


        private bool _isFromEntity = false;
        public bool IsFromEntity
        {
            get
            {
                return _isFromEntity;
            }
            set
            {
                _isFromEntity = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsFromEntity"));
            }
        }



        private string _value = null;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Value"));
            }
        }

        private string _key = null;
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Key"));
            }
        }



        public MappingEditable(string key, string value, bool isFromEntity)
        {
            this.Key = key;
            this.Value = value;
            this.IsFromEntity = isFromEntity;
        }

        public MappingEditable(KeyValuePair<string, string> keyValuePair, bool isFromEntity)
        {
            this.Key = keyValuePair.Key;
            this.Value = keyValuePair.Value;
            this.IsFromEntity = isFromEntity;
        }

        public KeyValuePair<string, string> ToKeyValuePair()
        {
            return new KeyValuePair<string, string>(this.Key, this.Value);
        }
    }
}
