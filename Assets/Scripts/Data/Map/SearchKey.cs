namespace Search
{
	public enum SearchCategory
	{
		Name,
		MainTag,
		SubTag
	}

	[System.Serializable]
	public class SearchKey
	{
		public SearchKey()
		{
			Constructor(0, 0, 0, 0, 0);
		}

		public SearchKey(int landmarkIndex, int placeIndex, int locationIndex)
		{
			Constructor(landmarkIndex, placeIndex, locationIndex, 0, 0);
		}

		public SearchKey(int landmarkIndex, int placeIndex, int locationIndex, int strength)
		{
			Constructor(landmarkIndex, placeIndex, locationIndex, strength, 0);
		}

		public SearchKey(int landmarkIndex, int placeIndex, int locationIndex, int strength, int nearestPoint)
		{
			Constructor(landmarkIndex, placeIndex, locationIndex, strength, nearestPoint);
		}

		private void Constructor(int landmarkIndex, int placeIndex, int locationIndex, int strength, int nearestPoint)
		{
			m_landmarkIndex = landmarkIndex;
			m_placeIndex = placeIndex;
			m_locationIndex = locationIndex;
			m_strength = strength;
			m_nearestPoint = nearestPoint;
		}

		private int m_landmarkIndex = -1;
		private int m_placeIndex = -1;
		private int m_locationIndex = -1;
		private int m_strength = 0;
		private int m_nearestPoint = -1;

		public int landmarkIndex
		{
			get { return m_landmarkIndex; }
		}

		public int placeIndex
		{
			get { return m_placeIndex; }
		}

		public int locationIndex
		{
			get { return m_locationIndex; }
		}

		public int strength
		{
			get { return m_strength; }
			set { m_strength = value; }
		}

		public int nearestPoint
		{
			get { return m_nearestPoint; }
			set { m_nearestPoint = value; }
		}
	}
}