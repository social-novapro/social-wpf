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

## Thread Safety and Monitor Usage
Shared resources are protected using `lock` statements and the `Monitor` class. The most important example is the post upload queue.

The UI thread acts as the producer. When the user creates a post, reply, or quote, the post is placed into a shared queue. The Upload / Post Thread acts as the consumer. It waits while the queue is empty and wakes when the UI thread adds a new post.

The upload queue uses:
- `Monitor.Enter`
- `Monitor.Exit`
- `Monitor.Wait`
- `Monitor.Pulse`
- `Monitor.PulseAll`

This allows the upload thread to sleep when there is no work, instead of constantly checking the queue. When a new post is queued, the UI thread calls `Monitor.Pulse` to wake the upload thread. When the application closes, `Monitor.PulseAll` is used so waiting threads can stop cleanly.

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

## Threading Concepts Demonstrated
The project demonstrates several thread-related concepts from the course:
- Manual thread creation using `Thread`
- Thread names
- Thread priorities
- Background threads
- Thread sleeping
- Thread joining when the application closes
- Passing shared state into worker classes
- Safe shared-resource access
- `Monitor.Wait` / `Monitor.Pulse`
- Per-thread static data using `[ThreadStatic]`
- UI-safe updates using a WPF timer and shared status objects

## Project Requirement Summary
This project satisfies the Advanced Programming project brief because it includes:
- A WPF graphical interface
- Four total threads: the main UI thread and three worker threads
- Shared resources accessed by multiple threads
- Safe access to shared resources
- Explicit use of the `Monitor` class
- More than six thread-related techniques
- Isolated Storage for saving and retrieving login/session data
- API interaction and media caching to give each worker thread meaningful work

## References
- Microsoft HttpClient documentation: https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient
- Microsoft Isolated Storage documentation: https://learn.microsoft.com/en-us/dotnet/standard/io/isolated-storage
- Microsoft Monitor class documentation: https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor
- Microsoft Thread class documentation: https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread