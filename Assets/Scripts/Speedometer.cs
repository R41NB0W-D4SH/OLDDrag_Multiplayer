using UnityEngine;
using System.Collections;

public class Speedometer : MonoBehaviour
{

	public float _start; // ��������� ��������� ������� �� ��� Z

	public float maxSpeed; // ������������ �������� �� ����������

	public RectTransform arrow; // ������� ����������

	public enum ProjectMode { Project3D = 0, Project2D = 1 };
	public ProjectMode projectMode = ProjectMode.Project3D;

	public Transform target; // ������ � �������� ����� ��������

	public float velocity; // ������� �������� �������� �������

	private Rigidbody _3D;
	private Rigidbody2D _2D;
	private float speed;

	void Start()
	{
		arrow.localRotation = Quaternion.Euler(0, 0, _start);
		if (projectMode == ProjectMode.Project3D) _3D = target.GetComponent<Rigidbody>();
		else _2D = target.GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (projectMode == ProjectMode.Project3D) velocity = _3D.velocity.magnitude; else velocity = _2D.velocity.magnitude;
		if (velocity > maxSpeed) velocity = maxSpeed;
		speed = _start - velocity;
		arrow.localRotation = Quaternion.Euler(0, 0, speed);
	}
}