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
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        previousLaserReflector = null;
        
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
                if(Physics.Raycast(transform.position, - transform.right, out hit))
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
                    lr.SetPosition(1, -transform.right *5000);
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
