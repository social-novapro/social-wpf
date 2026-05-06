# social-wpf
Created by Daniel Kravec

## Project Overview
`social-wpf` is a WPF desktop client for the Interact social platform. The application allows a user to log in, view a feed, queue posts for upload, reply or quote posts, and cache media such as profile images and post attachments.

The main purpose of the project is to demonstrate multithreading in a C# desktop application. The app uses a WPF UI thread and several background worker threads that share common state safely.

## Main Features
- Login using the Interact API
- Feed loading from the Interact backend
- Post creation through a queued upload system
- Reply and quote support
- Profile image and attachment caching
- Thread monitor panel showing worker activity
- Isolated Storage for saved login/session settings
- Logout support that clears saved user session data


## Threads
| Thread | Role | Description |
| --- | --- | --- |
| UI Thread | Main WPF thread | Displays the interface, handles button clicks, and reads shared state for display |
| Feed Sync Thread | Background feed worker | Fetches posts from the Interact API and updates the shared feed list |
| Upload / Post Thread | Background upload worker | Waits for queued posts and uploads them to the API |
| Media Cache Thread | Background media worker | Downloads and caches profile images and post attachments |

## Shared Resources
The worker threads share several common resources through `SharedAppState`:

- Feed post list
- Post upload queue
- Thread status list
- Media cache dictionary
- Feed pagination state

## Isolated Storage
The app uses Isolated Storage to save and retrieve application settings. This includes:

- Login state
- Last username
- User ID
- User token / access token
- Basic session information

## API Usage
The app communicates with the Interact API using `HttpClient`.

API work is separated into `InteractApiClient`, while UI pages call the API client indirectly through the worker system where appropriate.

## References
- Microsoft HttpClient documentation: https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient
- Microsoft Isolated Storage documentation: https://learn.microsoft.com/en-us/dotnet/standard/io/isolated-storage
- Microsoft Monitor class documentation: https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor
- Microsoft Thread class documentation: https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread