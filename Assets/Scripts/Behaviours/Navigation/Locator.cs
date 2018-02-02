using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Navigation
{
	public class Locator : MonoBehaviour
	{
		#region Serialized Field
		[SerializeField]
		private Text textField = null;
		#endregion


		#region Hidden Fields
		private static float m_longitude = 0f;
		private static float m_latitude = 0f;
		private Coroutine serviceRoutine = null;
		#endregion


		#region Properties
		public static float longitude
		{
			get { return m_longitude; }
		}

		public static float latitude
		{
			get { return m_latitude; }
		}
		#endregion


		#region MonoBehaviour Implementation
		private void OnEnable()
		{
			StartServiceRoutine();
		}
		#endregion

		
		#region Methods
		private void StartServiceRoutine()
		{
			if(serviceRoutine != null)
				StopCoroutine(serviceRoutine);
			
			serviceRoutine = StartCoroutine(ServiceRoutine());
		}

		private IEnumerator ServiceRoutine()
		{
			LocationService locationService = Input.location;
			WaitForSeconds initializationDelay = new WaitForSeconds(30);

			if(!locationService.isEnabledByUser)
			{
				SetTextWarning("Turn on your gps");
            	yield break;
			}

			locationService.Start();
			SetTextWarning("Starting...");

			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
			{
				yield return new WaitForSeconds(1);
				maxWait--;
			}

			if (maxWait < 1)
			{
				SetTextWarning("Timed out.");
				yield break;
			}

			if(Input.location.status == LocationServiceStatus.Failed)
			{
				SetTextWarning("Unable to determine device location.");
				yield break;
			}
			else
			{
				SetTextWarning("Updating...");
				WaitForSeconds updateDelay = new WaitForSeconds(3f);
				while(Input.location.status == LocationServiceStatus.Running)
				{
					m_longitude = locationService.lastData.longitude;
					m_latitude = locationService.lastData.latitude;
					SetText();
					yield return updateDelay;
				}
			}

			Input.location.Stop();
			SetTextWarning("Stopped.");
		}

		private void SetText()
		{
			if(textField == null)
				return;

			string pattern = "Longitude: @long\nLatitude: @lat".Replace("@long", longitude.ToString("F3")).Replace("@lat", latitude.ToString("F3"));
			textField.text = pattern;
		}

		private void SetTextWarning(string message)
		{
			if(textField == null)
				return;

			textField.text = message;
		}
		#endregion
	}
}