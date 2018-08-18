using System;
using System.Linq;
using System.Threading.Tasks;
using BETRWallet.Constants;
using BETRWallet.Infrastructure;
using BETRWallet.Interfaces;
using BETRWallet.Views;
using DryIoc;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Essentials;

namespace BETRWallet.ViewModels 
{
    public abstract class ViewModelBase : ObservableObjectBase, INavigationAware, IConfirmNavigation, IDestructible
    {
        #region constants

        const string ButtonTextOK = "OK";
        const string CaptionError = "Error";
        const string ParameterKey = "Key";
        const string RootUriPrependText = "/";

        #endregion

        #region properties
        protected IContainer Container { get; }
        protected IDeviceService DeviceService { get; }
        protected INavigationService NavigationService { get; }
        protected IPageDialogService PageDialogService { get; }
        protected IRepositoryService RepositoryService { get; }
   
        public DelegateCommand GoBackCommand { get; set; }



        // Show Bet History
		public DelegateCommand ShowBetHistoryCommand { get; set; }
		// Shows the wallet page
        public DelegateCommand ShowWalletCommand { get; set; }
        // Settings
		public DelegateCommand TapProfileCommand { get; set; }

		public DelegateCommand ShowDebugCommand { get; set; }      
        public DelegateCommand IncrementDebugCommand { get; set; }

        public DelegateCommand ToggleMenuCommand { get; set; }

        

        DelegateCommand<string> _navigateAbsoluteCommand;
        public DelegateCommand<string> NavigateAbsoluteCommand => _navigateAbsoluteCommand ?? (_navigateAbsoluteCommand = new DelegateCommand<string>(async param => await OnNavigateAbsoluteCommand(param), CanNavigateAbsoluteCommand));

        DelegateCommand<string> _navigateCommand;
        public DelegateCommand<string> NavigateCommand => _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(async param => await OnNavigateCommand(param), CanNavigateCommand));

        DelegateCommand<string> _navigateModalCommand;
        public DelegateCommand<string> NavigateModalCommand => _navigateModalCommand ?? (_navigateModalCommand = new DelegateCommand<string>(async param => await OnNavigateModalCommand(param), CanNavigateModalCommand));

        DelegateCommand<string> _navigateNonModalCommand;
        public DelegateCommand<string> NavigateNonModalCommand => _navigateNonModalCommand ?? (_navigateNonModalCommand = new DelegateCommand<string>(async param => await OnNavigateNonModalCommand(param), CanNavigateNonModalCommand));

