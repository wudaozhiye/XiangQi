using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardContext
{
    public int SelectedIndex = -1;
    public Action<int> OnCellClicked;
}
