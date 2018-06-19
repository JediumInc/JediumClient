using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jedium.Assets;
using UnityEngine;

public class AsyncTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{

       // Thread t=new Thread((() =>
       // {
       //     AsyncClass cl = new AsyncClass();
       // }));
       //t.Start();
        BundleAssetLoader loader=new BundleAssetLoader();

	    GameObject t = loader.GetWebAssetSync<GameObject>("TestBundle", Guid.Parse("3601dced-3ddc-46da-87e0-e00aab388e54"));

	    Instantiate(t);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}


public class AsyncClass
{
    public AsyncClass()
    {
        SomeAsyncMethod();
    }

    async void SomeAsyncMethod()
    {
        await new WaitForSeconds(1);
        await new WaitForBackgroundThread();
        //ERROR - background thread
       // GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);
       // g.transform.position=new Vector3(0,1,0);
        Debug.Log("__SIMPLE ASYNC");
        await TaskAsync();
    }

    async Task TaskAsync()
    {
        await new WaitForSeconds(10);

        await new WaitForEndOfFrame();
        //OK - on main thread now
         GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);
         g.transform.position=new Vector3(0,1,0);
        Debug.Log("__TASK ASYNC");
    }
}
