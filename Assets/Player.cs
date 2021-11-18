using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public Vector2 gridPos      = new Vector2(0, 0);
    public Vector2 nextGridPos  = new Vector2(0, 0);


    public int      sizeMax = 16;
    public int      sizeMin = 6;
    public float    size;
    public float    sizeStepDown;
    public float    sizeStepDownTime;
    public float    sizeGrowTime;

    private int sizeDiff;

    public  UnityEvent  move_event;
    public  Sprite      sprite;

    void Start()
    {
        size            = sizeMax;
        sizeDiff        = Mathf.Abs(sizeMax - sizeMin);
        sizeStepDown    = (float)sizeDiff / (float)sizeMax;

        move_event      = new UnityEvent();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            nextGridPos.y++;
            move_event.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            nextGridPos.y--;
            move_event.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            nextGridPos.x--;
            move_event.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            nextGridPos.x++;
            move_event.Invoke();
        }

        if (size <= sizeMin)
        {
            size = sizeMin;
            return;
        }

        float newSizeScale = Mathf.Lerp(
                            transform.localScale.x,
                            (float)size / (float)sizeMax,
                            sizeStepDownTime * Time.deltaTime
                        ); 
        transform.localScale = new Vector3(newSizeScale, newSizeScale, 1);
    }
}
