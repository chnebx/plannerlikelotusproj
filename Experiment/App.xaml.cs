using Experiment.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Experiment
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DatabaseName = "Narnya.DB";
        public static string FolderPath = GlobalSettings.DefaultDBFolder;
        public static string DatabasePath = System.IO.Path.Combine(FolderPath, DatabaseName);
    }
}
