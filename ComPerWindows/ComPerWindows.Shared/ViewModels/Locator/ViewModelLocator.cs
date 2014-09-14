/*
  In App.xaml:
  <Application.Resources>
      <vm:Locator xmlns:vm="clr-namespace:HushWP8"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/


using ComPerWindows.Services.Implementations;
using ComPerWindows.Services.Interfaces;
using ComPerWindows.ViewModels.Pages;

namespace ComPerWindows.ViewModels.Locator
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;
    using ComPerWindows.Services.Implementations;
    using ComPerWindows.Services.Interfaces;
    using ComPerWindows.ViewModels.Pages;

    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the Locator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models

            }

            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<ITestService, TestService>();
            SimpleIoc.Default.Register<TestPageViewModel>();
        }

        public TestPageViewModel TestPage
        {
            get { return ServiceLocator.Current.GetInstance<TestPageViewModel>(); }
        }
        
        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<IStorageService>();
            SimpleIoc.Default.Unregister<ITestService>();
            SimpleIoc.Default.Unregister<TestPageViewModel>();
        }
    }
}