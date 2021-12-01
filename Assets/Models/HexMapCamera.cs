using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
	// swiwel отвечает за угол камеры, stick зв расстояние
	Transform swivel, stick;

	float zoom = 1f;

	// скорость вращения камеры
	public float rotationSpeed;

	float rotationAngle;

	// минимальное и максимальное масштабирование
	public float stickMinZoom, stickMaxZoom;

	// минимальный и максимальный угол наклона камеры
	public float swivelMinZoom, swivelMaxZoom;

	// скорости движения при минимальном и максимальном зуме
	public float moveSpeedMinZoom, moveSpeedMaxZoom;

	// границы карты
	public HexGrid grid;

	void Awake()
	{
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);
	}

	void Update()
	{
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDelta != 0f)
		{
			AdjustZoom(zoomDelta);
		}

		float rotationDelta = Input.GetAxis("Rotation");
		if (rotationDelta != 0f)
		{
			AdjustRotation(rotationDelta);
		}

		float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");
		if (xDelta != 0f || zDelta != 0f)
		{
			AdjustPosition(xDelta, zDelta);
		}
	}

	/// <summary>
	/// Метод манипуляции наклона и приближения камеры
	/// </summary>
	/// <param name="delta"></param>
	void AdjustZoom(float delta)
	{
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);

		float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}

	/// <summary>
	/// Метод манипуляции положением камеры
	/// </summary>
	/// <param name="xDelta"></param>
	/// <param name="zDelta"></param>
	void AdjustPosition(float xDelta, float zDelta)
	{
		Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) 
			* damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = position;

		transform.localPosition = ClampPosition(position);
	}

	/// <summary>
	/// Метод поворота камеры
	/// </summary>
	/// <param name="delta"></param>
	void AdjustRotation(float delta)
	{
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
		if (rotationAngle < 0f)
		{
			rotationAngle += 360f;
		}
		else if (rotationAngle >= 360f)
		{
			rotationAngle -= 360f;
		}
	}

	/// <summary>
	/// Чтобы не выйти за пределы карты
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	Vector3 ClampPosition(Vector3 position)
	{
		float xMax =
			(grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f) *
			(2f * HexMetrics.innerRadius);
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax =
			(grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) *
			(1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		return position;
	}
}
