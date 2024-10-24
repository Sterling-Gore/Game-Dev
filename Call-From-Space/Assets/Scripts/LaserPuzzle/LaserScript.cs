using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField]
    private Transform startPoint;
    public bool on = false;
    LaserScript previousLaserReflector;
    public bool isEndPoint = false;
    public LayerMask ignoreCollider;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        previousLaserReflector = null;
        ignoreCollider = ~ignoreCollider;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEndPoint)
        {
            lr.SetPosition(0, startPoint.position);
            RaycastHit hit;
            if(on)
            {
                if(Physics.Raycast(transform.position, - transform.right, out hit, 500, ignoreCollider))
                {
                    if (hit.collider)
                    {
                        lr.SetPosition(1, hit.point);
                        if( hit.collider.GetComponent<LaserScript>())
                        {
                            if(previousLaserReflector && previousLaserReflector != hit.collider.GetComponent<LaserScript>())
                            {
                                checkPreviousReflector();
                            }
                            previousLaserReflector = hit.collider.GetComponent<LaserScript>();
                            previousLaserReflector.on = true;
                        }
                        else
                        {
                            checkPreviousReflector();
                        }
                    }
                    
                }
                else
                {
                    checkPreviousReflector();
                    lr.SetPosition(1, -transform.right *500);
                }
            }
            else
            {
                checkPreviousReflector();
                lr.SetPosition(1, startPoint.position);
            } 
        }
        
        
    }

    void checkPreviousReflector()
    {
        if(previousLaserReflector)
        {
            previousLaserReflector.on = false;
            previousLaserReflector = null;
        }
    }



}
