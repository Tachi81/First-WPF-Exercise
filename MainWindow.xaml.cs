using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfBasics.Enums;
using WpfBasics.Models;


namespace WpfBasics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickResetButton(object sender, RoutedEventArgs e)
        {
            var ckList = FindVisualChildren<CheckBox>(OurMainWindow);
            foreach (CheckBox checkBox in ckList)
            {
                checkBox.IsChecked = false;
            }

            var tbList = FindVisualChildren<TextBox>(OurMainWindow);
            foreach (TextBox textBox in tbList)
            {
                textBox.Text = "";
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void ClickAddButton(object sender, RoutedEventArgs e)
        {
            var auto = CreateCar();
            var filename = AppDomain.CurrentDomain.BaseDirectory + @"..\..\CarFiles\" + auto.Name + ".txt";
            Helpers.Serializer.SerializeObject<Car>(auto, filename);
            MessageBox.Show("chyba poszło");

        }

        private Car CreateCar()
        {
            var CreatedCar = new Car
            {
                Name = DescriptionText.Text,

                Revision = Revision.Text,

                Status = Status.Text
            };

            if (IdNumber.Text != "")
            {
                CreatedCar.IdentificationNumber = Int32.Parse(IdNumber.Text.Trim());
            }

            if (Material.Text != "")
            {
                CreatedCar.Material = (MaterialEnum)System.Enum.Parse(typeof(MaterialEnum), Material.
                    Text.Replace(" ", "").Replace(":", ""));
            }

            var productionOptionList = new List<ProductionOptionEnum>();
            var ckList = FindVisualChildren<CheckBox>(OurMainWindow).Where(ck => ck.IsChecked == true);
            foreach (CheckBox checkBox in ckList)
            {
                productionOptionList.Add((ProductionOptionEnum)System.Enum.Parse(typeof(ProductionOptionEnum), ((string)(checkBox.Content))));

            }

            CreatedCar.ProductionOptions = productionOptionList;

            if (LengthText.Text != "")
            {
                CreatedCar.Length = Double.Parse(LengthText.Text.Trim());
            }
            if (MassText.Text != "")
            {
                CreatedCar.Weight = Double.Parse(MassText.Text.Trim());
            }

            CreatedCar.Finish = (FinishEnum)System.Enum.Parse(typeof(FinishEnum), FinishDropdown.
                Text.Replace(" ", "").Replace(":", ""));

            CreatedCar.PurchaseInformation = (PurchaseInformationEnum)System.Enum.
                Parse(typeof(PurchaseInformationEnum), PurchaseInformation.Text.Replace(" ", "").Replace(":", ""));

            CreatedCar.SupplierName = SupplierNameText.Text;

            if (SupplierCode.Text != "")
            {
                CreatedCar.SupplierCode = Int32.Parse(SupplierCode.Text.Trim());
            }

            CreatedCar.Note = NoteText.Text;

            return CreatedCar;
        }

        private void ClickFindButton(object sender, RoutedEventArgs e)
        {

            var auto = FindCarFile();
            FillFieldsInApp(auto);

        }

        private void FillFieldsInApp(Car car)
        {
            DescriptionText.Text = car.Name;

            Revision.Text = car.Revision;

            Status.Text = car.Status;
            LengthText.Text = car.Length.ToString();

            var checkboxes = FindVisualChildren<CheckBox>(OurMainWindow).ToList();
            foreach (var chckbox in car.ProductionOptions)
            {
                var checkboxName = chckbox.ToString();
                var ckbx = checkboxes.Single(ck => ck.Name.Replace("Checkbox", "") == checkboxName);
                ckbx.IsChecked = true;
            }

        }

        private Car FindCarFile()
        {
            var car = new Car();
            var searchedText = AppDomain.CurrentDomain.BaseDirectory + @"..\..\CarFiles\" + DescriptionText.Text + ".txt";
            if (File.Exists(searchedText))
            {
                car = Helpers.Serializer.DeSerializeObject<Car>(searchedText);
            }
            else
            {
                MessageBox.Show("There is no such car");
            }
            return car;

        }
    }
}
