using System;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace FinGameWorks.Scripts.Views
{
    public class ScrollableTabItem : StatefulWidget
    {
        public readonly int Index;
        public readonly float Width;
        public bool Dragging = false;
        public readonly string Title;
        public readonly Action<int> OnCloseButtonPressed;
        public readonly Action<int> OnStripSelected;
        public readonly Action<int, int> OnIndexSwitched;
        public readonly Func<int, String> GetTitleOfIndex;
        
        public readonly bool Selected;

        public ScrollableTabItem(int index, float width, string title, Action<int> onCloseButtonPressed, Action<int,int> onIndexSwitched, Action<int> onStripSelected, Func<int,String> getTitleOfIndex, bool selected, Key key = null) : base(key)
        {
            Index = index;
            Width = width;
            Title = title;
            OnCloseButtonPressed = onCloseButtonPressed;
            OnStripSelected = onStripSelected;
            OnIndexSwitched = onIndexSwitched;
            Selected = selected;
            GetTitleOfIndex = getTitleOfIndex;
        }

        public override State createState()
        {
            return new ScrollableTabItemState();
        }
    }

    public class ScrollableTabItemState : TickerProviderStateMixin<ScrollableTabItem>
    {
        public override Widget build(BuildContext context)
        {
            return new Draggable<int>
            (
                child: new AnimatedSize
                (
                    child: new AnimatedOpacity(child:widget.Dragging ?
                            (Widget) new Container
                            (
                                width: 0
                            ) : new Row
                            (
                                children: new List<Widget>
                                {
                                    draggableStripBody(widget.Index),
                                    draggableStripIntersection(widget.Index)
                                }
                            ),
                        curve: Curves.easeInOut,
                        duration: TimeSpan.FromSeconds(0.6f),
                        opacity: widget.Dragging ? 0.0f : 1.0f
                    ),
                    vsync:this,
                    curve: Curves.easeInOut,
                    duration: TimeSpan.FromSeconds(0.4f)
                ), 
                feedback: new Card(child:draggableStripBody(widget.Index)), 
                data: widget.Index,
                affinity: Axis.vertical,
                maxSimultaneousDrags: 1,
                onDragStarted: () => { setState(() => widget.Dragging = true); },
                onDragCompleted:() => { setState(() => widget.Dragging = false); },
                onDragEnd:details => { setState(() => widget.Dragging = false); },
                onDraggableCanceled:(velocity, offset) => { setState(() => widget.Dragging = false); }
            );
        }
        
        private Widget draggableStripBody(int index)
        {
            return new SizedBox
            (
                width: widget.Width,
                height: ScrollableTabView.TabControlHeight,
                child: new ScrollableTabStrip
                (
                    title: widget.GetTitleOfIndex.Invoke(index),
                    stripSelected: widget.Selected,
                    onStripSelected: () =>
                    {
                        widget.OnStripSelected.Invoke(index);
                    },
                    onCloseButtonPressed: () =>
                    {
                        widget.OnCloseButtonPressed.Invoke(index);
                    },
                    preserveLeftIntersection: true
                )
            );
        }

        private Widget draggableStripIntersection(int index)
        {
            return new DragTarget<int>(builder: (buildContext, data, rejectedData) =>
                {
                    return new AnimatedOpacity
                    (
                        child:new AnimatedSize
                        (
                            vsync:this,
                            curve: Curves.easeInOut,
                            duration: TimeSpan.FromSeconds(0.4f),
                            child: data.Count > 0
                                ? (Widget) new Row
                                (
                                    children: new List<Widget>
                                    {
                                        new Container
                                        (
                                            width: ScrollableTabView.TabStripIntersectionWidth * 2 + ScrollableTabView.TabStripMaxWidth
                                        )
                                    }
                                )
                                : new Container
                                (
                                    width: ScrollableTabView.TabStripIntersectionWidth
                                )
                        ),
                        opacity: data.Count > 0 ? 1.0f : 0.0f,
                        curve: Curves.easeInOut,
                        duration: TimeSpan.FromSeconds(0.6f)
                    );
                }, 
                onWillAccept: data => { return data != index; },
                onAccept: data => { widget.OnIndexSwitched.Invoke(data,index); },
                onLeave:data => { }
            );
        }
    }
}