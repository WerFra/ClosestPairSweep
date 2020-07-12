using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Closest_Pair_Sweep {

    public enum StepSize {
        Sweep,
        EventPointSchedule,
        LineContent,
    }

    public class ClosestPairSweep {

        private readonly PriorityQueue<double, (Func<Point> Func, string Desc)> prv_EventPointSchedule = new PriorityQueue<double, (Func<Point>, string)>();
        private readonly HashSet<Point> prv_LineContent = new HashSet<Point>();

        public StepSize StepSize;
        public StatusData Status = new StatusData();



        public IEnumerable<Point> LineContent => prv_LineContent.AsEnumerable();

        public ClosestPairSweep(IEnumerable<Point> aPrv_Points, StepSize aStepSize = StepSize.Sweep) {
            prv_EventPointSchedule.AddRange(aPrv_Points.Select(x => (x.X, InsertAction(x))));
            StepSize = aStepSize;
        }

        private (Func<Point>, string) InsertAction(Point aPoint) {
            Func<Point> lcl_Func = () => {
                prv_LineContent.Add(aPoint);
                return aPoint;
            };
            return (lcl_Func, $"Add {aPoint} to LineContent");
        }

        private (Func<Point>, string) DeleteAction(Point aPoint) {
            Func<Point> lcl_Func = () => {
                if (prv_LineContent.Contains(aPoint))
                    prv_LineContent.Remove(aPoint);
                return aPoint;
            };
            return (lcl_Func, $"Remove {aPoint} from LineContent");
        }

        public async Task<(Point, Point)> Sweep(MainWindow aWindow = null) {
            if (prv_EventPointSchedule.Count < 2)
                throw new ArgumentException();

            (Func<Point> Func, string Desc) lcl_Pop = prv_EventPointSchedule.Pop();
            Point lcl_P1 = lcl_Pop.Func.Invoke();
            if (StepSize >= StepSize.LineContent) {
                Status = new StatusData() { P1 = lcl_P1,Info = lcl_Pop.Desc };
                await MayWait(aWindow);
            }
            lcl_Pop = prv_EventPointSchedule.Pop();
            Point lcl_P2 = lcl_Pop.Func.Invoke();
            if (StepSize >= StepSize.LineContent) {
                Status = new StatusData() { P1 = lcl_P2, Info = lcl_Pop.Desc };
                await MayWait(aWindow);
            }
            double lcl_d = Distance(lcl_P1, lcl_P2);
            if (StepSize >= StepSize.LineContent) {
                Status = new StatusData() { P1 = lcl_P1, P2 = lcl_P2, Info = $"Initial distance betweem {lcl_P1} and {lcl_P2} is {lcl_d:f2}." };
                await MayWait(aWindow);
            }
            prv_EventPointSchedule.Push(lcl_P1.X + lcl_d, DeleteAction(lcl_P1));
            prv_EventPointSchedule.Push(lcl_P2.X + lcl_d, DeleteAction(lcl_P2));
            while (!prv_EventPointSchedule.Empty) {
                (Func<Point> Func, string Desc) lcl_Event = prv_EventPointSchedule.Pop();
                Point lcl_Current = lcl_Event.Func.Invoke();
                if (StepSize >= StepSize.EventPointSchedule) {
                    Status = new StatusData() { P1 = lcl_Current, Info = lcl_Event.Desc };
                    await MayWait(aWindow);
                }
                if (prv_LineContent.Contains(lcl_Current)) {
                    List<Point> lcl_ToRemove = new List<Point>();
                    // InsertAction
                    foreach (var lcl_Point in prv_LineContent) {
                        
                        if (lcl_Current == lcl_Point)
                            continue;
                        if (lcl_Point.X + lcl_d < lcl_Current.X) {
                            if (StepSize >= StepSize.LineContent) {
                                Status = new StatusData() { P1 = lcl_Point, Info = $"Removing {lcl_Point} early as the min distance has changed and it is now outside of it." };
                                await MayWait(aWindow);
                            }
                            lcl_ToRemove.Add(lcl_Point);
                            continue;
                        }

                        double lcl_newD = Distance(lcl_Current, lcl_Point);
                        if (StepSize >= StepSize.LineContent) {
                            Status = new StatusData() { P1 = lcl_Current, P2 = lcl_Point, Info = $"Distance between {lcl_Current} and {lcl_Point} is {lcl_newD:f2}. Old distance is {lcl_d:f2}." };
                            await MayWait(aWindow);
                        }
                        if (lcl_newD < lcl_d) {
                            lcl_d = lcl_newD;
                            lcl_P1 = lcl_Point;
                            lcl_P2 = lcl_Current;
                        }
                    }
                    foreach (var lcl_Remove in lcl_ToRemove) {
                        prv_LineContent.Remove(lcl_Remove);
                    }
                    prv_EventPointSchedule.Push(lcl_Current.X + lcl_d, DeleteAction(lcl_Current));
                }
            }

            Status = new StatusData() { Info = $"Found minimum distance of {lcl_d:f2}" };
            return (lcl_P1, lcl_P2);
        }

        private async Task MayWait(MainWindow aWindow) {
            if (aWindow != null)
                aWindow.UpdateGUI();
                await aWindow.btn_Continue;
        }

        private double Distance(Point aP1, Point aP2) {
            return Math.Sqrt(Math.Pow(aP1.X - aP2.X, 2) + Math.Pow(aP1.Y - aP2.Y, 2));
        }
    }
}
