using System;
using BETRWallet.Interfaces;
using Prism.Navigation;
using Prism.Services;

namespace BETRWallet.ViewModels
{
    public class SplashPageViewModel :ViewModelBase
    {
        public SplashPageViewModel(IRepositoryService repositoryService, IDeviceService deviceService, INavigationService navigationService, IPageDialogService pageDialogService, DryIoc.IContainer container)
            : base(repositoryService, deviceService, navigationService, pageDialogService, container)
        {
            
        }
    }
}
