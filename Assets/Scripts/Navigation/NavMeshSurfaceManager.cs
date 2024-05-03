using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System.Threading.Tasks;

/// <summary>
/// NavMeshSurfaceManager handles updates that the game might make to the map, and updates the surface as needed
/// </summary>
[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshSurfaceManager : MonoBehaviour
{
    static bool s_NeedsNavMeshUpdate;
    NavMeshSurface m_Surface;

    public static void RequestNavMeshUpdate() //can be changed to an RPC for networking, probably
    {
        s_NeedsNavMeshUpdate = true;
    }

    void Start()
    {
        m_Surface = GetComponent<NavMeshSurface>();
        //m_Surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (s_NeedsNavMeshUpdate)
        {
            m_Surface.UpdateNavMesh(m_Surface.navMeshData);
            s_NeedsNavMeshUpdate = false;
        }
    }

    public async Task UpdateNavMeshSurface()
    {
        m_Surface.UpdateNavMesh(m_Surface.navMeshData);
        await Task.Yield();
    }
}
