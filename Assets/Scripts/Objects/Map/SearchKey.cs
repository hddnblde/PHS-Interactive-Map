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

		public SearchKey(int primaryIndex, int secondaryIndex, int tertiaryIndex)
		{
			Constructor(primaryIndex, secondaryIndex, tertiaryIndex, 0, 0);
		}

		public SearchKey(int mainIndex, int subIndex, int tertiaryIndex, int strength)
		{
			Constructor(mainIndex, subIndex, tertiaryIndex, strength, 0);
		}

		public SearchKey(int mainIndex, int subIndex, int tertiaryIndex, int strength, int nearestPoint)
		{
			Constructor(mainIndex, subIndex, tertiaryIndex, strength, nearestPoint);
		}

		private void Constructor(int primaryIndex, int secondaryIndex, int tertiaryIndex, int strength, int nearestPoint)
		{
			m_primaryIndex = primaryIndex;
			m_secondaryIndex = secondaryIndex;
			m_tertiaryIndex = tertiaryIndex;
			m_strength = strength;
			m_nearestPoint = nearestPoint;
		}

		private int m_primaryIndex = -1;
		private int m_secondaryIndex = -1;
		private int m_tertiaryIndex = -1;
		private int m_strength = 0;
		private int m_nearestPoint = -1;

		public int primaryIndex
		{
			get { return m_primaryIndex; }
		}

		public int secondaryIndex
		{
			get { return m_secondaryIndex; }
		}

		public int tertiaryIndex
		{
			get { return m_tertiaryIndex; }
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