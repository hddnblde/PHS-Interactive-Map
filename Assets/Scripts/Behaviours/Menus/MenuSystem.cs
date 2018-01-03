using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
	public class MenuSystem : MonoBehaviour
	{
		private Queue<int> contextQueue = new Queue<int>();

		[SerializeField]
		private MenuStructure menuStructure = null;

		private void GetCurrentContext()
		{
			int[] contextTable = contextQueue.ToArray();

		}
	}
}