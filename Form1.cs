using Newtonsoft.Json;
using System.ComponentModel;
using System.Data;
using System.Xml.Serialization;

namespace WinFormsTestv5
{
    public partial class Form1 : Form
    {
        private List<Result> _allResults = new List<Result>();
        private readonly string _dataFilePath = "Data\\pokemon.json";
        private string columnToSort = "id";
        private ListSortDirection sortDirection = ListSortDirection.Ascending;
        public Form1()
        {
            InitializeComponent();
            setupGrid();
            setupCmbFilter();
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            var json = File.ReadAllText(_dataFilePath);
            var rootBbject = JsonConvert.DeserializeObject<Rootobject>(json);
            _allResults = rootBbject.results.ToList();
            int currentId = 1;
            foreach (var pokemon in _allResults)
            {
                pokemon.id = currentId++;
            }
            refreshGrid();
        }

        private void setupGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "id", Name = "id" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "name", Name = "name" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "URL", DataPropertyName = "url", Name = "url", SortMode = DataGridViewColumnSortMode.NotSortable });
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
        }

        private void refreshGrid()
        {
            if (_allResults.Count == 0 || _allResults == null)
            {
                dataGridView1.DataSource = null;
                return;
            }
            List<Result> filterResult = _allResults;
            var filterText = txtFilter.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(filterText))
            {
                switch (cmbFilter.SelectedItem.ToString())
                {
                    case "Name":
                        filterResult = _allResults.Where(r => r.name != null && r.name.ToLower().Contains(filterText)).ToList();
                        break;
                    case "URL":
                        filterResult = _allResults.Where(r => r.url != null && r.url.ToLower().Contains(filterText)).ToList();
                        break;
                    default:
                        filterResult = _allResults.Where(r => r.name.ToLower().Contains(filterText) || r.url.ToLower().Contains(filterText)).ToList();
                        break;
                }
            }

            List<Result> sortedResults;

            switch (columnToSort)
            {
                case "name":
                    sortedResults = sortDirection == ListSortDirection.Ascending ?
                        filterResult.OrderBy(r => r.name).ToList() :
                        filterResult.OrderByDescending(r => r.name).ToList();
                    break;
                default:
                    sortedResults = filterResult;
                    break;
            }
            dataGridView1.DataSource = sortedResults;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var columnName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (dataGridView1.Columns[columnName].SortMode == DataGridViewColumnSortMode.NotSortable)
            {
                return; // Ignore clicks on non-sortable columns
            }
            if (columnToSort == columnName)
            {
                // Toggle sort direction
                sortDirection = sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                // New column to sort by, default to ascending
                columnToSort = columnName;
                sortDirection = ListSortDirection.Ascending;
            }
            refreshGrid();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Result newPokemon = new Result { name = "New Pokemon", url = "" };
            using (var editor = new PokemonEditorForm(newPokemon))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    _allResults.Add(newPokemon);
                    refreshGrid();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to edit.");
                return;
            }
            var selectedRow = dataGridView1.SelectedRows[0];
            var selectedPokemon = (Result)selectedRow.DataBoundItem;
            using (var editor = new PokemonEditorForm(selectedPokemon))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    refreshGrid();
                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }
            var selectedRow = dataGridView1.SelectedRows[0];
            var selectedPokemon = (Result)selectedRow.DataBoundItem;
            var confirmResult = MessageBox.Show($"Are you sure to delete {selectedPokemon.name}?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                _allResults.Remove(selectedPokemon);
                refreshGrid();
            }
        }

        private void setupCmbFilter()
        {
            cmbFilter.Items.Clear();
            var option = new List<string> { "All", "Name", "URL" };
            cmbFilter.DataSource = option;

        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshGrid();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            refreshGrid();
        }

        private void btnExportXML_Click(object sender, EventArgs e)
        {
            if (_allResults == null || _allResults.Count == 0)
            {
                MessageBox.Show("No data to export.");
                return;
            }
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Export to XML"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<Result>));
                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        xmlSerializer.Serialize(writer, _allResults);
                    }
                    MessageBox.Show("Data exported successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting data: {ex.Message}");
                }

            }
        }
    }
}
