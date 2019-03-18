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
        }

        private void ShowAddresses()
        {
            List<Person> myAddresses = new List<Person>();
            myAddresses = myDB.ReadDB();
            foreach (Person entry in myAddresses)
            {
                ListViewItem lvi = new ListViewItem();
                listView1.Items.Add(entry);
            }
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
            }
        }
    }
}
