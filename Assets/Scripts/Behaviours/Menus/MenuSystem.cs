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
		private MenuLayout layout = null;
		private MenuNode rootNode = null;
		private bool isOpen = false;
		private List<int> contextList = new List<int>();
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
		}

		private void OnEnable()
		{
			RegisterEvent();
		}

		private void OnDisable()
		{
			DeregisterEvent();
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			rootNode = MenuNode.CreateNode(menuStructure.GetItems());
			layout = GetComponent<MenuLayout>();
		}

		private void RegisterEvent()
		{
			MenuLayout.OnContentClick += OnContentClick;
			MenuLayout.OnPrimaryClick += OnPrimaryClick;
//			MenuLayout.OnSecondaryClick += OnSecondaryClick;
		}

		private void DeregisterEvent()
		{
			MenuLayout.OnContentClick -= OnContentClick;
			MenuLayout.OnPrimaryClick -= OnPrimaryClick;
//			MenuLayout.OnSecondaryClick -= OnSecondaryClick;
		}

		private void Open()
		{
			MenuNode currentNode = GetCurrentNode();
			MenuNode[] currentNodes = currentNode.GetAll();
			List<MenuContent> content = new List<MenuContent>();

			if(currentNodes != null)
			{
				foreach(MenuNode node in currentNodes)
					content.Add(node.ToContent());
			}

			string label = (currentNode == rootNode ? "" : currentNode.label);
			layout.SetContent(content.ToArray(), label);
		}

		private void Clear()
		{
			contextList.Clear();
			layout.ClearContent();
			layout.ClearText();
		}

		private MenuNode GetCurrentNode()
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
				return currentNode;
		}
		#endregion


		#region Events
		private void OnPrimaryClick()
		{
			if(contextList.Count > 0)
				isOpen = true;
			else
				isOpen = !isOpen;
			
			if(!isOpen)
				Clear();
			else
			{
				if(contextList.Count > 0)
					contextList.RemoveAt(contextList.Count - 1);
				Open();
			}

			layout.ShowBackground(isOpen);
		}

		private void OnSecondaryClick()
		{
			
		}

		private void OnContentClick(int index)
		{
			contextList.Add(index);
			Open();
		}


		#endregion
	}
}