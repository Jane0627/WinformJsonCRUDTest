using Newtonsoft.Json;
using System.ComponentModel;

namespace WinFormsTestv3
{
    public partial class Form1 : Form
    {
        private HttpClient _httpClient = new HttpClient();
        //SortableBindingList<Post> _allPost = new SortableBindingList<Post>(new List<Post>());
        private List<Post> _allPost = new List<Post>();
        //BindingSource _bindingSource = new BindingSource();
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private string _sortColumn = "id";

        public Form1()
        {
            InitializeComponent();
            setupDataGridView();
            setupCmbFilter();
        }

        private async void btnLoadData_Click(object sender, EventArgs e)
        {
            string url = "https://jsonplaceholder.typicode.com/posts";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                _allPost = JsonConvert.DeserializeObject<List<Post>>(json);
                //dataGridView1.AutoGenerateColumns = true;
                refreshGrid();
            }
            else
            {
                MessageBox.Show("Error fetching data");
            }
        }

        private void setupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                HeaderText = "ID"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "title",
                HeaderText = "Title",
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "body",
                HeaderText = "Body",
                Name = "body"
            });

            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["body"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void setupCmbFilter()
        {
            var options = new List<string> { "ID", "Title", "Body" };
            cmbFilter.DataSource = options;
        }

        private void refreshGrid()
        {
            if (_allPost == null || _allPost.Count == 0)
            {
                dataGridView1.DataSource = null;
                return;
            }

            List<Post> sortedList = _allPost;
            List<Post> dataToShow = _allPost;
            string keyword = txtFilter.Text.ToLower();
            string filterColumn = cmbFilter.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(keyword) && !string.IsNullOrEmpty(filterColumn))
            {
                switch (filterColumn)
                {
                    case "ID":
                        if (int.TryParse(keyword, out int idValue))
                        {
                            dataToShow = dataToShow.Where(p => p.id == idValue).ToList();
                        }
                        else
                        {
                            dataToShow = new List<Post>();
                        }
                        break;
                    case "Title":
                        dataToShow = dataToShow.Where(p => p.title != null && p.title.ToLower().Contains(keyword)).ToList();
                        break;
                    case "Body":
                        dataToShow = dataToShow.Where(p => p.body != null && p.body.ToLower().Contains(keyword)).ToList();
                        break;
                }
            }

            switch (_sortColumn)
            {
                case "id":
                    if (_sortDirection == ListSortDirection.Ascending)
                    {
                        sortedList = dataToShow.OrderBy(p => p.id).ToList();
                    }
                    else
                    {
                        sortedList = dataToShow.OrderByDescending(p => p.id).ToList();
                    }
                    break;
                case "title":
                    if (_sortDirection == ListSortDirection.Ascending)
                    {
                        sortedList = dataToShow.OrderBy(p => p.title).ToList();
                    }
                    else
                    {
                        sortedList = dataToShow.OrderByDescending(p => p.title).ToList();
                    }
                    break;
                case "body":
                    if (_sortDirection == ListSortDirection.Ascending)
                    {
                        sortedList = dataToShow.OrderBy(p => p.body).ToList();
                    }
                    else
                    {
                        sortedList = dataToShow.OrderByDescending(p => p.body).ToList();
                    }
                    break;
                default:
                    sortedList = dataToShow;
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
            if (_sortColumn == column.DataPropertyName)
            {
                //toggle sort direction
                _sortDirection = (_sortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                _sortColumn = column.DataPropertyName;
                _sortDirection = ListSortDirection.Ascending;
            }

            refreshGrid();

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Post newPost = new Post { userId = 1, id = (_allPost.Count > 0) ? _allPost.Max(p => p.id) + 1 : 1 };
            using (PostEditorForm editor = new PostEditorForm(newPost))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    _allPost.Add(editor.currentPost);
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
            var selectedPost = (Post)dataGridView1.SelectedRows[0].DataBoundItem;
            if (selectedPost == null)
            {
                MessageBox.Show("Selected row is invalid.");
                return;
            }
            using (PostEditorForm editor = new PostEditorForm(selectedPost))
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
            var selectedPost = (Post)dataGridView1.SelectedRows[0].DataBoundItem;
            if (selectedPost == null)
            {
                MessageBox.Show("Selected row is invalid.");
                return;
            }
            var confirmResult = MessageBox.Show("Are you sure to delete this item?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                _allPost.Remove(selectedPost);
                refreshGrid();
            }
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
