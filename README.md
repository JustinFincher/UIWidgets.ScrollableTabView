# UIWidgets.ScrollableTabView
> A Chrome-like scrollable tab view widget for UIWidgets

# Usage

```csharp
new ScrollableTabView
(
    int totalCount = 0, // total tab count
    int selectedIndex = -1, // current tab count
    Func<int,Widget> viewOfIndex = null, // bottom view provider
    Func<int,string> titleOfIndex = null, // title provider
    float tabControlLeftPadding = 12, 
    float tabControlRightPadding = 12, 
    float tabControlTopPadding = 8, 
    Action<int> onSelectedIndexChanged = null,  // need to change your selectedIndex data in this scope
    Action<int> onIndexClosed = null,  // need to delete your data item in this
    Action<int,int> onIndexSwitched = null,  // need to reorder your data list in this
    Key key = null
)
```

# Sample

```csharp
using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace FinGameWorks.Scripts.Views
{
    public class AppMainScreen : StatefulWidget
    {
        public List<string> TabList = new List<string>();
        public int ScrollTabIndex = -1;

        public AppMainScreen(Key key = null) : base(key)
        {
            for (int i = 0; i < 16; i++)
            {
                TabList.Add("Tab " + i.ToString());
            }
        }

        public override State createState()
        {
            return new AppMainScreenState();
        }
    }

    class AppMainScreenState : State<AppMainScreen>
    {
        public override Widget build(BuildContext context)
        {
            return new Scaffold
            (
                body: new SafeArea
                (
                    child: new ScrollableTabView
                        (
                        totalCount:widget.TabList.Count,
                        selectedIndex:widget.ScrollTabIndex,
                        tabControlLeftPadding: 16,
                        tabControlRightPadding:16,
                        tabControlTopPadding:16,
                        viewOfIndex: i => { return new Center(child:new Text(widget.TabList[i]));},
                        titleOfIndex: i => { return widget.TabList[i]; },
                        onSelectedIndexChanged: i =>
                        {
                            setState(() => widget.ScrollTabIndex = i);
                        },
                        onIndexClosed: i =>
                        {
                            setState(() =>
                            {
                                if (widget.ScrollTabIndex >= i)
                                {
                                    widget.ScrollTabIndex--;
                                }
                                widget.TabList.RemoveAt(i);
                            });
                        },
                        onIndexSwitched: (pre, nex) =>
                        {
                            setState(() =>
                            {
                                widget.ScrollTabIndex = nex;
                                if (pre < nex)
                                {
                                    var tmp = widget.TabList[pre];
                                    for (int i = pre; i < nex; i++)
                                    {
                                        widget.TabList[i] = widget.TabList[i + 1];
                                    }
                                    widget.TabList[nex] = tmp;
                                }
                                else if (pre > nex)
                                {
                                    var tmp = widget.TabList[pre];
                                    for (int i = pre; i > nex; i--)
                                    {
                                        widget.TabList[i] = widget.TabList[i - 1];
                                    }
                                    widget.TabList[nex + 1] = tmp;
                                }
                            });
                        }
                    )
                )
            );
        }
    }
}
```