using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private float respawnYValue = -30;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;  
    }

    
    private void Update()
    {
        if (transform.position.y <= respawnYValue)
            transform.position = startPos;
    }
}
