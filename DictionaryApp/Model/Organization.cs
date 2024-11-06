using DictionaryApp.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DictionaryApp.Model
{
    public class Organization : INotifyPropertyChanged
    {
        [Key]
        public int OrganizationId { get; set; }  // Primary Key

        public string Name { get; set; }
        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}