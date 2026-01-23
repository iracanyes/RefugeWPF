using RefugeWPF.ClassesMetiers.Config;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
            // Load environment variables 
            LoadEnvVars();
        }

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
