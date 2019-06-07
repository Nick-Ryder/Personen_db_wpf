using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Personen_db_wpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Person myPerson = new Person();
        public AddressDB myDB = new AddressDB();
        public Person SelectedPerson { get; set; }
        String standardSort = "Vorname";

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public MainWindow()
        {
            InitializeComponent();
            ShowAddresses(standardSort);
            LockForm();
        }

        private void ShowAddresses(String sortBy)
        {
            List<Person> myAddresses = new List<Person>();
            myAddresses = myDB.ReadDB(sortBy);
            if (myAddresses != null) { 
                foreach (Person entry in myAddresses)
                {
                    listView1.Items.Add(entry);
                }
            }
            else MessageBox.Show("Datenbank konnte nicht gelesen werden", "Error", MessageBoxButton.OK);
        }

        public void Insert_Click(object sender, EventArgs e)
        {
            GetTextboxEntries();
            // Personendaten in Datenbank eintragen
            int index = myDB.InsertDB(myPerson);            // index = Primärschlüssel des Eintrags in DB
            if (index > -1)
            {
                myPerson.Id = index;
                // Daten in Listview eintragen
                //listView1.Items.Add(myPerson);
                listView1.Items.Clear();
                ShowAddresses(standardSort);
                ClearTextboxEntries();
                LockForm();
            }
            else MessageBox.Show("Datenbankfehler beim eintragen!", "Error", MessageBoxButton.OK);
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                myPerson = listView1.Items.GetItemAt(listView1.SelectedIndex) as Person;
                //MessageBox.Show(listView1.SelectedIndices[0].ToString(), "Hallo", MessageBoxButtons.OK);
                //MessageBox.Show(myPerson.Id.ToString(), "Hallo", MessageBoxButton.OK);
                if (myDB.DeleteFromDB(myPerson.Id))
                {
                    listView1.Items.RemoveAt(listView1.SelectedIndex);
                    ClearTextboxEntries();
                    LockForm();
                    Insert.IsEnabled = false;
                    Delete.IsEnabled = false;
                    Update.IsEnabled = false;
                    New.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Datenbank Fehler", "Achtung", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Kein Eintrag selektiert!", "Achtung", MessageBoxButton.OK);
            }
        }


        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                GetTextboxEntries();
                //myPerson.Id = Convert.ToInt32(listView1.SelectedItems[0].SubItems[8].Text);
                int affectedRows = myDB.UpdateDB(myPerson);
                if (affectedRows > 0)
                {
                    listView1.Items.Clear();
                    ShowAddresses(standardSort);
                    ClearTextboxEntries();
                    LockForm();
                }
                else MessageBox.Show("Datenbankfehler beim Update", "Error", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Es ist kein Eintrag selektiert", "Achtung", MessageBoxButton.OK);
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ClearTextboxEntries();
            UnlockForm();
            Delete.IsEnabled = false;
            Update.IsEnabled = false;
            TextBoxFname.Focus();
        }

        private void ItemActivate(object sender, MouseButtonEventArgs e)
        {
            ListViewEntry_Selected(); 
        }

        private void ListViewEntry_Selected()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                //ListViewItem lvi = listView1.SelectedItems[0];
                myPerson = listView1.Items.GetItemAt(listView1.SelectedIndex) as Person;
                TextBoxFname.Text = myPerson.FName;
                TextBoxLname.Text = myPerson.LName;
                TextBoxStreet.Text = myPerson.Street;
                TextBoxNumber.Text = myPerson.Number;
                TextBoxPlz.Text = myPerson.Plz;
                TextBoxLocation.Text = myPerson.Location;
                TextBoxTelephone.Text = myPerson.Telephone;
                TextBoxEmail.Text = myPerson.Email;

                UnlockForm();
                Insert.IsEnabled = true;
                Update.IsEnabled = true;
                Delete.IsEnabled = true;
            }
        }

        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;
            //MessageBox.Show(headerClicked.Column.Header.ToString(), "hghg", MessageBoxButton.OK);
            
            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;
                    MessageBox.Show(sortBy, "bla", MessageBoxButton.OK);

                    //Sort(sortBy, direction);
                    listView1.Items.Clear();
                    ShowAddresses("Id");

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(listView1.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void GetTextboxEntries()
        {
            myPerson.FName = TextBoxFname.Text;
            myPerson.LName = TextBoxLname.Text;
            myPerson.Street = TextBoxStreet.Text;
            myPerson.Number = TextBoxNumber.Text;
            myPerson.Plz = TextBoxPlz.Text;
            myPerson.Location = TextBoxLocation.Text;
            myPerson.Telephone = TextBoxTelephone.Text;
            myPerson.Email = TextBoxEmail.Text;
        }

        private void ClearTextboxEntries()
        {
            TextBoxFname.Text = string.Empty;
            TextBoxLname.Text = string.Empty;
            TextBoxStreet.Text = string.Empty;
            TextBoxNumber.Text = string.Empty;
            TextBoxPlz.Text = string.Empty;
            TextBoxLocation.Text = string.Empty;
            TextBoxTelephone.Text = string.Empty;
            TextBoxEmail.Text = string.Empty;
        }

        private void LockForm()
        {
            TextBoxFname.IsEnabled = false;
            TextBoxLname.IsEnabled = false;
            TextBoxStreet.IsEnabled = false;
            TextBoxNumber.IsEnabled = false;
            TextBoxPlz.IsEnabled = false;
            TextBoxLocation.IsEnabled = false;
            TextBoxTelephone.IsEnabled = false;
            TextBoxEmail.IsEnabled = false;
            Insert.IsEnabled = false;
            Update.IsEnabled = false;
            Delete.IsEnabled = false;
            New.IsEnabled = true;
        }

        private void UnlockForm()
        {
            TextBoxFname.IsEnabled = true;
            TextBoxLname.IsEnabled = true;
            TextBoxStreet.IsEnabled = true;
            TextBoxNumber.IsEnabled = true;
            TextBoxPlz.IsEnabled = true;
            TextBoxLocation.IsEnabled = true;
            TextBoxTelephone.IsEnabled = true;
            TextBoxEmail.IsEnabled = true;
            Insert.IsEnabled = true;
            Update.IsEnabled = true;
            Delete.IsEnabled = true;
            New.IsEnabled = true;
        }
    }
}
