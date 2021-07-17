using System;
using System.IO;
using Diplom.Mobile.Android;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidDbPath))]

namespace Diplom.Mobile.Android
{
    internal class AndroidDbPath : IPath
    {
        public string GetDatabasePath(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
        }
    }
}