namespace ClosestPairSweep {

    public enum StepSize {
        Sweep,
        EventPointSchedule,
        LineContent,
    }

    public class ClosestPair {
        
        private readonly PriorityQueue<double, (Func<PointD> Func, string Desc)> prv_EventPointSchedule = new();
        private readonly HashSet<PointD> prv_LineContent = new();

        public StepSize StepSize;
        public StatusData Status = new();



        public IEnumerable<PointD> LineContent => prv_LineContent.AsEnumerable();
        public double SweepLinePos { get; private set; }

        public ClosestPair(IEnumerable<PointD> aPoints, StepSize aStepSize = StepSize.Sweep) {
            prv_EventPointSchedule.AddRange(aPoints.Select(x => (x.X, InsertAction(x))));
            StepSize = aStepSize;
        }

        private (Func<PointD>, string) InsertAction(PointD aPoint) {
            PointD InsertFunc() {
                prv_LineContent.Add(aPoint);
                return aPoint;
            }
            return (InsertFunc, $"Add {aPoint} to SweepStatusStructure");
        }

        private (Func<PointD>, string) DeleteAction(PointD aPoint) {
            PointD DeleteFunc() {
                if (prv_LineContent.Contains(aPoint))
                    prv_LineContent.Remove(aPoint);
                return aPoint;
            };
            return (DeleteFunc, $"Remove {aPoint} from SweepStatusStructure");
        }

        public async Task<(PointD, PointD)> Sweep(Func<Task>? aWaitFunc = null) {
            if (prv_EventPointSchedule.Count < 2)
                throw new InvalidOperationException("At least two points are required for a closest pair sweep!");

            (Func<PointD> Func, string Desc) lcl_Pop = GetEvent();
            PointD lcl_P1 = lcl_Pop.Func.Invoke();
            if (StepSize >= StepSize.LineContent) {
                Status = new StatusData() { P1 = lcl_P1, Info = lcl_Pop.Desc };
                await RunWait(aWaitFunc);
            }
            lcl_Pop = GetEvent();
            PointD lcl_P2 = lcl_Pop.Func.Invoke();
            if (StepSize >= StepSize.LineContent) {
                Status = new StatusData() { P1 = lcl_P2, Info = lcl_Pop.Desc };
                await RunWait(aWaitFunc);
            }
            double lcl_d = Distance(lcl_P1, lcl_P2);
            if (StepSize >= StepSize.LineContent) {
                Status = new StatusData() { P1 = lcl_P1, P2 = lcl_P2, D = lcl_d, Info = $"Initial distance betweem {lcl_P1} and {lcl_P2} is {lcl_d:f2}." };
                await RunWait(aWaitFunc);
            }
            prv_EventPointSchedule.Push(lcl_P1.X + lcl_d, DeleteAction(lcl_P1));
            prv_EventPointSchedule.Push(lcl_P2.X + lcl_d, DeleteAction(lcl_P2));
            while (!prv_EventPointSchedule.Empty) {
                (Func<PointD> Func, string Desc) lcl_Event = GetEvent();
                PointD lcl_Current = lcl_Event.Func.Invoke();
                if (StepSize >= StepSize.EventPointSchedule) {
                    Status = new StatusData() { P1 = lcl_Current, D = lcl_d, Info = lcl_Event.Desc };
                    await RunWait(aWaitFunc);
                }
                if (prv_LineContent.Contains(lcl_Current)) {
                    List<PointD> lcl_ToRemove = new();
                    // InsertAction
                    foreach (var lcl_Point in prv_LineContent) {

                        if (lcl_Current == lcl_Point)
                            continue;
                        if (lcl_Point.X + lcl_d < lcl_Current.X) {
                            if (StepSize >= StepSize.LineContent) {
                                Status = new StatusData() { P1 = lcl_Point, D = lcl_d, Info = $"Removing {lcl_Point} early as the min distance has changed and it is now outside of it." };
                                await RunWait(aWaitFunc);
                            }
                            lcl_ToRemove.Add(lcl_Point);
                            continue;
                        }

                        double lcl_newD = Distance(lcl_Current, lcl_Point);
                        if (StepSize >= StepSize.LineContent) {
                            Status = new StatusData() { P1 = lcl_Current, P2 = lcl_Point, D = lcl_d, Info = $"Distance between {lcl_Current} and {lcl_Point} is {lcl_newD:f2}. Old distance is {lcl_d:f2}." };
                            await RunWait(aWaitFunc);
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

            static async Task RunWait(Func<Task>? aWaitFunc) {
                if (aWaitFunc is not null)
                    await aWaitFunc();
            }
        }

        private (Func<PointD> Func, string Desc) GetEvent() {
            SweepLinePos = prv_EventPointSchedule.MinPriority;
            return prv_EventPointSchedule.Pop();
        }

        private static double Distance(PointD aP1, PointD aP2) {
            return Math.Sqrt(Math.Pow(aP1.X - aP2.X, 2) + Math.Pow(aP1.Y - aP2.Y, 2));
        }
    }
}
