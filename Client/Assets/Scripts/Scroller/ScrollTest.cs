using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollTest : MonoBehaviour
{
    [SerializeField] CardScrollView scrollView = default;
    // Start is called before the first frame update
    void Start()
    {
        scrollView.OnSelectionChanged(OnSelectionChanged);

        var items = Enumerable.Range(0, 20)
            .Select(i => new CardItemData($"Cell {i}"))
            .ToArray();

        scrollView.UpdateData(items);
        scrollView.SelectCell(0);
    }

    // Update is called once per frame
    void OnSelectionChanged(int index)
    {
        //selectedItemInfo.text = $"Selected item info: index {index}";
    }
}