        private bool _isPlacingBet;
        public bool IsPlacingBet
        {
            get { return _isPlacingBet; }
            set
            {

                _isPlacingBet = value;
                RaisePropertyChanged();
              
            }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
                OnIsBusyChanged();
            }
        }
        



		bool _isRefreshing;
		public bool IsRefreshing
        {
			get { return _isRefreshing; }
            set
            {
				_isRefreshing = value;
                RaisePropertyChanged();
                OnIsBusyChanged();
            }
        }



        /// <summary>
        /// Allows the menu to be toggled on/off
        /// </summary>
        private bool _showMenu;
        public bool ShowMenu
        {
            get { return _showMenu; }
            set
            {

                _showMenu= value;
                RaisePropertyChanged();

            }
        }

        private bool _showShare = false;
        public bool ShowShare
        {
            get { return _showShare; }
            set
            {

                _showShare= value;
                RaisePropertyChanged();

            }
        }




		private bool _allowSettingsChange;
		public bool AllowSettingsChange
        {
			get { return _allowSettingsChange; }
            set
            {

				_allowSettingsChange= value;
                RaisePropertyChanged();

            }
        }

        

        bool _isNavigating;
        public bool IsNavigating
        {
			get { return _isNavigating; }
            set
            {
				_isNavigating = value;
                RaisePropertyChanged();
                OnIsBusyChanged();
            }
        }



        bool _showAddWallet = false; // default to true
        public bool ShowAddWallet
        {
            get { return _showAddWallet; }
            set
            {
                _showAddWallet = value;
                RaisePropertyChanged();

            }
        }


        bool _showBackNavigation = false; // default to true
        public bool ShowBackNavigation
        {
            get { return _showBackNavigation; }
            set
            {
                _showBackNavigation = value;
                RaisePropertyChanged();
               
            }
        }

        bool _showSettingsNavigation = true;
        public bool ShowSettingsNavigation
        {
            get { return _showSettingsNavigation; }
            set
            {
                _showSettingsNavigation = value;
                RaisePropertyChanged();

            }
        }


        /// <summary>
        /// Used in ContextTitleView
        /// </summary>
		string _topContextTitle;
		public string TopContextTitle
        {
			get { return _topContextTitle; }
            set
            {
				_topContextTitle = value;
                RaisePropertyChanged();

            }
        }


        /// <summary>
        /// Used in ContextTitleView
        /// </summary>
		string _bottomContextTitle;
        public string BottomContextTitle
        {
			get { return _bottomContextTitle; }
            set
            {
				_bottomContextTitle = value;
                RaisePropertyChanged();

            }
        }

        string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();

            }
        }

        #endregion

        #region navigation_command_handlers

        protected virtual bool CanNavigateAbsoluteCommand(string uri)
        {
            return !string.IsNullOrEmpty(uri);
        }

        protected virtual async Task OnNavigateAbsoluteCommand(string uri)
        {
            try
            {
                if (CanNavigateAbsoluteCommand(uri))
                {
                    SetIsBusy();
                    await NavigateToNewRootUri(uri);
                }
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
            finally
            {
                ClearIsBusy();
            }
        }

        // used to clean up and messages etc ,,
		public virtual void CleanUp()
		{
			
		}

        protected virtual bool CanNavigateCommand(string uri)
        {
            return !string.IsNullOrEmpty(uri);
        }

        protected virtual async Task OnNavigateCommand(string uri)
        {
            try
            {
                if (CanNavigateCommand(uri))
                {
                    SetIsBusy();
                    await NavigateToUri(uri);
                }
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
            finally
            {
                ClearIsBusy();
            }
        }

        protected virtual bool CanNavigateModalCommand(string uri)
        {
            return !string.IsNullOrEmpty(uri);
        }

        protected virtual async Task OnNavigateModalCommand(string uri)
        {
            try
            {
                if (CanNavigateCommand(uri))
                {
                    SetIsBusy();
                    await NavigateModalToUri(uri);
                }
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
            finally
            {
                ClearIsBusy();
            }
        }

        protected virtual bool CanNavigateNonModalCommand(string uri)
        {
            return !string.IsNullOrEmpty(uri);
        }

        protected virtual async Task OnNavigateNonModalCommand(string uri)
        {
            try
            {
                if (CanNavigateCommand(uri))
                {
                    SetIsBusy();
                    await NavigationService.NavigateAsync(uri, null, false);
                }
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
            finally
            {
                ClearIsBusy();
            }
        }

        #endregion

        #region ctor

        public ViewModelBase(IRepositoryService repository, IDeviceService deviceService, INavigationService navigationService, IPageDialogService pageDialogService, IContainer container)
            : base(repository)
        {
            RepositoryService = repository ?? throw new ArgumentNullException(nameof(repository));
            DeviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            PageDialogService = pageDialogService ?? throw new ArgumentNullException(nameof(pageDialogService));
            Container = container ?? throw new ArgumentNullException(nameof(container));

            GoBackCommand = new DelegateCommand(OnTapBack);

                  ToggleMenuCommand = new DelegateCommand(OnToggleMenu);
       
        }

        #endregion

        #region INavigationAware_implementation

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        #endregion

        #region command handlers

        /// <summary>
        /// Handle the menu toggle  from the viewmodel implementation
        /// </summary>
        internal  virtual async  void OnToggleMenu()
        {   }



		
        async void OnTapBack()
        {
            await GoBack();
        }

        #endregion

        #region IConfirmNavigation_implementation

        public virtual bool CanNavigate(NavigationParameters parameters)
        {
            return true;
        }

        #endregion

        #region IDestructible_implementation

        public virtual void Destroy()
        {
        }

        #endregion


        #region logging

		internal void LogException(string message, Exception ex)
		{
			
		}

		internal void LogException(Exception ex)
        {

        }

		internal void LogException(string message)
        {

        }

        #endregion


        #region helper_methods

       
        /// <summary>
        /// Allows us to navigate all the way back to the start
        /// </summary>
        /// <returns>The to start.</returns>
		protected async Task NavigateToStart()
		{
            try
            {
				NavigationParameters navParams = new NavigationParameters();
				navParams.Add(NavigationParameterNames.BackToStart, "");

				await NavigationService.NavigateAsync($"app:////{nameof(SplashPage)}",navParams );
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }



        protected async Task DisplayDialog(string title, string message, string buttonText = ButtonTextOK)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or white space.", nameof(title));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Value cannot be null or white space.", nameof(message));

            if (string.IsNullOrWhiteSpace(buttonText))
                throw new ArgumentException("Value cannot be null or white space.", nameof(buttonText));

            try
            {
                await PageDialogService.DisplayAlertAsync(title, message, buttonText);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        protected async Task<bool> DisplayDialog(string title, string message, string acceptButtonText, string cancellationButtonText)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or white space.", nameof(title));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Value cannot be null or white space.", nameof(message));

            if (string.IsNullOrWhiteSpace(acceptButtonText))
                throw new ArgumentException("Value cannot be null or white space.", nameof(acceptButtonText));

            if (string.IsNullOrWhiteSpace(cancellationButtonText))
                throw new ArgumentException("Value cannot be null or white space.", nameof(cancellationButtonText));

            try
            {
                return await PageDialogService.DisplayAlertAsync(title, message, acceptButtonText, cancellationButtonText);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
                return false;
            }
        }

        protected async Task NavigateToNewRootUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(uri));
            }
            await NavigateToNewRootUriInternal(uri);
        }

        async Task NavigateToNewRootUriInternal(string uri, NavigationParameters navigationParameters = null)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(uri));
            }
            try
            {
                if (!uri.StartsWith(RootUriPrependText))
                {
                    uri = string.Concat(RootUriPrependText, uri);
                }
                await NavigationService.NavigateAsync(uri, navigationParameters, false);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        protected async Task NavigateToUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("Value cannot be null or white space.", nameof(uri));

            try
            {
                await NavigationService.NavigateAsync(uri, useModalNavigation: false);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        protected async Task NavigateModalToUri(string uri, object parameter)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("Value cannot be null or white space.", nameof(uri));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            try
            {
                var navigationParameters = new NavigationParameters();
                navigationParameters.Add(ParameterKey, parameter);
                await NavigateModalToUri(uri, navigationParameters);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        protected async Task NavigateModalToUri(string uri, NavigationParameters navigationParameters)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("Value cannot be null or white space.", nameof(uri));

            if (navigationParameters == null)
                throw new ArgumentNullException(nameof(navigationParameters));

            try
            {
                await NavigationService.NavigateAsync(uri, navigationParameters, true);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        protected async Task NavigateModalToUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("Value cannot be null or white space.", nameof(uri));

            try
            {
                await NavigationService.NavigateAsync(uri, useModalNavigation: true);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        protected virtual Task HandleException(Exception ex)
        {
            ClearIsBusy();
            var baseException = ex.GetBaseException();
            return PageDialogService.DisplayAlertAsync(CaptionError, baseException.Message, ButtonTextOK);
        }

        protected void ClearIsBusy()
        {
            DeviceService.BeginInvokeOnMainThread(() => IsBusy = false);
        }

        protected void SetIsBusy()
        {
            DeviceService.BeginInvokeOnMainThread(() => IsBusy = true);
        }

        protected virtual void OnIsBusyChanged()
        {
        }

        protected virtual async Task GoBack( NavigationParameters navParams = null, bool animate  = false)
        {
            try
            {
                await NavigationService.GoBackAsync(navParams, true, animate);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        #endregion
    }

}
