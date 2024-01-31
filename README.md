# Unobtrusive Code
An extension for Visual Studio that lets you hide away obtrusive code like comments and logging, to let you focus on the actual code. Only supports C#.

## Status
Branch  | Status
------- | ------
master  | [![Build Status](https://dev.azure.com/nkallander/Unobtrusive%20Code/_apis/build/status/niklaskallander.vs-unobtrusive-code?branchName=master)](https://dev.azure.com/nkallander/Unobtrusive%20Code/_build/latest?definitionId=3&branchName=master)
develop | [![Build Status](https://dev.azure.com/nkallander/Unobtrusive%20Code/_apis/build/status/niklaskallander.vs-unobtrusive-code?branchName=develop)](https://dev.azure.com/nkallander/Unobtrusive%20Code/_build/latest?definitionId=3&branchName=develop)

## Download
https://marketplace.visualstudio.com/items?itemName=niklaskallander.UnobtrusiveCode

## Features
Dims out and creates outlining regions for obtrusive code like comments and logging to let you focus on the task at hand. The extension enables all features by default and adds a new command (default: ALT+Q) to toggle the created outlining regions. The features can be enabled/disabled separately, and you have some degree of control over how Unobtrusive Code behaves.

## Options
Options for the extension can be found at `Tools -> Options -> Unobtrusive Code`.

Name                           | Default Value         | Description
------------------------------ | --------------------- | -----------
**Comments**                   |                       |
Dimming enabled                | True                  | Disable/enable comment dimming
Outlining enabled              | True                  | Disable/enable comment outlining
**Dimming**                    |                       |
Dimming opacity                | 0.4                   | Dimming opacity (range: 0.00-1.00)
Dimming opacity toggle enabled | False                 | Disable/Enable toggling of dimming opacity
Dimming opacity toggle key     | RightCtrl             | Hold down this key to temporarily display dimmed obtrusive code with full opacity
**Logging**                    |                       |
Dimming enabled                | True                  | Disable/enable logging dimming
Outlining enabled              | True                  | Disable/enable logging outlining
Outlining enabled              | log. logger. logging. | Set logging identifiers (case-insensitive, strips whitespace between the identifier word and ".")
**Misc**                       |                       |
Parsing delay (ms)             | 1000                  | The delay (ms) until the document is re-parsed after a buffer change
**Outlining**                  |                       |
Collapsed by default           | True                  | Collapse obtrusive code outlining regions as they are found (semi-working)
Collapsed form                 |                       | Collapsed form of obtrusive code outlining regions (defaults to an empty string)

The menu command for toggling created outlining regions can be configured at `Tools -> Options -> Environment -> Keyboard`, the command id is `EditorContextMenus.CodeWindow.CollapseUncollapseObtrusiveCodeOutliningRegions` (default shortcut is ALT+Q). The command can be found in the context menu when right clicking the document.

## Feedback
All feedback is much appreciated. Thanks! (:
