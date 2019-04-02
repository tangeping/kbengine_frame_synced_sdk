using KBEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : FrameSyncBehaviour
{
    private KBEngine.Grid gridMap;

    //private AStarPathfinder pathfinder = null;

    private FPVector nodePostion = FPVector.zero;

    private Vector2Int[] _path = null;

    private FPVector startPosition = FPVector.zero;
    private FPVector endPosition = FPVector.zero;
    private Vector2Int startNode = Vector2Int.zero;
    private Vector2Int endNode = Vector2Int.zero;
   
    private FP speed = 8.0f;
    private int stepIndex = 0;
    private FPVector velocity = FPVector.zero;

    public FPVector NodePostion { get { return nodePostion; } }
    public FPVector Velocity { get { return velocity; } }
    public FP Speed { get { return speed; } set { speed = FPMath.Max(1.0f, value); } }
    public Vector2Int[] path { get { return _path; } set { _path = value; } }

    public KBEngine.Grid GridMap { get { if (gridMap == null) gridMap = AStarPath.GridGraph; return gridMap; } }

    void ClearPath()
    {
        path = null;
        stepIndex = 1;
        velocity = FPVector.zero;
        startPosition = FPVector.zero;
        endPosition = FPVector.zero;
        startNode = Vector2Int.zero;
        endNode = Vector2Int.zero;
    }

    public bool InSameNode(FPVector p1,FPVector p2)
    {
        /*Debug.LogFormat("p1:{0},p2:{1}", GridMap.getIndex(p1), GridMap.getIndex(p2));*/
        return GridMap.getIndex(p1) == GridMap.getIndex(p2);
    }

    FPVector GetOnGroundPoint(FPVector point)
    {
        FPVector result = point;
        RaycastHit floorHit;
        int layer = 1 << LayerMask.NameToLayer("ground");
        if (Physics.Raycast(new Vector3(result.x.AsFloat(), 100, result.z.AsFloat()), -Vector3.up,
            out floorHit, Mathf.Infinity, layer))
        {
            result.y = floorHit.point.y;
        }
        return result;
    }

    public void goToDestination(FPVector destPositon)
    {
        if (FPVector.Distance(FPTransform.position, destPositon) < Speed * FrameSyncManager.DeltaTime)
        {
            FPTransform.position = destPositon;
        }
        else
        {
            var v = (destPositon - FPTransform.position).normalized * Speed * FrameSyncManager.DeltaTime;
            FPTransform.LookAt(new FPVector(destPositon.x, FPTransform.position.y, destPositon.z));
            FPTransform.Translate(v, Space.World);
        }
    }

    // Use this for initialization
    void Start () {
        FPTransform.position = GetOnGroundPoint(FPTransform.position);
    }

    public void SetDestination(FPVector dest,int type = 0)
    {
        ClearPath();//清除标记
        dest = GetOnGroundPoint(dest);
        startPosition = FPTransform.position;
        endPosition = dest;
        startNode = GridMap.getIndex(startPosition);
        endNode = GridMap.getIndex(endPosition);

        Debug.Log("startNode:" + startNode + ",endNode:" + endNode);
        if(type == 0)// if you use Roy-TAstar navigate,you should call GetPath(start,end) function. 
        {
            path = GridMap.GetPath(startNode, endNode);
        }
        else if(type == 1)    //if you use Justinhj navigate , you should call GetPath(start,end,new AstarPathfinder(gridMap,0) function.
        {
            path = GridMap.GetPath(startNode, endNode, AStarPath.PathFinder);
            Debug.Log("path.length:" + path.Length);
        }
    }

    private void MoveStep()
    {
        if (!(path != null && path.Length > 0 && stepIndex < path.Length))
        {
            return;
        }
        nodePostion = GridMap.GetCenterPoint(path[stepIndex].x, path[stepIndex].y);
        nodePostion = GetOnGroundPoint(nodePostion);
        if(FPVector.Distance(FPTransform.position, nodePostion) < Speed* FrameSyncManager.DeltaTime)
        {
            FPTransform.position = nodePostion;
            if(stepIndex < path.Length)
            {                  
                if(stepIndex == path.Length - 1)
                {
                    velocity = FPVector.zero;
                }
                stepIndex++;
            }
        }
        else
        {
            velocity = (nodePostion - FPTransform.position).normalized * Speed * FrameSyncManager.DeltaTime;
            FPTransform.LookAt(new FPVector(nodePostion.x, FPTransform.position.y, nodePostion.z));
            FPTransform.Translate(velocity, Space.World);
        }
//         Debug.LogWarningFormat("{0} FPTransform.position:{1},endNode:{2},stepIndex:{3},Length:{4},node:{5},position:{6}",
//             name, GridMap.getIndex(FPTransform.position), endNode, stepIndex, path.Length, path[stepIndex],FPTransform.position);
    }

    public void StopStep()
    {
        ClearPath();
    }

    public bool isCompleted()
    {
        return startNode==endNode || GridMap.getIndex(FPTransform.position) == endNode;      
    }

    public override void OnSyncedUpdate()
    {
        MoveStep();
    }



    void onDrawPath()
    {
        if (path!= null && path.Length > 0)
        {
            Gizmos.color = Color.green;
            FPVector currPostion = GridMap.GetCenterPoint(path[0].x, path[0].y);
            for (int i = stepIndex; i < path.Length; i++)
            {
                FPVector nextPosition = GridMap.GetCenterPoint(path[i].x, path[i].y);
                nextPosition = GetOnGroundPoint(nextPosition);
                Gizmos.DrawLine(currPostion.ToVector(), nextPosition.ToVector());
                currPostion = nextPosition;
            }
        }
    }
    private void OnDrawGizmos()
    {
        onDrawPath();
    }
}
