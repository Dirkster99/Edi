MsgBox

This project shows the implementation of a custom message box service that
is driven by a service locator interface. It inlcudes a notification component
that can be used to implement positioned pop-up messages in the vicinity of
related controls. All implementation is in WPF and follows MVVM without compromises.

See https://github.com/Dirkster99/MsgBox for more details.

Custom message boxes are themeable (black and light) and use the same API as the
standard .Net message boxes. There also types of message boxes - such as, display
of .Net Exceptions (including Stacktrace information) - which are not supported in
the standard .Net API.