using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


/// <summary>
/// ui生命周期:实例化对象->OnAwake(A)->OnAwake(B)->OnAwakeFinished->UIClearData->show->UIAnimationSetup
/// 取自己的组件请用OnAwake->取其他窗口组件使用OnAwakeFinished
/// 如果是动态创建的窗口子项那么组件初始化在OnAwake里,不然其他窗口调用不到.
/// </summary>
public class UITemplate : AssetsLoader
{
    public RectTransform rectTransform
    {
        get { return _rect_transform; }
    }

    public System.Object userData { get; set; }

    private Vector3 _originalPosition;
    private Vector3 _originalLocalPosition;
    private RectTransform _rect_transform = null;
    private bool _ui_inited = false;
    private List<UITemplate> _lst_ui_tpl_child = new List<UITemplate>();
    private bool _is_show = false;

    protected override void Awake()
    {
        _rect_transform = gameObject.GetComponent<RectTransform>();
        _originalPosition = _rect_transform.position;
        _originalLocalPosition = _rect_transform.localPosition;
        OnAwake();
    }

    //一般加重所有资源完成后调用
    public virtual void UIAwakeAllAfter(bool clear_data = true, bool include_child = true)
    {
        if (_ui_inited) return;
        _ui_inited = true;
        OnAwakeFinished();
        if (clear_data) OnClearData();
        if (include_child) _AwakeChildrenUIAllAfter();
    }


    //loading call OnAwake
    protected virtual void OnAwake()
    {
    }

    protected virtual void OnClearData()
    {
        _ClearChildrenUIData();
    }

    protected virtual void OnAwakeFinished()
    {
    }

    /// <summary>
    /// 自身或包含自身的对象显示时，当前对象为显示状态，则调用
    /// </summary>
    protected virtual void OnShow()
    {
        _UpdateChildrenShowOrHide(true);
    }

    /// <summary>
    /// 自身或包含自身的对象隐藏时，当前对象为显示状态，则调用
    /// </summary>
    protected virtual void OnHide()
    {
        _UpdateChildrenShowOrHide(false);
    }

    private void _AwakeChildrenUIAllAfter()
    {
        foreach (var ui_base in _lst_ui_tpl_child)
        {
            ui_base.UIAwakeAllAfter(false);
        }
    }


    private void _ClearChildrenUIData()
    {
        foreach (var ui_base in _lst_ui_tpl_child)
        {
            ui_base.OnClearData();
        }
    }

    public void UpdateUIChildLst()
    {
        _lst_ui_tpl_child.Clear();
        _updateUIChildLst(transform);
    }

    private void _updateUIChildLst(Transform cur_transform)
    {
        var child_count = cur_transform.childCount;
        for (var i = 0; i < child_count; i++)
        {
            var transform_child = cur_transform.GetChild(i);
            var ui_base = transform_child.GetComponent<UITemplate>();
            if (ui_base == null)
            {
                _updateUIChildLst(transform_child);
                continue;
            }

            _lst_ui_tpl_child.Add(ui_base);
        }
    }

    public virtual bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    public virtual void SetVisible(bool is_visible)
    {
        if (gameObject.activeSelf != is_visible || _is_show != is_visible)
        {
            gameObject.SetActive(is_visible);

            if (is_visible)
            {
                _OnShow();
            }
            else
            {
                _OnHide();
            }
        }
    }


    public bool IsShow()
    {
        return _is_show;
    }

    /// <summary>
    /// 自身显示时调用
    /// </summary>
    private void _OnShow()
    {
        _is_show = true;

        //每次显示后都更新子级对象【因为存在动态添加的对象】
        UpdateUIChildLst();

        UIAwakeAllAfter();
        OnShow();
    }

    /// <summary>
    /// 自身隐藏时调用
    /// </summary>
    private void _OnHide()
    {
        if (_ui_inited) OnClearData();
        _is_show = false;
        OnHide();
    }

    private void _UpdateChildrenShowOrHide(Boolean if_show)
    {
        foreach (var ui_tpl in _lst_ui_tpl_child)
        {
            if (ui_tpl == null) continue;
            if (!ui_tpl.IsVisible()) continue;

            if (if_show)
            {
                ui_tpl._OnShow();
            }
            else
            {
                ui_tpl._OnHide();
            }
        }
    }


