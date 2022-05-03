using System.Collections;
using System.Collections.Generic;
using FancyScrollView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCell : FancyCell<CardItemData, CardContext>
{
    [SerializeField] Animator animator = default;
    [SerializeField] TextMeshProUGUI message = default;
    static class AnimatorHash
    {
        public static readonly int Scroll = Animator.StringToHash("scroll");
    }
    public override void UpdateContent(CardItemData itemData)
    {
        message.text = itemData.Message;

        var selected = Context.SelectedIndex == Index;
        // image.color = selected
        //     ? new Color32(0, 255, 255, 100)
        //     : new Color32(255, 255, 255, 77);
    }

    public override void UpdatePosition(float position)
    {
        currentPosition = position;

        if (animator.isActiveAndEnabled)
        {
            animator.Play(AnimatorHash.Scroll, -1, position);
        }

        animator.speed = 0;
    }
    float currentPosition = 0;

    void OnEnable() => UpdatePosition(currentPosition);
}
