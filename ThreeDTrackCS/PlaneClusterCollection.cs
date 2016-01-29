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

        public int ClusterCount
        {
            get
            {
                return allClusters.Count;
            }
        }

        public int PlaneCount
        {
            get
            {
                return allPlanes.Count;
            }
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
                        foreach ( Plane p in allClusters[possibleClusters[i]] )
                        {
                            allClusters[possibleClusters[0]].AddPlane( p );
                        }
                        allClusters.Remove( possibleClusters[i] );
                    }
                    break;
            }

        }

        internal void RemoveSmallClusters( int minimumCellCount)
        {
            HashSet<int> removingClusters = new HashSet<int>();

            foreach ( KeyValuePair<int, PlaneCluster> kvp in allClusters )
            {
                if ( kvp.Value.Count < minimumCellCount )
                {
                    removingClusters.Add( kvp.Key );
                }
            }

            foreach ( int clusterIndex in removingClusters )
            {
                foreach ( Plane plane in allClusters[clusterIndex] )
                {
                    allPlanes.Remove( plane.Id );
                }
                allClusters.Remove( clusterIndex );
            }

        }
    }
}
