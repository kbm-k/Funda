## Getting Started

Application is built using Azure Functions, .NET Core 3.1, C#.

Funda project:
I decided to not to divide into several layers/tiers, that would be overengineering. So Azure Function project contains:
-Models. Some files contain several classes - just to make it easier to view the code in github.
-Services. ApiService.cs contains the main logic of app.
-Extensions. Here retry and circuit breaker policies are implemented.

How it works:
1) I run the first request to endpoint. Read total number of objects (if it is equal to 0 -> return), read total number of pages.
2) Create a task for each page and put them into collection
3) Run them, using Task.WhenAll
4) Group the data by makelaarId, order by the number of objects, take 10 and return them

I wanted to batch the tasks and run them by portions of N tasks. But I tried it and measured the time, it wasn't faster, so decided to not to use the batches. Probably performance didn't improve just because there were 158 pages/requests, on a larger number of queries, the results could be different.

Funda.Tests project:
Contains a file with test data and a unit-test to test ApiService.GetAgent method.