


using System;
using Prism;
using Prism.Ioc;
using Prism.Modularity;

using BETRWallet.ViewModels;
using BETRWallet.Views;
using Xamarin.Forms;
using Prism.DryIoc;
using BETRWallet.Interfaces;
using AndroidSpecific = Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;
using System.Reflection;
using BETRWallet.Services;
using Prism.Navigation;

namespace BETRWallet
{
    [XamlCompilation (XamlCompilationOptions.Compile)]
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { 


            AndroidSpecific.Application.SetWindowSoftInputModeAdjust(this, AndroidSpecific.WindowSoftInputModeAdjust.Resize); 

        }




        protected  override void OnInitialized()
        {
            InitializeComponent();    // splash

            //  InitLocalization();

            NavigationService.NavigateAsync($"app:////{nameof(SplashPage)}");


            //  new System.Uri($"/NavigationPage/{nameof(SplashPage)}", UriKind.Relative));
            //NavigationService.NavigateAsync(new System.Uri("/NavigationPage/CustomTabbedPage?selectedTab=HomePage", System.UriKind.Absolute));
        }

 

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            

            // Register the repository as a singleton
            containerRegistry.RegisterSingleton<IRepositoryService, RepositoryService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
 
            containerRegistry.RegisterForNavigation<SplashPage, SplashPageViewModel>();

      

        }

    }
}