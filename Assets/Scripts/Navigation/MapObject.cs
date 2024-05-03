using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System.Threading.Tasks;

/// <summary>
/// This component will be attached to objects that are added or removed from the game scene, in order to dynamically add pathfinding routes for units.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshModifier))]
public class MapObject : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    NavMeshModifier m_NavMeshModifier;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_NavMeshModifier = GetComponent<NavMeshModifier>();
        m_NavMeshModifier.enabled = true;
        NavMeshSurfaceManager.RequestNavMeshUpdate();
    }

    // Update is called once per frame
    void OnDestroy()
    {
        NavMeshSurfaceManager.RequestNavMeshUpdate();
        Destroy(gameObject);
    }
}
