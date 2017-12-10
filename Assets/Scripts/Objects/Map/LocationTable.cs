using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
#endif

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Location Table", order = 3, fileName = "Location Table")]
	public class LocationTable : ScriptableObject
	{
		[SerializeField]
		private string clustersPath = "";

		[System.Serializable]
		public class Cluster
		{
			public Cluster(string name, List<Place> places)
			{
				this.m_name = name;
				this.m_places = places;
			}

			[SerializeField]
			private string m_name = "Group";

			[SerializeField]
			private List<Place> m_places = new List<Place>();

			public string name
			{
				get { return m_name; }
			}

			public List<Location> GetLocations()
			{
				if(m_places == null || m_places.Count == 0)
					return null;

				List<Location> table = new List<Location>();

				foreach(Place place in m_places)
				{
					table.Add(place as Location);

					if(place.hasRooms && place.rooms != null && place.rooms.Length > 0)
					{
						foreach(Room room in place.rooms)
							table.Add(room as Location);
					}
				}

				return table;
			}
		}

		[SerializeField]
		private List<Cluster> clusters = new List<Cluster>();

		public List<Location> GetTable()
		{
			if(clusters == null || clusters.Count == 0)
				return null;
			
			List<Location> table = new List<Location>();

			foreach(Cluster cluster in clusters)
			{
				List<Location> clusterList = cluster.GetLocations();

				if(clusterList != null)
					table.AddRange(clusterList);
			}

			return table;
		}
	
		public void SetClusters(List<Cluster> clusters)
		{
			this.clusters = clusters;
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(LocationTable))]
	public class LocationTableEditor : Editor
	{
		private SerializedProperty
		clustersPathProperty = null,
		clustersProperty = null;

		private LocationTable table = null;

		private void OnEnable()
		{
			table = target as LocationTable;
			clustersPathProperty = serializedObject.FindProperty("clustersPath");
			clustersProperty = serializedObject.FindProperty("clusters");
		}

		public override void OnInspectorGUI()
		{
			if(GUILayout.Button("Find clusters from path"))
				FindClustersFromPath(clustersPathProperty.stringValue);
			
			DrawDefaultInspector();
		}

		private void FindClustersFromPath(string path)
		{
			char separator = '/';
			string fullApplicationPath = Application.dataPath + separator + path;

			if(!Directory.Exists(fullApplicationPath))
				return;
			
			string[] directories = Directory.GetDirectories(fullApplicationPath);

			clustersProperty.ClearArray();
			clustersProperty.arraySize = (directories != null && directories.Length > 0 ? directories.Length : 0);

			if(clustersProperty.arraySize == 0)
			{
				Debug.Log("Failed to load clusters.");
				return;
			}

			List<LocationTable.Cluster> clusters = new List<LocationTable.Cluster>();

			for(int i = 0; i < directories.Length; i++)
			{
				string directory = directories[i];
				string[] assets = Directory.GetFiles(directory, "*.asset");

				List<Place> places = new List<Place>();
				foreach(string asset in assets)
				{
					string assetPath = "Assets/" + asset.Replace(Application.dataPath + "/", "").Replace("\\", "/");
					Place place = AssetDatabase.LoadAssetAtPath<Place>(assetPath);
					places.Add(place);
				}

				string clusterName = directory.Split(Path.DirectorySeparatorChar).Last();
				LocationTable.Cluster cluster = new LocationTable.Cluster(clusterName, places);
				clusters.Add(cluster);
			}

			table.SetClusters(clusters);
		}
	}
	#endif
}