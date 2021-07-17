using Diplom.Common.Entities;
using Diplom.Common.Models;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace Diplom.Mobile
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<Product> Product { get; set; }

        public DbSet<Review> Review { get; set; }

        public DbSet<ReviewShow> ReviewShow { get; set; } //отображаемые отзывы

        public DbSet<Content> Content { get; set; }

        public DbSet<Basket> Basket { get; set; }

        public DbSet<BasketList> BasketList { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderList> OrderList { get; set; }

        //Через конструктор объект этого класса получает в переменную _databasePath путь к базе данных
        private readonly string _databasePath;

        public ApplicationContext()
        {
            const string dbFileName = "clientapp.db";
            _databasePath = DependencyService.Get<IPath>().GetDatabasePath(dbFileName);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databasePath}");
        }
    }
}