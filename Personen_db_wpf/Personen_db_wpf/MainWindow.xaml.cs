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

        public MainWindow()
        {
            InitializeComponent();
            ShowAddresses();
            LockForm();
        }

        private void ShowAddresses()
        {
            List<Person> myAddresses = new List<Person>();
            myAddresses = myDB.ReadDB();
            foreach (Person entry in myAddresses)
            {
                listView1.Items.Add(entry);
            }
        }

        public void Insert_Click(object sender, EventArgs e)
        {
            myPerson.FName = TextBoxFname.Text;
            myPerson.LName = TextBoxLname.Text;
            myPerson.Street = TextBoxStreet.Text;
            myPerson.Number = TextBoxNumber.Text;
            myPerson.Plz = TextBoxPlz.Text;
            myPerson.Location = TextBoxLocation.Text;
            myPerson.Telephone = TextBoxTelephone.Text;
            myPerson.Email = TextBoxEmail.Text;
            // Personendaten in mdf eintragen
            int index = myDB.InsertDB(myPerson);            // index = Primärschlüssel des geänderten Eintrags in DB
            myPerson.Id = index;
            
            // Daten in Listview eintragen
            listView1.Items.Add(myPerson);
            ClearForm();
            LockForm();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                myPerson = (Person)listView1.Items.GetItemAt(listView1.SelectedIndex);
                //MessageBox.Show(listView1.SelectedIndices[0].ToString(), "Hallo", MessageBoxButtons.OK);
                MessageBox.Show(myPerson.Id.ToString(), "Hallo", MessageBoxButton.OK);
                if (myDB.RemoveFromDb(myPerson.Id))
                {
                    listView1.Items.RemoveAt(listView1.SelectedIndex);
                    ClearForm();
                    LockForm();
                    Insert.IsEnabled = false;
                    Delete.IsEnabled = false;
                    Change.IsEnabled = false;
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

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            UnlockForm();
            Delete.IsEnabled = false;
            Change.IsEnabled = false;
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
                myPerson = (Person)listView1.Items.GetItemAt(listView1.SelectedIndex);
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
                Change.IsEnabled = true;
                Delete.IsEnabled = true;
            }
        }

        private void ClearForm()
        {
            TextBoxFname.Text = "";
            TextBoxLname.Text = "";
            TextBoxStreet.Text = "";
            TextBoxNumber.Text = "";
            TextBoxPlz.Text = "";
            TextBoxLocation.Text = "";
            TextBoxTelephone.Text = "";
            TextBoxEmail.Text = "";
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
            Change.IsEnabled = false;
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
            Change.IsEnabled = true;
            Delete.IsEnabled = true;
            New.IsEnabled = true;
        }
    }
}
