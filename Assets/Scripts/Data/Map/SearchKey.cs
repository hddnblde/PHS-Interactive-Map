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

		public SearchKey(int poiIndex, int placeIndex, int locationIndex)
		{
			Constructor(poiIndex, placeIndex, locationIndex, 0, 0);
		}

		public SearchKey(int poiIndex, int placeIndex, int locationIndex, int strength)
		{
			Constructor(poiIndex, placeIndex, locationIndex, strength, 0);
		}

		public SearchKey(int poiIndex, int placeIndex, int locationIndex, int strength, int nearestPoint)
		{
			Constructor(poiIndex, placeIndex, locationIndex, strength, nearestPoint);
		}

		private void Constructor(int poiIndex, int placeIndex, int locationIndex, int strength, int nearestPoint)
		{
			m_poiIndex = poiIndex;
			m_placeIndex = placeIndex;
			m_locationIndex = locationIndex;
			m_strength = strength;
			m_nearestPoint = nearestPoint;
		}

		private int m_poiIndex = -1;
		private int m_placeIndex = -1;
		private int m_locationIndex = -1;
		private int m_strength = 0;
		private int m_nearestPoint = -1;

		public int poiIndex
		{
			get { return m_poiIndex; }
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