using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    public class PlaneClusterCollection : IEnumerable<PlaneCluster>
    {
        private List<int> possibleClusters;
        private Dictionary<int, Plane> allPlanes;
        private Dictionary<int, PlaneCluster> allClusters;
        internal double PointEpsilon;
        internal double RequiredHitPercetage;
        internal double NormalDirectionEpsilon;
        internal FeatureExtractor parent;

        internal PlaneClusterCollection( FeatureExtractor extractor )
        {
            parent = extractor;
            possibleClusters = new List<int>();
            allPlanes = new Dictionary<int, Plane>();
            allClusters = new Dictionary<int, PlaneCluster>();
        }

        public IEnumerator<PlaneCluster> GetEnumerator()
        {
            return ((IEnumerable<PlaneCluster>)allClusters.Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return allClusters.Values.GetEnumerator();
        }

        internal void Clear()
        {
            allPlanes.Clear();
            allClusters.Clear();
        }

        internal void AddPlane( Plane plane )
        {
            allPlanes.Add( plane.Id, plane );
            possibleClusters.Clear();

            foreach ( PlaneCluster cluster in allClusters.Values )
            {
                if ( cluster.IsMatch( plane, NormalDirectionEpsilon, PointEpsilon, RequiredHitPercetage ) )
                {
                    possibleClusters.Add( cluster.id );
                }
            }

            switch ( possibleClusters.Count )
            {
                case 0:
                    PlaneCluster nCluster = new PlaneCluster( this );
                    nCluster.AddPlane( plane );
                    allClusters.Add( nCluster.id, nCluster );
                    break;
                case 1:
                    allClusters[possibleClusters[0]].AddPlane( plane );
                    break;
                default:
                    for ( int i = possibleClusters.Count - 1; i > 0; i-- )
                    {
                        foreach ( Plane p in allClusters[i] )
                        {
                            allClusters[0].AddPlane( p );
                        }
                        allClusters.Remove( possibleClusters[i] );
                    }
                    break;
            }

        }

    }
}
