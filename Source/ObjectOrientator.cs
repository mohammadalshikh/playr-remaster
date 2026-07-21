using UnityEngine;

public class ObjectOrientator : MonoBehaviour
{
	[SerializeField]
	private Vector3 screenPos;

	[SerializeField]
	private new Camera camera;

	private void Awake()
	{
		OrientateObject();
	}

	private void OnEnable()
	{
		CameraScrollController.CameraAnimCompleted += HandleCameraAnimCompleted;
		IntroView.IntroSceneCompleted += HandleCameraAnimCompleted;
		MapController.CamOrthgraphicSizeChanged += HandleCameraAnimCompleted;
		GameTypeMenuManager.CamOrthgraphicSizeChanged += HandleCameraAnimCompleted;
	}

	private void OnDisable()
	{
		CameraScrollController.CameraAnimCompleted -= HandleCameraAnimCompleted;
		IntroView.IntroSceneCompleted -= HandleCameraAnimCompleted;
		MapController.CamOrthgraphicSizeChanged -= HandleCameraAnimCompleted;
		GameTypeMenuManager.CamOrthgraphicSizeChanged -= HandleCameraAnimCompleted;
	}

	private void HandleCameraAnimCompleted()
	{
		OrientateObject();
	}

	private void OrientateObject()
	{
		if ((bool)camera)
		{
			base.transform.position = camera.ViewportToWorldPoint(screenPos);
		}
		else
		{
			base.transform.position = Camera.mainCamera.ViewportToWorldPoint(screenPos);
		}
	}
}
