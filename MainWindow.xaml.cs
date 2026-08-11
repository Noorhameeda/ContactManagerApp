using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace ContactManagerApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client = new() { BaseAddress = new Uri("http://localhost:5003") };

        public MainWindow()
        {
            InitializeComponent();
            LoadContacts();
        }

        private async void LoadContacts()
        {
            var contacts = await _client.GetFromJsonAsync<List<Contact>>("/contacts");
            ContactListBox.Items.Clear();
            if (contacts != null)
                foreach (var c in contacts)
                    ContactListBox.Items.Add($"{c.Id}: {c.Name} | {c.Phone} | {c.Email}");
        }

        private async void AddContact_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text)) return;

            var contact = new Contact
            {
                Name = NameBox.Text,
                Phone = PhoneBox.Text,
                Email = EmailBox.Text
            };

            await _client.PostAsJsonAsync("/contacts", contact);
            NameBox.Clear();
            PhoneBox.Clear();
            EmailBox.Clear();
            LoadContacts();
        }

        private async void DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            if (ContactListBox.SelectedItem == null) return;
            var selected = ContactListBox.SelectedItem.ToString();
            var id = int.Parse(selected.Split(':')[0]);
            await _client.DeleteAsync($"/contacts/{id}");
            LoadContacts();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadContacts();
    }
}