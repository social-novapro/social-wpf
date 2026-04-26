# social-wpf
Created by Daniel Kravec

## Threads
| Thread | Role | Description |
| --- | --- | --- |
| UI Thread | Main thread / WPF interface | Required main app thread |
| Feed Sync Thread | Fetches posts / refreshes feed | Background API work |
| Upload / Post Thread | Waits for queued posts and uploads them | Background API work |
| Media Cache Thread | Downlods/caches avatars or images | Long-running work task |