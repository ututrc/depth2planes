using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    public static class FeatureExtractorHelper
    {
        internal static bool DepthDataValid( IntPtr depthDataPointer, ref DepthDataFormat format, ref int topLeftIndex, ref int topRightIndex, ref int bottomLeftIndex, ref int bottomRightIndex )
        {
            switch ( format )
            {
                case DepthDataFormat.UInt16mm:
                    return Marshal.PtrToStructure<ushort>( depthDataPointer + topLeftIndex * 2 ) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + topRightIndex * 2 ) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + bottomLeftIndex * 2 ) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + bottomRightIndex * 2 ) > 0;
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool IsPlane( Vector3d[] pointCloud, ref int topLeftIndex, ref int topRightIndex, ref int bottomLeftIndex, ref int bottomRightIndex, ref double epsilon, out Vector3d planePoint, out Vector3d planeNormal )
        {

            planePoint = pointCloud[topLeftIndex];
            planeNormal = Vector3d.Cross( pointCloud[topRightIndex] - pointCloud[topLeftIndex], pointCloud[bottomLeftIndex] - pointCloud[topLeftIndex] );
            planeNormal.Normalize();

            return Math.Abs( planeNormal.X * pointCloud[bottomRightIndex].X + planeNormal.Y * pointCloud[bottomRightIndex].Y + planeNormal.Z * pointCloud[bottomRightIndex].Z - ( planeNormal.X * planePoint.X + planeNormal.Y * planePoint.Y + planeNormal.Z * planePoint.Z ) ) <= epsilon;
        }

        public static bool IsPlane( double epsilon, out Vector3d planePoint, out Vector3d planeNormal, params Vector3d[] points )
        {
            if ( points.Length < 3 )
            {
                planePoint = planeNormal = new Vector3d();
                return false;
            }
            planePoint = points[0];
            planeNormal = Vector3d.Cross( points[1] - points[0], points[2] - points[0] );
            planeNormal.Normalize();
            if (points.Length == 3)
            {
                return true;
            }

            for ( int i = 3; i < points.Length; i++ )
            {
                if ( Math.Abs( planeNormal.X * points[i].X + planeNormal.Y * points[i].Y + planeNormal.Z * points[i].Z - ( planeNormal.X * planePoint.X + planeNormal.Y * planePoint.Y + planeNormal.Z * planePoint.Z ) ) > epsilon )
                    return false;
            }

            return true;
        }

        internal static Plane CreatePlane( ref Vector3d point, ref Vector3d normal, ref int id )
        {

            return new Plane( ref point, ref normal, ref id );
        }

        internal static bool CalculateIntersection( PlaneCluster plane1, PlaneCluster plane2, out Line3D line )
        {
            Vector3d normal1 = plane1.AverageNormal;
            Vector3d normal2 = plane2.AverageNormal;
            Vector3d point1 = plane1.AveragePoint;
            Vector3d point2 = plane2.AveragePoint;

            double d1 = normal1 * point1;
            double d2 = normal2 * point2;
            if ( Math.Abs( normal1 * normal2 - 1 ) < double.Epsilon )
            {
                line = new Line3D();
                return false;
            }

            Vector3d lineNormal = Vector3d.Cross( normal1, normal2 );

            Vector3d linePointAsVector = -Vector3d.Cross( normal1 * d2 - normal2 * d1, lineNormal ) / lineNormal.LengthSquared;

            line = new Line3D { Position = linePointAsVector, Direction = lineNormal };

            return true;
        }

        internal static Vector3d CalculateLinePlaneIntersectionPoint( Line3D line, Vector3d planeNormal )
        {
            return line.Position + line.Direction * -( planeNormal * line.Position ) / ( line.Direction * planeNormal );
        }

        internal static void FindMostDistantPoints( List<Vector2d> acceptablePoints, out Vector2d start, out Vector2d stop )
        {
            double maxDist = 0;
            int sourcePair = -1, targetPair = -1;
            double dist;
            for ( int i = 0; i < acceptablePoints.Count; i++ )
            {
                for ( int j = i + 1; j < acceptablePoints.Count; j++ )
                {
                    dist = ( acceptablePoints[i] - acceptablePoints[j] ).LengthSquared;
                    if ( dist > maxDist )
                    {
                        sourcePair = i;
                        targetPair = j;
                    }
                }
            }
            start = acceptablePoints[sourcePair];
            stop = acceptablePoints[targetPair];
        }

        internal static bool FindBestLine( List<Vector2d> acceptablePoints, FeatureImageSize size, out Vector2d start, out Vector2d stop )
        {
            const double epsilon = 5;
            List<Vector2d> uniquePoints = new List<Vector2d>();
            foreach ( Vector2d point in acceptablePoints )
            {
                if ( IsUnique( point, uniquePoints, epsilon ) && HasAcceptableValues( point, size ) )
                {
                    uniquePoints.Add( point );
                }
            }

            if ( uniquePoints.Count < 2 )
            {
                start = stop = new Vector2d();
                return false;
            }

            if ( uniquePoints.Count > 2 )
            {
                FindMostDistantPoints( uniquePoints, out start, out stop );
                return true;
            }

            start = uniquePoints[0];
            stop = uniquePoints[1];
            return true;
        }

        private static bool HasAcceptableValues( Vector2d point, FeatureImageSize size )
        {
            const double epsilon = 1;
            return point.X >= -epsilon && point.X <= size.Width + epsilon && point.Y >= -epsilon && point.Y <= size.Height + epsilon;
        }

        private static bool IsUnique( Vector2d point, List<Vector2d> uniquePoints, double epsilon )
        {
            epsilon *= epsilon;
            for ( int i = 0; i < uniquePoints.Count; i++ )
            {
                if ( epsilon >= ( uniquePoints[i] - point ).LengthSquared )
                    return false;
            }
            return true;
        }
    }
}
