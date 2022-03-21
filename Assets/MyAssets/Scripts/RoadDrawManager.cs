using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using UnityEngine.AI;

public class RoadDrawManager : MonoBehaviour
{
	public Camera cam = null;
	public LineRenderer lineRenderer = null;
	private Vector3 mousePos;
	private Vector3 Pos, newPos;
	private Vector3 previousPos;
	private List<Vector3> linePositions = new List<Vector3>();
	public float minimumDistance = 0.1f;
	private float distance = 0;



	private Vector3[] lineRendererPose;
	public PathCreator _pathCreator;
	public PathPlacer _PathPlacer;
	private GameObject _pathCreatorGameObject;

    GameObject road;
    bool canDraw = false;

    public Vector3 worldPosition;



    private GameObject player;
	//private float zpos;
    
	public GameObject pathCreatorPrefab;

	public float waitTime;
	private float timeStamp = Mathf.Infinity;

	private void Start()
	{
        road = GameObject.FindGameObjectWithTag("Road");
        player = GameObject.FindGameObjectWithTag("Player");
	}


    void Update()
	{

        if (Input.GetMouseButtonDown(0))
			{
            
            linePositions.Clear();



            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null && hit.transform.tag == "Road")
                {
                    canDraw = true;
                    Pos = hit.point;
                }
                else
                {
                    linePositions.Clear();
                }
            }



            newPos = new Vector3(Pos.x, Pos.y+0.1f, Pos.z);
				previousPos = Pos;
				linePositions.Add(newPos);
			}

			else if (Input.GetMouseButton(0))
			{
            /*
			mousePos = Input.mousePosition;
            mousePos.z = mousePosZ(mousePos.y);
            //mousePos.z = player.transform.position.z;
            Pos = cam.ScreenToWorldPoint(mousePos);*/
            //Scores.handCount -= Time.deltaTime;

            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null && hit.transform.tag == "Road" && canDraw == true)
                {
                    Pos = hit.point;
                    newPos = new Vector3(Pos.x, Pos.y + 0.1f, Pos.z);
                    distance = Vector3.Distance(Pos, previousPos);
                    if (distance >= minimumDistance )
                    {
                        previousPos = Pos;
                        linePositions.Add(newPos);
                        lineRenderer.positionCount = linePositions.Count;
                        lineRenderer.SetPositions(linePositions.ToArray());
                    }
                }
            }
                
			}
			else if (Input.GetMouseButtonUp(0))
			{
				lineRendererPose = new Vector3[lineRenderer.positionCount];
				lineRenderer.GetPositions(lineRendererPose);

			    if(lineRendererPose.Length > 0)
				MakeSpline();
                canDraw = false;
			}
		
	}

	public void MakeSpline()
	{
		lineRenderer.positionCount = 0;
		_pathCreatorGameObject = Instantiate(pathCreatorPrefab, transform.position, transform.rotation);



        _pathCreator = _pathCreatorGameObject.GetComponentInChildren<PathCreator>();
		_PathPlacer = _pathCreatorGameObject.GetComponentInChildren<PathPlacer>();

		_pathCreator.bezierPath = new BezierPath(lineRendererPose);
		_pathCreator.bezierPath.AutoControlLength = 0.01f;
		_pathCreator.bezierPath.Space = PathSpace.xyz;

		_PathPlacer.Generate();

        //Destroy(roadMeshHolder.GetComponent<MeshCollider>());
        //roadMeshHolder.AddComponent<MeshCollider>(); 
    }

    
    float mousePosZ(float z)
    {
        if (z < 15)
        {
            z = z * 2.5f;
        }
        else if (z < 25)
        {
            z = z * 2;
        }
        else
        {
            z = z / 90;
        }
        return z;
    }
}
