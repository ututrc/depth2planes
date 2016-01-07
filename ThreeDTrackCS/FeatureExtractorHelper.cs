using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    static class FeatureExtractorHelper
    {
        internal static bool DepthDataValid( IntPtr depthDataPointer, DepthDataFormat format, int topLeftIndex, int topRightIndex, int bottomLeftIndex, int bottomRightIndex )
        {
            switch ( format )
            {
                case DepthDataFormat.UInt16mm:
                    return Marshal.PtrToStructure<ushort>(depthDataPointer + topLeftIndex * 2) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + topRightIndex * 2 ) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + bottomLeftIndex * 2 ) > 0
                        && Marshal.PtrToStructure<ushort>( depthDataPointer + bottomRightIndex * 2 ) > 0;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static bool IsPlane( IntPtr pointCloudPointer, PointCloudFormat format, int topLeftIndex, int topRightIndex, int bottomLeftIndex, int bottomRightIndex, double epsilon, out Vector3d planePoint, out Vector3d planeNormal )
        {


            switch ( format )
            {
                case PointCloudFormat.FloatVector:
                    {
                        Vector3f topLeft = Marshal.PtrToStructure<Vector3f>(pointCloudPointer + topLeftIndex * Vector3f.Size);
                        Vector3f topRight = Marshal.PtrToStructure<Vector3f>( pointCloudPointer + topRightIndex * Vector3f.Size );
                        Vector3f bottomLeft = Marshal.PtrToStructure<Vector3f>( pointCloudPointer + bottomLeftIndex * Vector3f.Size );
                        Vector3f bottomRight = Marshal.PtrToStructure<Vector3f>( pointCloudPointer + bottomRightIndex * Vector3f.Size );

                        Vector3f dir1 = topRight - topLeft;
                        Vector3f dir2 = bottomLeft - topLeft;

                        Vector3f norm = Vector3f.Cross( dir1, dir2 );
                        norm.Normalize();

                        planePoint = topLeft;
                        planeNormal = norm;

                        float A = norm.X;
                        float B = norm.Y;
                        float C = norm.Z;
                        float D = -A * topLeft.X - B * topLeft.Y - C * topLeft.Z;

                        return Math.Abs( A * bottomRight.X + B * bottomRight.Y + C * bottomRight.Z + D )  <= epsilon;

                    }
                case PointCloudFormat.DoubleVector:
                    {
                        Vector3d topLeft = Marshal.PtrToStructure<Vector3d>( pointCloudPointer + topLeftIndex * Vector3d.Size );
                        Vector3d topRight = Marshal.PtrToStructure<Vector3d>( pointCloudPointer + topRightIndex * Vector3d.Size );
                        Vector3d bottomLeft = Marshal.PtrToStructure<Vector3d>( pointCloudPointer + bottomLeftIndex * Vector3d.Size );
                        Vector3d bottomRight = Marshal.PtrToStructure<Vector3d>( pointCloudPointer + bottomRightIndex * Vector3d.Size );

                        Vector3d dir1 = topRight - topLeft;
                        Vector3d dir2 = bottomLeft - topLeft;

                        Vector3d norm = Vector3d.Cross( dir1, dir2 );
                        norm.Normalize();

                        planePoint = topLeft;
                        planeNormal = norm;

                        double A = norm.X;
                        double B = norm.Y;
                        double C = norm.Z;
                        double D = -A * topLeft.X - B * topLeft.Y - C * topLeft.Z;

                        return Math.Abs( A * bottomRight.X + B * bottomRight.Y + C * bottomRight.Z + D ) <= epsilon;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        internal static Plane CreatePlane( Vector3d point, Vector3d normal, int id )
        {

            return new Plane( point, normal, id );
        }
    }
}
