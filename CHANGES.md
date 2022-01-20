## 0.5.3 (2021/01/20)
* RollForward policy set to LatestMinor

## 0.5.1 (2021/08/31)
* WebClient is embedded into installer

## 0.4.39-beta (2021/04/30)
* Spectre.Console reverted to 0.37 (regression)

## 0.4.38-beta (2021/04/28)
* exposed version on /api/v1/version
* update to Spectre.Console 0.39

## 0.4.34-beta (2021/02/09)
* sanitizing jagged standard error & output
* turning word-wrap off in monitor

## 0.4.32-beta (2021/02/08)
* removed bug which caused some tasks being never restarted

## 0.4.29-beta (2021/02/07)
* removed throttling startup
* better logging around infinite "SyncInProgress" (bug still not fixed)

## 0.4.28-beta (2021/02/05)
* deferred startup (debouncing sync message)

## 0.4.26-beta (2021/02/03)
* monitoring can be filtered by task

## 0.4.24-beta (2021/02/02)
* improved performance when starting processes

## 0.4.23-beta (2021/01/26)
* Spectre.Console updated to 0.37.1-preview.0.13 (with --version fix)

## 0.4.22-beta (2021/01/20)
* packages updated

## 0.4.21-beta (2021/01/20)
* external handling of unparsed args (bypass Spectre)

## 0.4.20-beta (2021/01/19)
* server side expression filtering

## 0.4.16-beta (2021/01/19)
* removed --shell (now default), added -direct-execute switch

## 0.4.15-beta (2021/01/12)
* prevent status flick

## 0.4.14-beta (2021/01/12)
* added in progress spinner
* improved rest call performance

## 0.4.13-beta (2021/01/12)
* fixed a bug causing filtering with wildcards not working

## 0.4.12-beta (2021/01/11)
* list command no longer needs *
* info command separated from list
* spec command added

## 0.4.10-beta (2021/01/05)
* moving columns around to reduce neck pain

## 0.4.9-beta (2021/01/05)
* filter expression for tags now requires #

## 0.4.8-beta (2021/01/05)
* added option to use shell to execute commands

## 0.4.7-beta (2021/01/05)
* added column width limits in compact view 

## 0.4.6-beta (2021/01/04)
* filtering for logs and monitor
* tags expression now handles wildcards

## 0.4.5-beta (2021/01/03)
* moved to Spectre.Console for command line arguments

## 0.4.4-beta (2020/12/29)
* added "Open UI" context menu item

## 0.4.3-beta (2020/12/29)
* re-release with new updated metadata

## 0.4.2-beta (2020/12/18)
* updated serialization settings for SignalR

## 0.4.1-beta (2020/12/17)
* fixed process killing to kill whole tree

## 0.4.0-beta (2020/12/17)
* added "monitor" command
* realtime notifications with SignalR 

## 0.3.2-beta (2020/12/15)
* fixes to installer

## 0.3.1-beta (2020/12/14)
* ported to .NET 5

## 0.3.0-beta (2020/12/13)
* added tags to tasks

## 0.2.2-beta (2020/12/10)
* initial release
