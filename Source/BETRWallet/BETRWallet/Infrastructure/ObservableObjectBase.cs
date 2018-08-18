
using BETRWallet.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
 

namespace BETRWallet.Infrastructure
{
    public abstract class ObservableObjectBase : BindableBase
    {
        protected IRepositoryService Repository { get; set; }

        readonly Dictionary<string, List<string>> _propertyDependencies = new Dictionary<string, List<string>>();

        string _id = Guid.NewGuid().ToString();
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Layer image Url ...
        /// </summary>
		string _layerImageUrl = string.Empty;
        public string LayerImageUrl
        {
			get => _layerImageUrl;
			set => SetProperty(ref _layerImageUrl, value);
        }

        string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableObjectBase(IRepositoryService repository)
        {
            _propertyDependencies = new Dictionary<string, List<string>>();
            Repository = repository;
        }

        public ObservableObjectBase()
        {
            _propertyDependencies = new Dictionary<string, List<string>>();
        }

        protected virtual void AddDependentProperties(string propertyName, List<string> dependentProperties)
        {
            _propertyDependencies.Add(propertyName, dependentProperties);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (_propertyDependencies.TryGetValue(args.PropertyName, out List<string> dependentProperties))
                foreach (var dependentProperty in dependentProperties)
                    OnPropertyChanged(new PropertyChangedEventArgs(dependentProperty));
        }
    }
}