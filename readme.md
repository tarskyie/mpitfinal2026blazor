# Remote Education Blazor web-app

This is a Blazor WebAssembly  for a remote education application. It provides functionality for students, teachers, and parents.

## Features

- **User Registration:** Students, Teachers, and Parents.
- **Tasks:** The app tracks tasks on the calendar.
- **AI chatbot**: An LLM connected through OpenAI-compatible chat completions API helps users to track tasks.

## Prerequisites

1. Visual Studio 2026 with ASP.NET development components.
2. .NET 10 SDK

## Setup

1. **Clone the repository:**
   ```powershell
   git clone https://github.com/tarskyie/mpitfinal2026blazor
   ```

2. **Restore packages:**
   ```powershell
   dotnet restore
   ```
   
3. **Configure connection**
   at ProfileService.cs
   ```C#
   private const string baseUrl = <Your API base URL>
   ```

4. **Run local server**
   ```powershell
   dotnet run
   ```