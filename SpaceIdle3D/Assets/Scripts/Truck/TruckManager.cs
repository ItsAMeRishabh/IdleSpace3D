using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckManager : MonoBehaviour
{
	[ SerializeField ] private GameObject truckPrefab;
	[ SerializeField ] private Transform truckRouteParent;
	[ SerializeField ] private Vector2 truckSpawnDelay;
	[ SerializeField ] private double truckSpawn_MinimumIPS = 50;
	public float truckMoveSpeed = 5;
	public float truckRotationSpeed = 5;
	[ HideInInspector ] public List<Transform> truckRoute;
	[ HideInInspector ] public GameObject iridiumTruck;
	private Coroutine truckCoroutine;

	private void OnIPSUpdated() { }
}