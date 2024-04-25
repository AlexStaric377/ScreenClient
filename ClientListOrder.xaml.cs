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
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading;

namespace ScreenClient
{
    /// <summary>
    /// Логика взаимодействия для ClientListOrder.xaml
    /// </summary>
    public partial class ClientListOrder : Window
    {
        public ClientListOrder()
        {
            InitializeComponent();
            TableOrders.Height = MainWindow.ScreenHeight;
            TableOrders.Width = MainWindow.ScreenWidth;
            if (1280 <= MainWindow.ScreenWidth) TableOrders.Width = MainWindow.ScreenWidth; 
            else
            {
                var WidthColumn = 1280 - MainWindow.ScreenWidth;
                TableOrders.Columns[0].Width = 250 - WidthColumn/1.2;
                TableOrders.Columns[1].Width = 350 - WidthColumn / 4;
                TableOrders.Columns[2].Width = 310 - WidthColumn / 4;
                
            }
            var ListAdmin = MainWindow.db.AdminSetValues.Where(p => p.Id == 1).FirstOrDefault();
            MainWindow.M_StatusOrder = ListAdmin.StatusOrder;
            MainWindow.CountHeadZakaz = MainWindow.db.HeadZakazs.Count();
            RunUpdateStackOrder();
            LoadTableOrder();
        }

        private void Users_Loaded(object sender, RoutedEventArgs e)
        {
            //LoadTableOrder();
        }

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        public static void LoadTableOrder(int StatusOrder1 = 0, int StatusOrder2 = 0, int StatusOrder3 = 0)
        {

            int Service = 0, MemStatus = 0, MemNumber = 0, MemKodTov = 0;
            string TimeVidachi = "", VidObslugi = "", NameTov = "", NameStatus = "";

            List<TableOrder> TableGridOrder = new List<TableOrder>(1);

            var HeadOrderAll = MainWindow.db.HeadZakazs.ToList().OrderBy(row => row.Number).OrderByDescending(row => row.Status);
            foreach (HeadZakaz pole in HeadOrderAll)   //MainWindow.db.HeadZakazs.ToList<HeadZakaz>()
            {

                if (((StatusOrder1 == pole.Status && StatusOrder2 == 0 && StatusOrder3 == 0) || (StatusOrder2 == pole.Status && StatusOrder1 == 0 && StatusOrder3 == 0) ||
                    (StatusOrder3 == pole.Status && StatusOrder1 == 0 && StatusOrder2 == 0) ||
                    ((StatusOrder2 == pole.Status || StatusOrder1 == pole.Status) && StatusOrder1 != 0 && StatusOrder2 != 0 && StatusOrder3 == 0) ||
                    ((StatusOrder3 == pole.Status || StatusOrder1 == pole.Status) && StatusOrder1 != 0 && StatusOrder3 != 0 && StatusOrder2 == 0) ||
                    ((StatusOrder2 == pole.Status || StatusOrder3 == pole.Status) && StatusOrder2 != 0 && StatusOrder3 != 0 && StatusOrder1 == 0) ||
                    ((StatusOrder2 == pole.Status || StatusOrder3 == pole.Status || StatusOrder1 == pole.Status) && StatusOrder2 != 0 && StatusOrder3 != 0 && StatusOrder1 != 0) ||
                    (StatusOrder2 == 0 && StatusOrder3 == 0 && StatusOrder1 == 0)) && pole.Status != 4)
                {
                    Service = pole.PlaceService;
                    MemStatus = pole.Status;
                    MemNumber = pole.Number;


                    if (MainWindow.db.SprVidGiveOuts.Where(s => s.KodGiveOut == Service).FirstOrDefault() != null)
                    {
                        var StrGiveOut = MainWindow.db.SprVidGiveOuts.Where(s => s.KodGiveOut == Service);
                        foreach (SprVidGiveOut strzap in StrGiveOut) { VidObslugi = "      "+strzap.NameGiveOutUk; }
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
            ClientListOrder Cook_Link = MainWindow.LinkMainWindow("ClientListOrder");
            Cook_Link.TableOrders.ItemsSource = TableGridOrder;

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
            while (0 == 0)
            {
                Thread.Sleep(5000);
                ApplicationContext dbUpdate = new ApplicationContext();
                MainWindow.MemCount = dbUpdate.HeadZakazs.Count();
                var ListAdmin = MainWindow.db.AdminSetValues.Where(p => p.Id == 1).FirstOrDefault();
                 
                if (MainWindow.CountHeadZakaz != MainWindow.MemCount || MainWindow.M_StatusOrder != ListAdmin.StatusOrder)
                {
                    MainWindow.M_StatusOrder = ListAdmin.StatusOrder;
                    MainWindow.CountHeadZakaz = MainWindow.MemCount;
                    LoadTableOrder();
                }
            }
            
        }
    }
}
