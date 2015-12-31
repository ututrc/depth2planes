using System;

namespace ThreeDTrackCS
{
    public abstract class PlaneClusterizationRule
    {

        private static EmptyPlaneClusterizationRule emptyRule = new EmptyPlaneClusterizationRule();
        private static NonDiagonalNextPlaneClusterizationRule nonDiagonalRule = new NonDiagonalNextPlaneClusterizationRule();
        private static NextPlaneClusterizationRule nextRule = new NextPlaneClusterizationRule();

        internal abstract bool Match( int width, Plane first, Plane second );

        public static PlaneClusterizationRule CreateNoLimitRule()
        {
            return emptyRule;
        }

        public static PlaneClusterizationRule CreateNonDiagonalNextRule()
        {
            return nonDiagonalRule;
        }

        public static PlaneClusterizationRule CreateNextRule()
        {
            return nextRule;
        }

        public static PlaneClusterizationRule CreateQuadraticDistanceRule( int distance )
        {
            return new QuadraticDistancePlaneClusterizationRule( distance );
        }

        public static PlaneClusterizationRule CreateCircularDistanceRule( double distance )
        {
            return new CircularDistancePlaneClusterizationRule( distance );
        }

    }

    internal class CircularDistancePlaneClusterizationRule : PlaneClusterizationRule
    {
        private double squaredDistance;

        public CircularDistancePlaneClusterizationRule( double distance )
        {
            squaredDistance = distance * distance;
        }

        internal override bool Match( int width, Plane first, Plane second )
        {
            int fx = first.Id % width;
            int fy = first.Id / width;
            int sx = second.Id % width;
            int sy = second.Id / width;
            int xd = fx - sx;
            int yd = fy - sy;

            return xd * xd + yd * yd <= squaredDistance;
        }
    }

    internal class QuadraticDistancePlaneClusterizationRule : PlaneClusterizationRule
    {
        private int distance;

        public QuadraticDistancePlaneClusterizationRule( int distance )
        {
            this.distance = distance > 0 ? distance : 1;

        }

        internal override bool Match( int width, Plane first, Plane second )
        {
            int fx = first.Id % width;
            int fy = first.Id / width;
            int sx = second.Id % width;
            int sy = second.Id / width;
            int xd = Math.Abs( fx - sx );
            int yd = Math.Abs( fy - sy );

            return xd <= distance && yd <= distance;
        }
    }

    internal class NextPlaneClusterizationRule : QuadraticDistancePlaneClusterizationRule
    {
        public NextPlaneClusterizationRule() : base( 1 )
        {
        }
    }

    internal class NonDiagonalNextPlaneClusterizationRule : PlaneClusterizationRule
    {
        internal override bool Match( int width, Plane first, Plane second )
        {
            int fx = first.Id % width;
            int fy = first.Id / width;
            int sx = second.Id % width;
            int sy = second.Id / width;

            int xd = Math.Abs( fx - sx );
            int yd = Math.Abs( fy - sy );

            return ( xd == 0 && yd == 1 ) || ( xd == 1 && yd == 0 );

        }
    }

    internal class EmptyPlaneClusterizationRule : PlaneClusterizationRule
    {
        internal override bool Match( int width, Plane first, Plane second )
        {
            return true;
        }
    }
}