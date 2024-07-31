using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetTerrainObstacles : MonoBehaviour
{
    public Terrain targetTerrain;
    TreeInstance[] Obstacle;
    float width;
    float length;
    float height;
    bool isError;

    void Start()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("Target Terrain is not assigned!");
            return;
        }

        Obstacle = targetTerrain.terrainData.treeInstances;
        if (Obstacle.Length == 0)
        {
            Debug.LogWarning("No trees found on the target terrain.");
            return;
        }

        length = targetTerrain.terrainData.size.z;
        width = targetTerrain.terrainData.size.x;
        height = targetTerrain.terrainData.size.y;
        Debug.Log("Terrain Size is: " + width + " , " + height + " , " + length);

        int i = 0;
        GameObject parent = new GameObject("Tree_Obstacles");

        Debug.Log("Adding " + Obstacle.Length + " NavMeshObstacle Components for Trees");

        foreach (TreeInstance tree in Obstacle)
        {
            Vector3 tempPos = Vector3.Scale(tree.position, new Vector3(width, height, length)) + targetTerrain.GetPosition();
            Quaternion tempRot = Quaternion.AngleAxis(tree.rotation * Mathf.Rad2Deg, Vector3.up);

            Debug.Log("Tree Position: " + tempPos);
            Debug.Log("Tree Rotation: " + tempRot);

            GameObject obs = new GameObject("Obstacle" + i);
            obs.transform.SetParent(parent.transform);
            obs.transform.position = tempPos;
            obs.transform.rotation = tempRot;

            obs.AddComponent<NavMeshObstacle>();
            NavMeshObstacle obsElement = obs.GetComponent<NavMeshObstacle>();
            obsElement.carving = true;
            obsElement.carveOnlyStationary = true;

            GameObject treePrefab = targetTerrain.terrainData.treePrototypes[tree.prototypeIndex].prefab;

            if (treePrefab.GetComponent<Collider>() == null)
            {
                isError = true;
                Debug.LogError("ERROR: There is no CapsuleCollider or BoxCollider attached to '" + treePrefab.name + "'. Please add one of them.");
                break;
            }

            Collider coll = treePrefab.GetComponent<Collider>();
            if (coll is CapsuleCollider capsuleColl)
            {
                obsElement.shape = NavMeshObstacleShape.Capsule;
                obsElement.center = capsuleColl.center;
                obsElement.radius = capsuleColl.radius;
                obsElement.height = capsuleColl.height;
            }
            else if (coll is BoxCollider boxColl)
            {
                obsElement.shape = NavMeshObstacleShape.Box;
                obsElement.center = boxColl.center;
                obsElement.size = boxColl.size;
            }
            else
            {
                isError = true;
                Debug.LogError("ERROR: There is no CapsuleCollider or BoxCollider attached to '" + treePrefab.name + "'. Please add one of them.");
                break;
            }

            i++;
        }

        parent.transform.position = targetTerrain.GetPosition();
        if (!isError) Debug.Log("All " + Obstacle.Length + " NavMeshObstacles were successfully added to your Scene, Hooray!");
    }
}
