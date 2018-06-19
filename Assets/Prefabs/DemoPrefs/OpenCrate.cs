using System.Collections;
using System.Collections.Generic;
using Jedium.Behaviours;
using Jedium.LocalBehaviours;
using JediumCore;
using UnityEngine;

public class OpenCrate : JediumLocalBehaviour
{
    private JediumAnimatorBehaviour _crate;

    private bool open = false;
	// Use this for initialization
    public override void Init(ClientGameObject jgo)
    {
        base.Init(jgo);
        _crate = GetComponent<JediumAnimatorBehaviour>();
    }



    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            open = !open;
            _crate.SetBool("OpenCrate",open);
        }
    }
}