    #region UI Tween

    protected Tweener movingTween;

    public Tweener MoveBy(Vector2 delta, float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);
        var oldPos = this._originalLocalPosition;
        this.transform.localPosition = new Vector3(_originalLocalPosition.x + delta.x,
            _originalLocalPosition.y + delta.y, oldPos.z);
        movingTween = MoveToLocalPosition(oldPos, time, delay, ease, move_end_callback);
        return movingTween;
    }

    public Tweener MoveFrom(Vector2 localFrom, float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);
        var oldPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(localFrom.x, localFrom.y, oldPos.z);
        movingTween = MoveToLocalPosition(oldPos, time, delay, ease, move_end_callback);
        return movingTween;
    }

    public Tweener MoveByLeft(float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);
        var oldPos = this._originalLocalPosition;
        this.transform.localPosition = new Vector3(_originalLocalPosition.x + -rectTransform.sizeDelta.x,
            _originalLocalPosition.y, oldPos.z);
        movingTween = MoveToLocalPosition(oldPos, time, delay, ease, move_end_callback);
        return movingTween;
    }

    public Tweener MoveByRight(float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);

        var covPos = UICamera.MainUICamera.WorldToScreenPoint(this.transform.position);
        covPos = new Vector3(covPos.x + (Screen.width - covPos.x) + rectTransform.sizeDelta.x, covPos.y, covPos.z);
        covPos = UICamera.MainUICamera.ScreenToWorldPoint(covPos);
        covPos = this.transform.InverseTransformPoint(covPos);
        var oldPos = this._originalLocalPosition;
        this.transform.localPosition =
            new Vector3(_originalLocalPosition.x + covPos.x, _originalLocalPosition.y, oldPos.z);
        movingTween = MoveToLocalPosition(oldPos, time, delay, ease, move_end_callback);
        return movingTween;
    }

    public Tweener MoveByTop(float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);
        var oldPos = this._originalLocalPosition;
        this.transform.localPosition = new Vector3(_originalLocalPosition.x,
            _originalLocalPosition.y + rectTransform.sizeDelta.y, oldPos.z);
        movingTween = MoveToLocalPosition(oldPos, time, delay, ease, move_end_callback);
        return movingTween;
    }

    public Tweener MoveByBot(float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);
        var oldPos = this._originalLocalPosition;
        this.transform.localPosition = new Vector3(_originalLocalPosition.x,
            _originalLocalPosition.y + -rectTransform.sizeDelta.y, oldPos.z);
        movingTween = MoveToLocalPosition(oldPos, time, delay, ease, move_end_callback);
        return movingTween;
    }

    public Tweener MoveByX(float delta, float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);
        var oldPos = this._originalLocalPosition;
        this.transform.localPosition =
            new Vector3(_originalLocalPosition.x + delta, _originalLocalPosition.y, oldPos.z);
        movingTween = MoveToLocalPosition(oldPos, time, delay, ease, move_end_callback);
        return movingTween;
    }

    public Tweener MoveByY(float delta, float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);
        var oldPos = this._originalLocalPosition;
        this.transform.localPosition =
            new Vector3(_originalLocalPosition.x, _originalLocalPosition.y + delta, oldPos.z);
        movingTween = MoveToLocalPosition(oldPos, time, delay, ease, move_end_callback);
        return movingTween;
    }

    public Tweener MoveTo(Vector3 pos, float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        if (movingTween != null) movingTween.Kill(true);
        var oldPos = this._originalLocalPosition;
        this.transform.localPosition = oldPos;
        movingTween = MoveToLocalPosition(pos, time, delay, ease, move_end_callback);
        return movingTween;
    }


    public Tweener MoveToLocalPosition(Vector3 pos, float time, float delay = 0, Ease ease = Ease.OutCubic,
        Action<UITemplate> move_end_callback = null)
    {
        var t = transform.DOLocalMove(pos, time).SetEase(ease).SetDelay(delay);
        if (move_end_callback != null)
        {
            t.OnComplete(() => move_end_callback(this)); //todo pos * getScale()
        }

        return t;
    }

    #endregion

    public LayerType GetLayer()
    {
        return LayerType.POP_UP;
    }
}