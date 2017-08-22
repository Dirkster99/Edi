MRULib

This library Implements a WPF/MVVM Control libray (with backend) that manages a Most Recently Used list of files:
- with saving/loading settings from to XML
- List can be grouped by last access (Pinned, Today, Yesterday, Last Week)
- A recently documents menu entry sorted by last access (without grouping is also supported)
- Pinned entries can be moved up and don in the list
- List entries can be removed based on their age (e.g. Remove all entries older than 1 week)
- Support for Light/Black theming is build in

There is a demo application and unit test project to demonstrate usage of the control
and document each feature, such as, the ability to configure a minimum and maximum value
that can be used to keep the resulting number of list entries within defined bounds.
