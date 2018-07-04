using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Commands;

public class ViewModel
{

    private NoesisView view;


    public ViewModel(NoesisView view)
    {
        if(view!=null)
        {
            this.view = view;
        }

    }

    public void Subscribe (object model) 
    {
        view.Content.DataContext = model;
    }

}



