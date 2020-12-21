using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FileManager;

namespace Explorer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly Logger logger;

        private readonly DataManager.DataManager manager;


        public MainViewModel()
        {
            ProductClick = new DelegateCommand(ProductOpen);
            ProductCategoryClick = new DelegateCommand(ChooseCategory);
            logger = new Logger();
            var thread = new Thread(logger.Start);
            thread.Start();
            try
            {
                manager = new DataManager.DataManager();
                manager.MakeLog();

                PreInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("MainWindow() :\n" + ex.Message);
            }
        }

        public ObservableCollection<ListViewItem> ProductCategories { get; set; } =
            new ObservableCollection<ListViewItem>();

        public ObservableCollection<ListViewItem> Products { get; set; } = new ObservableCollection<ListViewItem>();

        public ObservableCollection<ListViewItem> SelectedProducts { get; set; } =
            new ObservableCollection<ListViewItem>();

        public ObservableCollection<ListViewItem> SelectedProductCategpries { get; set; } =
            new ObservableCollection<ListViewItem>();

        public ListViewItem SelectedProductCategory { get; set; }

        public ListViewItem SelectedProduct { get; set; }

        //public ObservableCollection<string> TextProducts { get; set; } = new ObservableCollection<string>();
        public string TextProducts { get; set; }

        public ICommand ProductClick { get; }
        public ICommand ProductCategoryClick { get; }

        private void PreInit()
        {
            var first = new ListViewItem();
            first.Tag = -1;
            first.Content = "...";
            ProductCategories.Add(first);
            foreach (var el in manager.Repository.ProductCategories)
            {
                var tmp = new ListViewItem();
                tmp.Tag = el.ProductCategoryID;
                tmp.Content = el.Name;
                ProductCategories.Add(tmp);
            }

            foreach (var el in manager.Repository.Products)
            {
                var tmp = new ListViewItem();
                tmp.Tag = el.ProductID;
                tmp.Content = el.Name;
                Products.Add(tmp);
            }
        }

        private void ProductOpen(object parametr)
        {
            if (parametr is ListViewItem productItem)
            {
                SelectedProducts.Clear();
                var tmp = new ListViewItem();
                var id = (int) productItem.Tag;
                tmp.Content += $"Name: {manager.Repository.GetProduct(id).Name}\n" +
                               $"Product number {manager.Repository.GetProduct(id).ProductNumber}\n" +
                               $"Size: {manager.Repository.GetProduct(id).Size}\n" +
                               $"Published {manager.Repository.GetProduct(id).SellStartDate}";
                SelectedProducts.Add(tmp);
            }
        }

        private void ChooseCategory(object parametr)
        {
            if (parametr is ListViewItem productCategoryItem)
            {
                ListBoxItem item = productCategoryItem;
                var id = (int) item.Tag;
                if (id == -1)
                {
                    Products.Clear();
                    foreach (var el in manager.Repository.Products)
                    {
                        var tmp = new ListViewItem();
                        tmp.Tag = el.ProductID;
                        tmp.Content = el.Name;
                        Products.Add(tmp);
                    }
                }
                else
                {
                    Products.Clear();
                    foreach (var el in manager.Repository.Products.Where(ell => ell.ProductCategoryID == id))
                    {
                        var tmp = new ListViewItem();
                        tmp.Tag = el.ProductID;
                        tmp.Content = el.Name;
                        Products.Add(tmp);
                    }
                }
            }
        }
    }
}