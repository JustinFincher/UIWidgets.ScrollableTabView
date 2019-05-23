using System;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.scheduler;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Canvas = Unity.UIWidgets.ui.Canvas;
using Color = Unity.UIWidgets.ui.Color;

namespace FinGameWorks.Scripts.Views
{
    public class ScrollableTabStrip : StatefulWidget
    {
        public readonly string Title;
        public readonly Action OnCloseButtonPressed;
        public readonly Action OnStripSelected;
        public readonly bool StripSelected;
        public readonly bool PreserveLeftIntersection;
        public bool IsHoveringCloseButton = false;

        public ScrollableTabStrip(Action onCloseButtonPressed, Action onStripSelected, string title = "", bool stripSelected = false, bool preserveLeftIntersection = false, Key key = null) : base(key)
        {
            Title = title;
            OnCloseButtonPressed = onCloseButtonPressed;
            OnStripSelected = onStripSelected;
            StripSelected = stripSelected;
            PreserveLeftIntersection = preserveLeftIntersection;
        }

        public override State createState()
        {
            return new ScrollableTabStripState();
        }
    }

    public class ScrollableTabStripState : TickerProviderStateMixin<ScrollableTabStrip>
    {
        
        public override Widget build(BuildContext context)
        {
            return new LayoutBuilder(builder: (buildContext, constraints) =>
            {
                return new CustomPaint
                (
                    painter: new ScrollableTabStripPainter(widget.StripSelected, widget.PreserveLeftIntersection, context),
                    child:new Listener
                    (
                        child:
                        new Container
                        (
                            padding: EdgeInsets.fromLTRB(ScrollableTabView.TabStripTopRadius, 0, 0, 0),
                            child: new Row
                                (
                                children: new List<Widget>
                                {
                                    new Expanded
                                    (
                                        flex:2,
                                        child:new Text
                                        (
                                            widget.Title,
                                            style:Theme.of(context).textTheme.body1,
                                            overflow: TextOverflow.ellipsis,
                                            maxLines: 1
                                        )
                                    ),
                                    new Listener
                                    (
                                        onPointerEnter: evt =>
                                        {
                                        },
                                        onPointerExit: evt =>
                                        {
                                        },
                                        child: new Container
                                        (
                                            width:18,
                                            height:18,
                                            decoration: new BoxDecoration
                                            (
                                                borderRadius: BorderRadius.all(9),
                                                color: widget.IsHoveringCloseButton
                                                    ? Colors.red.withOpacity(0.3f)
                                                    : Colors.transparent
                                            ),
                                            child:new IconButton
                                            (
                                                icon: new Icon
                                                (
                                                    icon:Icons.close,
                                                    color: widget.IsHoveringCloseButton
                                                        ? Colors.white
                                                        : Colors.black
                                                ),
                                                iconSize: 12,
                                                padding: EdgeInsets.zero,
                                                onPressed: () =>
                                                {
                                                    widget.OnCloseButtonPressed.Invoke();
                                                }
                                            )  
                                        )
                                    )
                                }
                                )
                        ),
                        onPointerDown: evt =>
                        {
                            widget.OnStripSelected.Invoke();
                        }
                    )
                );
            });
            
        }
    }

    public class ScrollableTabStripPainter: AbstractCustomPainter
    {
        public readonly bool StripSelected;
        public readonly bool PreserveLeftIntersection;
        private readonly BuildContext relativeContext;

        public ScrollableTabStripPainter(bool stripSelected,bool preserveLeftIntersection,BuildContext relativeContext,Listenable repaint = null) : base(repaint)
        {
            StripSelected = stripSelected;
            PreserveLeftIntersection = preserveLeftIntersection;
            this.relativeContext = relativeContext;
        }

        public override void paint(Canvas canvas, Size size)
        {
            Paint bezierPaint = new Paint
            {
                style = PaintingStyle.fill,
                color = StripSelected ? 
                    Theme.of(relativeContext).buttonColor : 
                    Color.lerp(Theme.of(relativeContext).primaryColor,
                        Theme.of(relativeContext).buttonColor,0.5f),
                strokeCap = StrokeCap.round,
                strokeJoin = StrokeJoin.round
            };
            Path bezierPath = new Path();
            bezierPath.moveTo(- ScrollableTabView.TabStripIntersectionWidth, size.height); // left bottom
            bezierPath.lineTo(size.width + ScrollableTabView.TabStripIntersectionWidth, size.height); // right bottom
            bezierPath.lineTo(size.width + ScrollableTabView.TabStripIntersectionWidth / 2, size.height / 2); // right middle
            bezierPath.cubicTo(size.width,0,
                size.width,0,
                size.width - ScrollableTabView.TabStripTopRadius,0); // right top
            bezierPath.lineTo(ScrollableTabView.TabStripTopRadius,0); // left top
            bezierPath.cubicTo(0,0,
                0, 0,
                - ScrollableTabView.TabStripIntersectionWidth / 2, size.height / 2); // left middle
            if (PreserveLeftIntersection)
            {
                bezierPath.lineTo(- ScrollableTabView.TabStripIntersectionWidth, size.height); // left bottom
            }
            else
            {
                bezierPath.lineTo(0, size.height); // left bottom
            }
//            canvas.drawShadow(bezierPath,
//                Colors.black,
//                6f,
//                true);
            canvas.drawPath(bezierPath, bezierPaint);
        }

        public override bool shouldRepaint(CustomPainter oldDelegate)
        {
            return true;
        }
    }
}