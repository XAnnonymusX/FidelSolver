using System;
using System.Collections.Generic;
using System.IO;
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

namespace FidelSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dictionary<string, ImageBrush> items = new Dictionary<string, ImageBrush>();
        private static FrameworkElement[,] gridUIElements;
        private FidelGrid grid;
        private int selectedx = 0, selectedy = 0;

        public MainWindow()
        {
            InitializeComponent();
            CONST.INITIALIZESPRITES();
            
            grid = new FidelGrid();

            Canvas canvas = (Canvas)FindName("gridCanvas");
            RenderTargetBitmap bitmap = new RenderTargetBitmap(369, 350, CONST.DPI, CONST.DPI, PixelFormats.Pbgra32);
            BitmapImage bitmapspider = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "../../../imgs/spider.png"));
            CroppedBitmap bitmapspidercropped = new CroppedBitmap(bitmapspider, new Int32Rect(55, 0, 55, 47));
            CroppedBitmap bitmapfidelcropped = new CroppedBitmap((BitmapImage)Resources["fidel_base_image"], new Int32Rect(49, 0, 49, 47));
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                //drawingContext.DrawImage(bitmapfidelcropped, new Rect(0, 0, 49, 47));
                //drawingContext.DrawImage((CroppedBitmap)Resources["fidel_base_idle_1_bmp"], new Rect(0, 47, 49, 47));
                //drawingContext.DrawImage(bitmapspidercropped, new Rect(55, 0, 55, 47));
                drawingUtils.drawAt(drawingContext, (BitmapSource) Resources["bg_rock_base_image"], 0, 0);
                for(int i = 0; i < 7; i++)
                {
                    for(int j = 0; j < 7; j++)
                    {
                        drawingUtils.drawCenteredAt(drawingContext, (BitmapSource) Resources["fidel_base_idle_1_bmp"], CONST.MARGINLEFT + i * CONST.TILEWIDTH + CONST.TILEWIDTH/2, CONST.MARGINTOP + j * CONST.TILEHEIGHT + CONST.TILEHEIGHT/2);
                    }
                }
            }
            bitmap.Render(drawingVisual);
            Image img = new Image();
            img.Source = bitmap;
            img.Height = bitmap.PixelHeight;
            img.Width = bitmap.PixelWidth;
            canvas.Children.Add(img);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Note: this fires when the selection is changed by means other than a click (i.e. not by the user) as well.
        /// </summary>
        private void list_click(object sender, SelectionChangedEventArgs e)
        {
            //TODO: associate an Item to every list item and set item to that
            if (e.RemovedItems.Count > 0)
            {
                return;
            }
            //string name = (string)((ListBoxItem)e.AddedItems[0]).Content;
            //grid[selectedx, selectedy] = name;
        }

        private void grid_Click(object sender, MouseButtonEventArgs e)
        {
            //TODO: delete this V and find out over which tile the cursor is (if any)
            string[] rowcol = ((FrameworkElement)sender).Name.Substring(5, 3).Split('_');
            selectedx = Int32.Parse(rowcol[0]);
            selectedy = Int32.Parse(rowcol[1]);
            ((ListBox)FindName("listBox")).UnselectAll();
        }
    }

    public class FidelGrid
    {
        private Tile[,] grid;
        private byte width;
        private byte height;
        public FidelGrid(byte width = 7, byte height = 7)
        {
            grid = new Tile[width, height];
            this.width = width;
            this.height = height;
        }

        public Item this[byte x, byte y] {
            get {
                if(x > width || y > height)
                {
                    return Item.None;
                }
                return grid[x, y].item;
            }
            set {
                if (x > width || y > height)
                {
                    return;
                }
                grid[x, y].item = value;
            }
        }
    }

    public class Tile
    {
        public Item item {
            get { return item; }
            set {
                item = value;
                //TODO: assign correct sprite
            }
        }
        public BitmapSource sprite { get; private set; }
        //TODO: may have to change this (again) to accommodate shadows, moving eyes etc. at the very least should allow for a "isn't in a category" type
        public VEffect effects { get; private set; }

        public Tile(Item item = Item.None)
        {
            this.item = item;
            effects = null;
            //TODO: assign correct sprite
        }

        private void addEffect(VEffectType type)
        {
            if (effects == null)
            {
                effects = new VEffect(type);
            }
            else
            {
                VEffect cur = effects;
                while (cur.next != null)
                {
                    cur = cur.next;
                }
                cur.next = new VEffect(type);
            }
        }

        private void removeEffect(VEffectType type)
        {
            if (effects == null)
            {
                return;
            }
            if(effects.type == type)
            {
                effects = effects.next;
                return;
            }
            VEffect cur = effects;
            while (cur.next != null)
            {
                if (cur.next.type == type)
                {
                    cur.next = cur.next.next;
                    break;
                }
                cur = cur.next;
            }
        }

        //TODO: add animate() to go to next frame of animation
    }

    public class VEffect
    {
        public BitmapSource effect { get;}
        public VEffectType type { get;}
        public VEffect next;

        public VEffect(VEffectType type)
        {
            effect = makeVEffect(type);
            this.type = type;
            next = null;
        }

        private static BitmapSource makeVEffect(VEffectType type)
        {
            return new BitmapImage();
            //TODO: select a random effect of given type (maybe have another class which groups the bitmaps by type?)
        }
    }

    public static class drawingUtils
    {
        public static void drawAt(DrawingContext context, BitmapSource source, int x, int y)
        {
            /*if(source.DpiX != CONST.DPI || source.DpiY != CONST.DPI)
            {
                throw new Exception("Image DPI not consistent (" + source.DpiX + "/" + source.DpiY + " instead of " + CONST.DPI + ")");
            }*/
            context.DrawImage(source, new Rect(x, y, source.PixelWidth, source.PixelHeight));
        }
        public static void drawCenteredAt(DrawingContext context, BitmapSource source, int x, int y)
        {
            drawAt(context, source, x-source.PixelWidth/2, y-source.PixelHeight/2);
        }

        public static int gridToPixelCoordsX(byte x)
        {
            return CONST.MARGINLEFT + x * CONST.TILEWIDTH + CONST.TILEWIDTH / 2;
        }
        public static int gridToPixelCoordsY(byte y)
        {
            return CONST.MARGINTOP + y * CONST.TILEHEIGHT + CONST.TILEHEIGHT / 2;
        }
        //maybe make that a byte and return 255 if oob
        public static byte? pixelToGridCoordsX(int x)
        {
            if(x < CONST.MARGINLEFT || (x - CONST.MARGINLEFT) / CONST.TILEWIDTH > 255)
            {
                return null;
            }
            return (byte) ((x - CONST.MARGINLEFT) / CONST.TILEWIDTH);
        }
        public static byte? pixelToGridCoordsy(int y)
        {
            if (y < CONST.MARGINTOP || (y - CONST.MARGINTOP) / CONST.TILEHEIGHT > 255)
            {
                return null;
            }
            return (byte) ((y - CONST.MARGINTOP) / CONST.TILEHEIGHT);
        }
    }

    public static class CONST
    {
        public const int DPI = 96;
        public const int MARGINLEFT = 13;
        public const int MARGINTOP = 18;
        public const int TILEWIDTH = 49;
        public const int TILEHEIGHT = 45;

        public static Dictionary<Item, BitmapSource> ITEMSPRITES;

        public static void INITIALIZESPRITES(findByName findName)
        {
            ITEMSPRITES.Add(Item.None, new BitmapImage());
            ITEMSPRITES.Add(Item.Fidel, (BitmapSource) findName("fidel_base_idle_1"));
            ITEMSPRITES.Add(Item.Spider, (BitmapSource)findName("spider_base_idle_1"));
        }

        public delegate object findByName(string name);
    }

    public enum Item{
        None,
        Fidel,
        Spider,
        Medkit
    }

    public enum VEffectType
    {
        Splatter,
        Scorch
    }
}

//TODO: change sprites every frame to produce animation
//TODO: set initial health & xp
//TODO: pathfinding algo
//TODO: delete selected item on del keypress
//TODO: add indicator of selected item

//TODO: flip stairs-straignt and stairs-uturn sprites for use with the other-sided stairs



//TODO: allow partial progression



/* grid data structure is a 7x7 (or 9x7) array. Each slot has a list, containing every object on that tile (in order?)
 * when grid is red
 * 
 */