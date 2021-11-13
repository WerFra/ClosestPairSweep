using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ClosestPairSweep.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public const int CNST_Diameter = 5;

        public MainWindow() {
            Sweep = new ClosestPair(prv_Points);
            InitializeComponent();
            cmb_StepSize.ItemsSource = Enum.GetValues(typeof(StepSize)).Cast<StepSize>();
            cmb_StepSize.SelectedItem = StepSize.LineContent;
            itc_SweepStatusStructure.ItemsSource = SweepStatusStructure;
            this.Title = "Closest Pair Sweep";
        }

        public ClosestPair Sweep { get; set; }
        private readonly List<PointD> prv_Points = new();
        private readonly List<Shape> prv_Shapes = new();
        public readonly ObservableCollection<PointD> SweepStatusStructure = new();

        private void Canvas_MouseLeftButtonDown(object aSender, MouseButtonEventArgs aE) {
            Ellipse lcl_Ellipse = new() {
                Fill = Brushes.Black,
                Width = CNST_Diameter,
                Height = CNST_Diameter,
                StrokeThickness = 2
            };

            cnv_Canvas.Children.Add(lcl_Ellipse);

            var lcl_ClickPos = aE.GetPosition(cnv_Canvas);

            Canvas.SetLeft(lcl_Ellipse, lcl_ClickPos.X - CNST_Diameter / 2);
            Canvas.SetTop(lcl_Ellipse, lcl_ClickPos.Y - CNST_Diameter / 2);

            prv_Points.Add(new PointD(lcl_ClickPos.X, lcl_ClickPos.Y));

            aE.Handled = true;
        }

        private void Clear_Click(object aSender, RoutedEventArgs aE) {
            prv_Points.Clear();
            cnv_Canvas.Children.Clear();
        }

        private async void Calculate_Click(object aSender, RoutedEventArgs aE) {
            ClearHighlights();
            btn_Calculate.IsEnabled = false;
            btn_Continue.IsEnabled = true;

            Sweep = new ClosestPair(prv_Points, (StepSize)cmb_StepSize.SelectedItem);

            async Task WaitFunc() {
                UpdateGUI();
                await btn_Continue;
            }

            try {
                var (lcl_P1, lcl_P2) = await Sweep.Sweep(WaitFunc);
                Line lcl_Line = new ();
                lcl_Line.X1 = lcl_P1.X;
                lcl_Line.Y1 = lcl_P1.Y;
                lcl_Line.X2 = lcl_P2.X;
                lcl_Line.Y2 = lcl_P2.Y;
                lcl_Line.StrokeThickness = 2;
                lcl_Line.Stroke = Brushes.DarkGreen;
                cnv_Canvas.Children.Add(lcl_Line);
                UpdateGUI();
                prv_Shapes.Add(lcl_Line);
            } catch (InvalidOperationException lcl_Exception) {
                MessageBox.Show(lcl_Exception.Message);
            } finally {
                btn_Calculate.IsEnabled = true;
                btn_Continue.IsEnabled = false;
            }

        }

        private void ClearHighlights() {
            if (prv_Shapes.Count != 0) {
                foreach (Shape lcl_Shape in prv_Shapes) {
                    cnv_Canvas.Children.Remove(lcl_Shape);
                }
                prv_Shapes.Clear();
            }
        }

        public void UpdateGUI() {
            ClearHighlights();
            tb_LastMsg.Text = Sweep.Status.Info;
            SweepStatusStructure.Clear();
            foreach (PointD lcl_Point in Sweep.LineContent) {
                SweepStatusStructure.Add(lcl_Point);
            }

            if (SweepStatusStructure.Count > 0) {
                double lcl_MinX = Sweep.SweepLinePos - Sweep.Status.D;
                Line lcl_Left = new ();
                lcl_Left.X1 = lcl_MinX;
                lcl_Left.Y1 = 0;
                lcl_Left.X2 = lcl_MinX;
                lcl_Left.Y2 = cnv_Canvas.ActualHeight;
                lcl_Left.StrokeThickness = 2;
                lcl_Left.Stroke = Brushes.LightGreen;
                cnv_Canvas.Children.Add(lcl_Left);
                prv_Shapes.Add(lcl_Left);
                double lcl_MaxX = Sweep.SweepLinePos;
                Line lcl_Right = new ();
                lcl_Right.X1 = lcl_MaxX;
                lcl_Right.Y1 = 0;
                lcl_Right.X2 = lcl_MaxX;
                lcl_Right.Y2 = cnv_Canvas.ActualHeight;
                lcl_Right.StrokeThickness = 2;
                lcl_Right.Stroke = Brushes.LightCoral;
                cnv_Canvas.Children.Add(lcl_Right);
                prv_Shapes.Add(lcl_Right);
            }

            if (Sweep.Status.P1.HasValue && Sweep.Status.P2.HasValue) {
                Line lcl_Line = new ();
                lcl_Line.X1 = Sweep.Status.P1.Value.X;
                lcl_Line.Y1 = Sweep.Status.P1.Value.Y;
                lcl_Line.X2 = Sweep.Status.P2.Value.X;
                lcl_Line.Y2 = Sweep.Status.P2.Value.Y;
                lcl_Line.StrokeThickness = 2;
                lcl_Line.Stroke = Brushes.LightBlue;
                cnv_Canvas.Children.Add(lcl_Line);
                prv_Shapes.Add(lcl_Line);
            }
            if (Sweep.Status.P1.HasValue) {
                Ellipse lcl_Ellipse = new();
                lcl_Ellipse.Fill = Brushes.LightBlue;
                lcl_Ellipse.Width = CNST_Diameter;
                lcl_Ellipse.Height = CNST_Diameter;
                lcl_Ellipse.StrokeThickness = 2;
                cnv_Canvas.Children.Add(lcl_Ellipse);
                Canvas.SetLeft(lcl_Ellipse, Sweep.Status.P1.Value.X - CNST_Diameter / 2);
                Canvas.SetTop(lcl_Ellipse, Sweep.Status.P1.Value.Y - CNST_Diameter / 2);
                prv_Shapes.Add(lcl_Ellipse);
            }
            if (Sweep.Status.P2.HasValue) {
                Ellipse lcl_Ellipse = new ();
                lcl_Ellipse.Fill = Brushes.LightBlue;
                lcl_Ellipse.Width = CNST_Diameter;
                lcl_Ellipse.Height = CNST_Diameter;
                lcl_Ellipse.StrokeThickness = 2;
                cnv_Canvas.Children.Add(lcl_Ellipse);
                Canvas.SetLeft(lcl_Ellipse, Sweep.Status.P2.Value.X - CNST_Diameter / 2);
                Canvas.SetTop(lcl_Ellipse, Sweep.Status.P2.Value.Y - CNST_Diameter / 2);
                prv_Shapes.Add(lcl_Ellipse);
            }
        }

        private void StepSize_SelectionChanged(object aSender, RoutedEventArgs aE) {
            Sweep.StepSize = (StepSize)cmb_StepSize.SelectedItem;
            aE.Handled = true;
        }
    }
}
