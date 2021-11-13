using System.Diagnostics.CodeAnalysis;

namespace ClosestPairSweep {

    public struct PointD :IEquatable<PointD> {
        public double X;    
        public double Y;

        public PointD(double aX, double aY) {
            X = aX;
            Y = aY;
        }

        override public string ToString() => $"{X}:{Y}";

        override public int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public override bool Equals([NotNullWhen(true)] object? aObj) {
            if( base.Equals(aObj))
                return true;

            if (aObj is PointD lcl_Point) 
                return Equals(lcl_Point);
            
            return false;
        }

        public bool Equals(PointD aOther) => aOther.X == X && aOther.Y == Y;

        public static bool operator ==(PointD aA, PointD aB) => aA.Equals(aB);
        public static bool operator !=(PointD aA, PointD aB) => !aA.Equals(aB);
    }

    public class StatusData {
        public PointD? P1;
        public PointD? P2;
        public double D;
        public string? Info;
    }

}
