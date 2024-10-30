using System.Windows;
using DictionaryApp.Model;
using DictionaryApp.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApp
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;

        public MainWindow()
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=appdatabase.db")
                .Options;
            _context = new AppDbContext(options);

            _context.Database.EnsureCreated();

            DataContext = new MainViewModel(_context);
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}