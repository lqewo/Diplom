using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Diplom.Mobile
{
    public static class MySettings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string UserName //Логин
        {
            get => AppSettings.GetValueOrDefault(nameof(UserName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserName), value);
        }

        public static string Token //Токен
        {
            get => AppSettings.GetValueOrDefault(nameof(Token), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Token), value);
        }

        public static string Email //Email
        {
            get => AppSettings.GetValueOrDefault(nameof(Email), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Email), value);
        }

        public static string UserId
        {
            get => AppSettings.GetValueOrDefault(nameof(UserId), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserId), value);
        }

        public static string Role //Роль
        {
            get => AppSettings.GetValueOrDefault(nameof(Role), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Role), value);
        }

        public static void Clear()
        {
            Token = "";
            Email = "";
            UserId = "";
            Role = "";
        }
    }
}