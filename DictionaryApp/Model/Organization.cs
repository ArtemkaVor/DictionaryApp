using DictionaryApp.Model;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace DictionaryApp.Model
{
    public class Organization
    {
        [Key]
        public int OrganizationId { get; set; }  // Primary Key

        public string Name { get; set; }
        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
    }
}