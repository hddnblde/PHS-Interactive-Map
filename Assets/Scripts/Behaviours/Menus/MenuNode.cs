using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus.DataStructure
{
	public class MenuNode
	{
		public static MenuNode CreateNode(MenuItem[] items)
		{
			if(items == null)
				return null;

			MenuNode rootNode = new MenuNode("Root", false);
			int previousDepth = 0;

			for(int i = 0; i < items.Length; i++)
			{
				MenuItem item = items[i];
				MenuNode node = new MenuNode(item.title, item.toggleable);

				int difference = item.depth - previousDepth;

				if(item.depth == 0)
					rootNode.Add(node);
				else
				{
					MenuNode lastNode = GetLastNode(rootNode, item.depth);
					lastNode.Add(node);
				}
			}

			return rootNode;
		}

		private static MenuNode GetLastNode(MenuNode rootNode, int depth)
		{
			MenuNode currentNode = rootNode;

			for(int j = 0; j < depth; j++)
			{
				MenuNode node = currentNode.GetLast();

				if(node != null)
					currentNode = node;
			}

			return currentNode;
		}

		private string m_label = "";

		private bool m_toggleable = false;

		private List<MenuNode> m_nodes = new List<MenuNode>();

		public MenuNode(string label, bool toggleable)
		{
			m_label = label;
			m_toggleable = toggleable;
		}

		public string label
		{
			get { return m_label; }
		}

		public bool toggleable
		{
			get { return m_toggleable; }
		}

		public int nodeCount
		{
			get { return m_nodes.Count; }
		}

		private void Add(MenuNode node)
		{
			m_nodes.Add(node);
		}

		public MenuNode Get(int index)
		{
			if(index < 0 || index >= m_nodes.Count)
				return null;
			else
				return m_nodes[index];
		}

		public MenuNode GetLast()
		{
			if(m_nodes.Count == 0)
				return null;
			else
				return m_nodes[m_nodes.Count - 1];
		}

		public MenuNode[] GetAll()
		{
			return m_nodes.ToArray();
		}
	}
}