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

        internal PlaneCluster()
        {
            planes = new Dictionary<int, Plane>();
            id = clusterIdGenerator++;
        }

        public IEnumerator<Plane> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        internal bool IsMatch( Plane plane, double normalDirectionEpsilon, double pointEpsilon, double requiredHitPercetage = .001 )
        {

            if ( planes.Count == 0 )
                return true;

            double hits = 0;

            foreach ( Plane p in planes.Values )
            {
                if ( Vector3d.IsSimilar( p.Normal, plane.Normal, normalDirectionEpsilon ) && p.ContainsPoint(plane.Position, pointEpsilon) )
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

    }
}