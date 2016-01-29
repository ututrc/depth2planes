using System;
using System.Collections;
using System.Collections.Generic;

namespace ThreeDTrackCS
{
    public class PlaneCluster : IEnumerable<Plane>
    {

        private Dictionary<int, Plane> planes;
        private static int clusterIdGenerator;
        internal int id;
        private PlaneClusterCollection parent;

        internal PlaneCluster( PlaneClusterCollection collection )
        {
            parent = collection;
            planes = new Dictionary<int, Plane>();
            id = clusterIdGenerator++;
        }

        public IEnumerator<Plane> GetEnumerator()
        {
            return ( (IEnumerable<Plane>)planes.Values ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return planes.Values.GetEnumerator();
        }

        /// <summary>
        /// Checks if a plane fits into the cluster
        /// </summary>
        /// <param name="plane">The plane in question</param>
        /// <param name="normalDirectionEpsilon">An acceptable error for the direction</param>
        /// <param name="pointEpsilon">An acceptable error for the position</param>
        /// <param name="requiredHitPercetage">How many planes must match all conditions (NOTE: includes the rule matching)</param>
        /// <returns></returns>
        internal bool IsMatch( Plane plane, double normalDirectionEpsilon, double pointEpsilon, double requiredHitPercetage = .001 )
        {

            if ( planes.Count == 0 )
                return true;

            double hits = 0;

            foreach ( Plane p in planes.Values )
            {
                if ( (parent.parent.PlaneClusterizationRule == null || parent.parent.PlaneClusterizationRule.Match(parent.parent.GridDivision.Horizontal, plane, p ) ) &&  Vector3d.IsSimilar( p.Normal, plane.Normal, normalDirectionEpsilon, false ) && p.ContainsPoint(plane.Position, pointEpsilon) )
                {
                    hits++;
                }
            }

            hits /= planes.Count;

            return hits >= requiredHitPercetage;

        }

        internal bool IsMatch( Plane plane, object normalDirectionEpsilon, double pointEpsilon, object requiredHitPercetage )
        {
            throw new NotImplementedException();
        }

        internal void AddPlane( Plane plane )
        {
            planes.Add( plane.Id, plane );
        }

        internal void RemovePlane( Plane plane )
        {
            planes.Remove( plane.Id );
        }

        internal bool ContainsPlane( Plane plane )
        {
            return planes.ContainsKey( plane.Id );
        }

        public int Count
        {
            get
            {
                return planes.Count;
            }
        }

        public Vector3d AverageNormal
        {
            get
            {
                double x = 0, y = 0, z = 0;
                int count = 0;
                foreach ( Plane plane in planes.Values )
                {
                    x += plane.Normal.X;
                    y += plane.Normal.Y;
                    z += plane.Normal.Z;
                    count++;
                }

                Vector3d result = new Vector3d( x, y, z );

                if ( count > 0 )
                    result /= count;
                return result;

            }
        }

        public Vector3d AveragePoint
        {
            get
            {
                if ( planes.Count == 0 )
                    return new Vector3d();
                double x = 0, y = 0, z = 0;
                foreach ( Plane plane in planes.Values )
                {
                    x += plane.Position.X;
                    y += plane.Position.Y;
                    z += plane.Position.Z;
                }

                return new Vector3d( x, y, z ) / planes.Count;
            }
        }
    }
}