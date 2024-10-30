using CommunityToolkit.Mvvm.Input;
using DictionaryApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace DictionaryApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Organization> Organizations { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public ICollectionView FilteredEmployees { get; set; }

        private Organization _selectedOrganization;
        public Organization SelectedOrganization
        {
            get => _selectedOrganization;
            set
            {
                if (_selectedOrganization != value)
                {
                    _selectedOrganization = value;
                    OnPropertyChanged(nameof(SelectedOrganization));
                    LoadEmployees();
                }
            }
        }
        public async Task UpdateOrganizationAsync(Organization organization)
        {
            if (organization != null)
            {
                _context.Organizations.Update(organization);
                await _context.SaveChangesAsync();
            }
        }

        private void OnOrganizationNameChanged()
        {
            if (_selectedOrganization != null)
            {
                UpdateOrganizationAsync(_selectedOrganization);
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (_selectedEmployee != value)
                {
                    _selectedEmployee = value;
                    OnPropertyChanged(nameof(SelectedEmployee));
                    if (_selectedEmployee != null)
                    {
                        _selectedEmployee.PropertyChanged += async (s, e) =>
                        {
                            if (e.PropertyName == nameof(Employee.FullName) || e.PropertyName == nameof(Employee.Position))
                            {
                                await UpdateEmployeeAsync(_selectedEmployee);
                            }
                        };
                    }
                }
            }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                    FilteredEmployees.Refresh();
                }
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand AddOrganizationCommand { get; }
        public ICommand DeleteOrganizationCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand LoadPhotoCommand { get; }
        public ICommand AddOrganizationsWithEmployeesCommand { get; }

        public MainViewModel(AppDbContext context)
        {
            _context = context;

            Organizations = new ObservableCollection<Organization>();
            Employees = new ObservableCollection<Employee>();

            FilteredEmployees = CollectionViewSource.GetDefaultView(Employees);
            FilteredEmployees.Filter = FilterEmployees;

            SearchCommand = new RelayCommandAsync(async () => await SearchEmployeesAsync());
            LoadDataCommand = new RelayCommandAsync(async () => await LoadDataAsync());
            AddOrganizationCommand = new RelayCommandAsync(async () => await AddOrganizationAsync());
            DeleteOrganizationCommand = new RelayCommandAsync(async () => await DeleteOrganizationAsync());
            AddEmployeeCommand = new RelayCommandAsync(async () => await AddEmployeeAsync());
            DeleteEmployeeCommand = new RelayCommandAsync(async () => await DeleteEmployeeAsync());
            LoadPhotoCommand = new RelayCommandAsync(async () => await LoadPhotoAsync());
            AddOrganizationsWithEmployeesCommand = new RelayCommandAsync(async () => await AddOrganizationsWithEmployeesAsync());
            LoadDataCommand.Execute(null);
        }

        private async Task LoadDataAsync()
        {
            var organizations = await _context.Organizations.Include(o => o.Employees).ToListAsync();
            Organizations.Clear();
            foreach (var org in organizations)
            {
                Organizations.Add(org);
            }
        }

        private async Task AddOrganizationAsync()
        {
            var organizationName = SelectedOrganization?.Name ?? "Новая Организация"; 
            var newOrg = new Organization { Name = organizationName };
            await _context.Organizations.AddAsync(newOrg);
            await _context.SaveChangesAsync();
            Organizations.Add(newOrg);
            SelectedOrganization = newOrg;
        }

        private async Task DeleteOrganizationAsync()
        {
            if (SelectedOrganization != null)
            {
                _context.Organizations.Remove(SelectedOrganization);
                await _context.SaveChangesAsync();
                Organizations.Remove(SelectedOrganization);
                SelectedOrganization = null;
            }
        }

        private async Task AddEmployeeAsync()
        {
            if (SelectedOrganization != null)
            {
                var newEmployee = new Employee { FullName = "Новый Сотрудник", Position = "Должность", PhotoPath = "\\source\\repos\\DictionaryApp\\DictionaryApp\\View\\EmployeePhoto\\PhotoPlug.jpg" };
                SelectedOrganization.Employees.Add(newEmployee);
                await _context.SaveChangesAsync();
                Employees.Add(newEmployee);
                SelectedEmployee = newEmployee;
            }
        }

        private async Task DeleteEmployeeAsync()
        {
            if (SelectedEmployee != null && SelectedOrganization != null)
            {
                SelectedOrganization.Employees.Remove(SelectedEmployee);
                await _context.SaveChangesAsync();
                Employees.Remove(SelectedEmployee);
                SelectedEmployee = null;
            }
        }

        private async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        private async Task LoadPhotoAsync()
        {
            if (SelectedEmployee == null)
                return;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фотографию сотрудника"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedEmployee.PhotoPath = openFileDialog.FileName;
                await UpdateEmployeeAsync(SelectedEmployee);
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        private bool FilterEmployees(object obj)
        {
            if (obj is Employee employee)
            {
                return string.IsNullOrEmpty(SearchQuery) ||
                       employee.FullName.Contains(SearchQuery) ||
                       employee.Position.Contains(SearchQuery);
            }
            return false;
        }

        private void LoadEmployees()
        {
            Employees.Clear();
            if (SelectedOrganization != null)
            {
                foreach (var employee in SelectedOrganization.Employees)
                {
                    Employees.Add(employee);
                }
            }
            FilteredEmployees.Refresh();
        }

        private async Task SearchEmployeesAsync()
        {
            await Task.Delay(100);
            FilteredEmployees.Refresh();
        }

        private async Task AddOrganizationsWithEmployeesAsync()
        {
            for (int i = 1; i <= 10; i++)
            {
                var organization = new Organization { Name = $"Организация {i}" };

                for (int j = 1; j <= 100; j++)
                {
                    var employee = new Employee
                    {
                        FullName = $"Сотрудник {j}",
                        Position = $"Должность {j}",
                        PhotoPath = "EmployeePhoto/PhotoPlug.jpg"
                    };
                    organization.Employees.Add(employee);
                }

                await _context.Organizations.AddAsync(organization);
            }

            await _context.SaveChangesAsync();
            await LoadDataAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}