using HandyControl.Tools;
using RefugeWPF.ClassesMetiers.Config;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace RefugeWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Chargement des variables d'environnement 
            LoadEnvVars();

            // Définition de la langue d'affichage pour les composants HandyControl.DatePicker
            ConfigHelper.Instance.SetLang("fr");
        }

        /**
         * <summary>
         *  Chargement des variables d'environnement contenues 
         * </summary>
         */ 
        public void LoadEnvVars()
        {
            Trace.WriteLine("App - Loading environment variables...");

            try
            {
                // Load environment files .env
                var root = System.IO.Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\");
                var dotEnvFile = System.IO.Path.Combine(root, ".env");

                // Load environment variables
                DotEnv.Load(dotEnvFile);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                
            }
        }
    }



}
