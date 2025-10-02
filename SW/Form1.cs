using System.ComponentModel;
using System.Text.Json;

namespace WinFormsTestv4
{
    public partial class Form1 : Form
    {
        private HttpClient _httpClient = new HttpClient();
        private List<Character> _characters = new List<Character>();
        private string sortColumn = "name";
        private ListSortDirection sortDirection = ListSortDirection.Ascending;
        public Form1()
        {
            InitializeComponent();
            setupGrid();
            setupCmbFilter();
        }

        private async void btnLoadData_Click(object sender, EventArgs e)
        {
            string url = "https://swapi.dev/api/people/";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonString);
                var results = jsonDoc.RootElement.GetProperty("results");
                _characters = JsonSerializer.Deserialize<List<Character>>(results);
                dataGridView1.DataSource = _characters;
                refreshGrid();
            }
            else
            {
                MessageBox.Show($"Error: {response.StatusCode}");
            }
        }
        private void setupGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "name",
                HeaderText = "Name",
                Name = "name"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "height",
                HeaderText = "Height",
                Name = "height"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "mass",
                HeaderText = "Mass",
                Name = "mass"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "hair_color",
                HeaderText = "Hair Color",
                Name = "hair_color"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "skin_color",
                HeaderText = "Skin Color",
                Name = "skin_color"
            }
            );
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "eye_color",
                HeaderText = "Eye Color",
                Name = "eye_color"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "birth_year",
                HeaderText = "Birth Year",
                Name = "birth_year"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "gender",
                HeaderText = "Gender",
                Name = "gender"
            });

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["eye_color"].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void refreshGrid()
        {
            if (_characters.Count == 0)
            {
                return;
            }
            List<Character> filteredList = _characters;
            string filterText = txtFilter.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(filterText) && cmbFilter.SelectedItem != null)
            {
                string filterBy = cmbFilter.SelectedItem.ToString();
                switch (filterBy)
                {
                    case "Name":
                        filteredList = _characters.Where(c => c.name != null && c.name.ToLower().Contains(filterText)).ToList();
                        break;
                    case "Height":
                        filteredList = _characters.Where(c => c.height != null && c.height.ToLower().Contains(filterText)).ToList();
                        break;
                    case "Mass":
                        filteredList = _characters.Where(c => c.mass != null && c.mass.ToLower().Contains(filterText)).ToList();
                        break;
                    case "Hair Color":
                        filteredList = _characters.Where(c => c.hair_color != null && c.hair_color.ToLower().Contains(filterText)).ToList();
                        break;
                }
            }

            List<Character> sortedList;
            switch (sortColumn)
            {
                case "name":
                    sortedList = (sortDirection == ListSortDirection.Ascending) ?
                        filteredList.OrderBy(c => c.name).ToList() :
                        filteredList.OrderByDescending(c => c.name).ToList();
                    break;
                case "height":
                    sortedList = sortDirection == ListSortDirection.Ascending ?
                        filteredList.OrderBy(c => int.TryParse(c.height, out int h) ? h : int.MaxValue).ToList() :
                        filteredList.OrderByDescending(c => int.TryParse(c.height, out int h) ? h : int.MinValue).ToList();
                    break;
                case "mass":
                    sortedList = sortDirection == ListSortDirection.Ascending ?
                        filteredList.OrderBy(c => double.TryParse(c.mass, out double m) ? m : double.MaxValue).ToList() :
                        filteredList.OrderByDescending(c => double.TryParse(c.mass, out double m) ? m : double.MinValue).ToList();
                    break;
                case "hair_color":
                    sortedList = sortDirection == ListSortDirection.Ascending ?
                        filteredList.OrderBy(c => c.hair_color).ToList() :
                        filteredList.OrderByDescending(c => c.hair_color).ToList();
                    break;
                case "skin_color":
                    sortedList = sortDirection == ListSortDirection.Ascending ?
                        filteredList.OrderBy(c => c.hair_color).ToList() :
                        filteredList.OrderByDescending(c => c.hair_color).ToList();
                    break;
                case "eye_color":
                    sortedList = sortDirection == ListSortDirection.Ascending ?
                        filteredList.OrderBy(c => c.hair_color).ToList() :
                        filteredList.OrderByDescending(c => c.hair_color).ToList();
                    break;
                case "birth_year":
                    sortedList = sortDirection == ListSortDirection.Ascending ?
                        filteredList.OrderBy(c => c.hair_color).ToList() :
                        filteredList.OrderByDescending(c => c.hair_color).ToList();
                    break;
                default:
                    sortedList = filteredList;
                    break;
            }

            dataGridView1.DataSource = sortedList;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = dataGridView1.Columns[e.ColumnIndex];
            if (column.SortMode == DataGridViewColumnSortMode.NotSortable)
            {
                return;
            }
            if (sortColumn == column.Name)
            {
                sortDirection = (sortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                sortColumn = column.Name;
                sortDirection = ListSortDirection.Ascending;
            }
            refreshGrid();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Character newCharacter = new Character
            {
                name = "New Character",
                height = "",
                mass = "",
                hair_color = "",
            };

            using (var editor = new CharacterEditorForm(newCharacter))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    _characters.Add(newCharacter);
                    refreshGrid();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selectedRows = dataGridView1.SelectedRows;
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Please select a character to edit.");
                return;
            }
            var selectedCharacter = (Character)selectedRows[0].DataBoundItem;
            using (var editor = new CharacterEditorForm(selectedCharacter))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    refreshGrid();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedRows = dataGridView1.SelectedRows;
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Please select a character to delete.");
                return;
            }
            var selectedCharacter = (Character)selectedRows[0].DataBoundItem;
            var confirmResult = MessageBox.Show($"Are you sure to delete {selectedCharacter.name}?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                _characters.Remove(selectedCharacter);
                refreshGrid();
            }
        }

        private void setupCmbFilter()
        {
            cmbFilter.Items.Clear();
            var option = new string[]
            {
                "All",
                "Name",
                "Height",
                "Mass",
                "Hair Color",
                "Sking Color",
                "Eye Color",
                "Birth Year",
                "Gender"
            };
            cmbFilter.Items.AddRange(option);
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshGrid();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            refreshGrid();
        }
    }
}