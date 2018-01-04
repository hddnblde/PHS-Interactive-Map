using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Menus.DataStructure;

namespace Menus
{
	public class MenuSystem : MonoBehaviour
	{
		#region Serialized Field
		[SerializeField]
		private MenuStructure menuStructure = null;
		#endregion


		#region Hidden Fields
		private List<int> contextList = new List<int>();

		private MenuNode rootNode = null;
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
			PrintCurrentContext();
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			rootNode = MenuNode.CreateNode(menuStructure.GetItems());
		}

		private void PrintCurrentContext()
		{
			MenuNode[] currentItems = GetCurrentContextItems();

			if(currentItems == null)
			{
				Debug.Log("EMPTY!");
				return;
			}

			string items = "CURRENT CONTEXT:";
			foreach(MenuNode node in currentItems)
			{
				items += '\n' + node.label;
			}

			Debug.Log(items);
		}

		private MenuNode[] GetCurrentContextItems()
		{
			MenuNode currentNode = rootNode;

			foreach(int context in contextList)
			{
				MenuNode node = currentNode.Get(context);
				currentNode = node;

				if(node == null)
					break;
			}

			if(currentNode == null)
				return null;
			else
				return currentNode.GetAll();
		}
		#endregion
	}
}