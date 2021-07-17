namespace Diplom.Mobile
{
    public interface IPath
    {
        //Через метод GetDatabasePath мы будем получать из разных ОС путь к базе данных
        string GetDatabasePath(string filename);
    }
}