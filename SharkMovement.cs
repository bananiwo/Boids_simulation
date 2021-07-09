using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkMovement : BasicMovement
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    override protected void OnTriggerEnter2D(Collider2D other)
   {
       base.OnTriggerEnter2D(other);
   }

    override protected void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

}
