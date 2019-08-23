//using CommonServiceLocator;
using Prism.Unity;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager
{
    public class Bootstrapper:UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            //return ServiceLocator.Current.GetInstance<MainWindow>();
            return Container.Resolve<MainWindow>();
            //return new MainWindow();
        }
        protected override void InitializeShell()
        {
            //Application.Current.MainWindow = Shell;
            Application.Current.MainWindow.Show();
        }
    }
}
