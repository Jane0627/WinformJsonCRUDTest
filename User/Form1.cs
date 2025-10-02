using Newtonsoft.Json;
using System;

namespace WinFormsTest
{
    public partial class Form1 : Form
    {
        private HttpClient _httpClient = new HttpClient();
        private List<User> _users = new List<User>();
        public Form1()
        {
            InitializeComponent();
            
        }

        private async void btnLoadData_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "https://jsonplaceholder.typicode.com/users";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                _users = JsonConvert.DeserializeObject<List<User>>(json);
                MessageBox.Show("Data loaded successfully.");
                RefreshGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void RefreshGridView()
        {
            string filter = txtFilter.Text.ToLower();
            var dataToShow = _users; // default to show all
            if (!string.IsNullOrEmpty(filter))
            {
                dataToShow = _users
                    .Where(u => u.Name.ToLower().Contains(filter) ||
                           u.Email.ToLower().Contains(filter) ||
                           u.Address.ToString().ToLower().Contains(filter)).ToList();

            }
            dataGridViewUsers.DataSource = null;
            dataGridViewUsers.DataSource = dataToShow.OrderBy(u => u.Name).ToList();

            SetupGridViewColumns();
        }

        private void SetupGridViewColumns()
        {
            if (dataGridViewUsers.ColumnCount < 1)
            {
                return;
            }
            dataGridViewUsers.ReadOnly = true; // default to read-only
            dataGridViewUsers.Columns["Address"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            RefreshGridView();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var new_user = new User { Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1, Address = new Address() };
            var editor = new UserEditorForm(new_user);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                _users.Add(new_user);
                RefreshGridView();
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to edit.");
                return;
            }
            var selected_user = (User)dataGridViewUsers.SelectedRows[0].DataBoundItem;
            if (selected_user == null) { return; }
            var editor = new UserEditorForm(selected_user);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                RefreshGridView();
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }
            var selected_user = (User)dataGridViewUsers.SelectedRows[0].DataBoundItem;
            if (selected_user == null) { return; }
            var confirm = MessageBox.Show($"Are you sure you want to delete user '{selected_user.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                _users.Remove(selected_user);
                RefreshGridView();
            }
        }
    }
}
