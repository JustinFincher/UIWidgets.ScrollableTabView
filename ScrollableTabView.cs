using System;
using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;

namespace FinGameWorks.Scripts.Views
{
    public class ScrollableTabView : StatefulWidget
    {
        public readonly int TotalCount;
        public Func<int,Widget> GetViewOfIndex = index => new Container();
        public Func<Widget> GetDefaultView = () => new Container();
        public readonly Func<int,String> GetTitleOfIndex = index => "";
        public readonly Action<int> OnSelectedIndexChanged;
        public readonly Action<int> OnIndexClosed;
        public readonly Action<int, int> OnIndexSwitched;
        public readonly float TabControlLeftPadding;
        public readonly float TabControlRightPadding;
        public readonly float TabControlTopPadding;
        public int SelectedIndex;

        public const float TabControlHeight = 40;
        public const float TabStripMaxWidth = 172;
        public const float TabStripIntersectionWidth = 12;
        public const float TabStripTopRadius = 8;

        public ScrollableTabView(int totalCount = 0, int selectedIndex = -1, Func<int,Widget> viewOfIndex = null, Func<Widget> defaultView = null, Func<int,string> titleOfIndex = null, float tabControlLeftPadding = 12, float tabControlRightPadding = 12, float tabControlTopPadding = 8, Action<int> onSelectedIndexChanged = null, Action<int> onIndexClosed = null ,Action<int,int> onIndexSwitched = null, Key key = null) : base(key)
        {
            TotalCount = totalCount;
            SelectedIndex = selectedIndex;
            if (viewOfIndex != null)
            {
                GetViewOfIndex = viewOfIndex;
            }
            if (titleOfIndex != null)
            {
                GetTitleOfIndex = titleOfIndex;
            }
            TabControlLeftPadding = tabControlLeftPadding;
            TabControlRightPadding = tabControlRightPadding;
            TabControlTopPadding = tabControlTopPadding;
            OnSelectedIndexChanged = onSelectedIndexChanged;
            OnIndexSwitched = onIndexSwitched;
            OnIndexClosed = onIndexClosed;
            if (defaultView != null)
            {
                GetDefaultView = defaultView;
            }
        }

        public override State createState()
        {
            return new ScrollableTabViewState();
        }
        
        public static ScrollableTabViewState of(BuildContext context, bool nullOk = false) 
        {
            ScrollableTabViewState result = (ScrollableTabViewState) context.ancestorStateOfType(new TypeMatcher<ScrollableTabViewState>());
            if (nullOk || result != null) {
                return result;
            }

            throw new UIWidgetsError(
                "ScrollableTabViewState.of() called with a context that does not contain a ScrollableTabViewState.\n" +
                "The context used was:\n" + context);
        }
    }

    public class ScrollableTabViewState : TickerProviderStateMixin<ScrollableTabView>
    {
        
        public override Widget build(BuildContext context)
        {
            return new Column
            (
                children: new List<Widget>
                {
                    new Container
                    (
                        height:ScrollableTabView.TabControlHeight + widget.TabControlTopPadding,
                        child: ListView.builder
                        (
                            itemCount: widget.TotalCount,
                            padding:EdgeInsets.fromLTRB
                            (
                                widget.TabControlLeftPadding,
                                widget.TabControlTopPadding,
                                widget.TabControlRightPadding,
                                0
                            ),
                            scrollDirection:Axis.horizontal,
                            physics: new AlwaysScrollableScrollPhysics(),
                            itemBuilder: (buildContext, index) =>
                            {
                                return new ScrollableTabItem
                                (
                                    index: index,
                                    width: ScrollableTabView.TabStripMaxWidth, 
                                    title: widget.GetTitleOfIndex.Invoke(index),
                                    onCloseButtonPressed: widget.OnIndexClosed,
                                    onStripSelected: widget.OnSelectedIndexChanged,
                                    onIndexSwitched: widget.OnIndexSwitched,
                                    getTitleOfIndex: widget.GetTitleOfIndex,
                                    selected: index == widget.SelectedIndex
                                );
                            }
                        )
                    ),
                    new Expanded
                    (
                        child: widget.SelectedIndex >= 0 ? widget.GetViewOfIndex.Invoke(widget.SelectedIndex) : widget.GetDefaultView()
                    )
                }
            );
        }
    }
}