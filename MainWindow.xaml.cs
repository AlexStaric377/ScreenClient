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
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading;

namespace ScreenClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        public static double ScreenHeight = 0.0, SetHeigtCurent = 0.0, ScreenWidth = 0.0;
        public static int NumberOrder = 0, CheckBoxOrder = 0, MemStatusKod = 0, CountHeadZakaz = 0, MemCount = 0, MemKodTov = 0, PanelBoard = 0, MemStopDBConect = 0, M_StatusOrder=0;
        public static string MemNameTov = "";
        public static decimal MemOstTov = 0, MemPriceTov = 0;

        private void OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        public static ApplicationContext db = new ApplicationContext();
        public static List<TableOrder> TableGridOrder = new List<TableOrder>(1);

        /// Структура данных для многопотоковой среды (передача аргументов)
        /// </summary>
        public struct RenderInfo
        {
            public string argument1 { get; set; }
            public string argument2 { get; set; }
            public string argument3 { get; set; }
            public string argument4 { get; set; }
            public string argument5 { get; set; }
        }

        #region Вернуть ссылку на главное окно по запросу WPF C# {LinkMainWindow}
        /// <summary>
        /// Вернуть ссылку на окно по запросу WPF C# {ListWindowMain}MainWindow
        /// </summary>
        /// <param name="NameWindow"> Имя главного окна Рабочий стол или Панель</param>
        /// <returns></returns>
        public static dynamic LinkMainWindow(string NameWindow)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Title == NameWindow)
                    return (dynamic)window;
            }
            return null;
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // 1920х1080
            ScreenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

            InitializeComponent();
            TableOrders.Height = MainWindow.ScreenHeight;
            TableOrders.Width = MainWindow.ScreenWidth;
            if (1280 <= MainWindow.ScreenWidth) TableOrders.Width = MainWindow.ScreenWidth;
            else
            {
                var WidthColumn = 1280 - MainWindow.ScreenWidth;
                TableOrders.Columns[0].Width = 250 - WidthColumn / 1.2;
                TableOrders.Columns[1].Width = 350 - WidthColumn / 4;
                TableOrders.Columns[2].Width = 310 - WidthColumn / 4;

            }

            MainWindow.CountHeadZakaz = MainWindow.db.HeadZakazs.Count();
            var ListAdmin = MainWindow.db.AdminSetValues.Where(p => p.Id == 1).FirstOrDefault();
            MainWindow.M_StatusOrder = ListAdmin.StatusOrder;
            RunUpdateStackOrder();
        }

        private void Users_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTableOrder();
        }

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        public  void LoadTableOrder(int StatusOrder1 = 0, int StatusOrder2 = 0, int StatusOrder3 = 0)
        {

            int Service = 0, MemStatus = 0, MemNumber = 0, MemKodTov = 0;
            string TimeVidachi = "", VidObslugi = "", NameTov = "", NameStatus = "";

            List<TableOrder> TableGridOrder = new List<TableOrder>(1);
            try
            {
                var HeadOrderAll = MainWindow.db.HeadZakazs.ToList().OrderBy(row => row.Number).OrderByDescending(row => row.Status);
                foreach (HeadZakaz pole in HeadOrderAll)   //MainWindow.db.HeadZakazs.ToList<HeadZakaz>()
                {

                    if (((StatusOrder1 == pole.Status && StatusOrder2 == 0 && StatusOrder3 == 0) || (StatusOrder2 == pole.Status && StatusOrder1 == 0 && StatusOrder3 == 0) ||
                        (StatusOrder3 == pole.Status && StatusOrder1 == 0 && StatusOrder2 == 0) ||
                        ((StatusOrder2 == pole.Status || StatusOrder1 == pole.Status) && StatusOrder1 != 0 && StatusOrder2 != 0 && StatusOrder3 == 0) ||
                        ((StatusOrder3 == pole.Status || StatusOrder1 == pole.Status) && StatusOrder1 != 0 && StatusOrder3 != 0 && StatusOrder2 == 0) ||
                        ((StatusOrder2 == pole.Status || StatusOrder3 == pole.Status) && StatusOrder2 != 0 && StatusOrder3 != 0 && StatusOrder1 == 0) ||
                        ((StatusOrder2 == pole.Status || StatusOrder3 == pole.Status || StatusOrder1 == pole.Status) && StatusOrder2 != 0 && StatusOrder3 != 0 && StatusOrder1 != 0) ||
                        (StatusOrder2 == 0 && StatusOrder3 == 0 && StatusOrder1 == 0)) && pole.Status != 4 && pole.Status != 5)
                    {
                        Service = pole.PlaceService;
                        MemStatus = pole.Status;
                        MemNumber = pole.IdOrder;


                        if (MainWindow.db.SprVidGiveOuts.Where(s => s.KodGiveOut == Service).FirstOrDefault() != null)
                        {
                            var StrGiveOut = MainWindow.db.SprVidGiveOuts.Where(s => s.KodGiveOut == Service);
                            foreach (SprVidGiveOut strzap in StrGiveOut) { VidObslugi = "      " + strzap.NameGiveOutUk; }
                        }

                        var StrSprStatus = MainWindow.db.SprStatuss.Where(r => r.KodStatus == MemStatus);
                        foreach (SprStatus strzap in StrSprStatus) { NameStatus = strzap.NameStatusUk; }
                        int CountContentZakaz = MainWindow.db.ContentZakazs.Where(p => p.Number == MemNumber).Count();
                        var ContentOrder = MainWindow.db.ContentZakazs.Where(p => p.Number == MemNumber);
                        foreach (ContentZakaz zap in ContentOrder)
                        {
                            NameTov = zap.NameTovUK;
                            MemKodTov = zap.Kodtov;
                            //MemTime = MemTime < zap.Timeissue ? zap.Timeissue : MemTime;
                            TimeVidachi = ""; // "0:" + Convert.ToString(MemTime) + ":00";

                            if (CountContentZakaz == 1) TableGridOrder.Add(new TableOrder(zap.Id, zap.Number.ToString().PadLeft(12), VidObslugi, TimeVidachi, NameTov, zap.Quantity, NameStatus, NameStatus, "1", zap.Number, MemStatus, MemKodTov));
                            CountContentZakaz--;
                        }
                    }
                }
                TableOrders.ItemsSource = TableGridOrder;
            }
            catch (Exception ex)
            {
                PreOrderConection.ErorDebag("возникло исключение:" + Environment.NewLine + " Отсутствует соединение с сервером " + Environment.NewLine + " === Message: " + ex.Message.ToString() + Environment.NewLine + " === Exception: " + ex.ToString(), 2, 0, (PreOrderConection.StruErrorDebag)null);

                string TextWindows = "Отсутствует соединение с сервером" + Environment.NewLine + " Проверте включен ли сервер.";
                MessageError NewOrder = new MessageError(TextWindows, 1);
                NewOrder.Show();
                Thread.Sleep(10000);
                Environment.Exit(0);
            }
        }



        // запуск потока слежения за пасивностью клиента
        public static void RunUpdateStackOrder()
        {
            MainWindow.RenderInfo Arguments01 = new MainWindow.RenderInfo();
            Thread thread = new Thread(ThreadUpdateStackOrder);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true; // Фоновый поток
            thread.Start(Arguments01);
            //MainWindow.TimeOutRun = 1;
        }


        public static void ThreadUpdateStackOrder(object ThreadObj)
        {
            int StatusOrder = 0;
            while (0 == 0)
            {
                Thread.Sleep(5000);
                try
                {
                    ApplicationContext dbUpdate = new ApplicationContext();
                    MainWindow.MemCount = dbUpdate.HeadZakazs.Count();
                    var ListAdmin = dbUpdate.AdminSetValues.Where(p => p.Id == 1).FirstOrDefault();
                    StatusOrder = ListAdmin.StatusOrder;
                }
                catch (Exception ex)
                {
                    PreOrderConection.ErorDebag("возникло исключение:" + Environment.NewLine + " Отсутствует соединение с сервером " + Environment.NewLine + " === Message: " + ex.Message.ToString() + Environment.NewLine + " === Exception: " + ex.ToString(), 2, 0, (PreOrderConection.StruErrorDebag)null);

                    string TextWindows = "Отсутствует соединение с сервером" + Environment.NewLine + " Проверте включен ли сервер.";
                    MessageError NewOrder = new MessageError(TextWindows, 1);
                    NewOrder.Show();
                    Thread.Sleep(10000);
                    Environment.Exit(0);
                }
                
                if (MainWindow.CountHeadZakaz != MainWindow.MemCount || MainWindow.M_StatusOrder != StatusOrder)
                {

                    MainWindow.CountHeadZakaz = MainWindow.MemCount;
                    MainWindow.M_StatusOrder = StatusOrder;

                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate ()
                    {
                        MainWindow Client_Link = MainWindow.LinkMainWindow("MainWindow");
                        Client_Link.LoadTableOrder();

                    }));

                }
            }

        }


        //    ClientListOrder Client_link = new ClientListOrder();
        //    Client_link.Show();
        //}


        //private void Button_Click(object sender, MouseButtonEventArgs e)
        //{

        //    ClientListOrder Client_link = new ClientListOrder();
        //    Client_link.Show();
        //    //Close();
        //}

        //private void ExitPreOrder(object sender, MouseButtonEventArgs e)
        //{
        //    Environment.Exit(0);
        //}
    }
}
