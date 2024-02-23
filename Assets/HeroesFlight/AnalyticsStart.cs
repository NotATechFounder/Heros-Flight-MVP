using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsStart : MonoBehaviour
{
	async void Start()
	{
		await UnityServices.InitializeAsync();
		AnalyticsService.Instance.StartDataCollection();
	}
}
