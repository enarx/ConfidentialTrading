# ConfidentialTrading: a sample .NET application on Enarx

This is a sample application, based on Steve Sanderson's [Greenhouse Monitor](https://github.com/SteveSandersonMS/GreenhouseMonitor), showing a typical ASP.NET Core code, including nontrivial features like WebSockets (via SignalR) and gRPC-Web, can work unmodified when built as a WASI-compliant `.wasm` file and run inside [wasmtime](https://github.com/bytecodealliance/wasmtime) and [enarx](https://enarx.dev/).

## Install the prerequisites

 * Install [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
   * Download the [dotnet-install scripts](https://dotnet.microsoft.com/en-us/download/dotnet/scripts)
   * On Debian, install the following packages: `sudo apt-get update && sudo apt-get install build-essential cmake npm mono-devel dotnet --list-sdks`
   * Run `sudo chmod +x ./dotnet-install.sh`
   * Install the following .NET version: `./dotnet-install.sh --channel 7.0 --version 7.0.100-preview.5.22307.18`
   * Export paths: `export DOTNET_ROOT=$HOME/.dotnet` and `export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools`
 * Install [Node.js](https://nodejs.org/en/) to build the Svelte UI
 * Install [Wasmtime](https://wasmtime.dev/) or [Enarx](https://enarx.dev/docs/Quickstart) to run the .NET application

## Clone and build ConfidentialTrading

 * Clone the ConfidentialTrading repository:
   * `git clone https://github.com/enarx/ConfidentialTrading.git`
 * Build the Svelte UI
   * `cd ConfidentialTrading/src/ConfidentialTrading/UI`
   * `npm install`
   * `npm run build`
   * `cd ../../..`
 * Build the .NET application
   * `cd src/ConfidentialTrading`
   * `dotnet build`

## Run ConfidentialTrading

 * `cd src/ConfidentialTrading` (unless you're already there)
 * Run on dotnet:
   * `dotnet run`
   * Or, if you're using Visual Studio, press Ctrl+F5
* Run on wasmtime:
   * `wasmtime bin/Debug/net7.0/ConfidentialTrading.wasm --tcplisten 0.0.0.0:5000`
* Run on enarx:
   * `enarx run --wasmcfgfile ./Enarx.toml ./bin/Debug/net7.0/ConfidentialTrading.wasm`
* Browse to `http://localhost:5000`
* Remember to do a `dotnet build` each time you make further code changes
* This assumes `wasmtime` or `enarx` are available on your system `PATH`

## Attach a debugger

 * Start the application using this command:
   * `wasmtime bin/Debug/net7.0/ConfidentialTrading.wasm --tcplisten 0.0.0.0:8001 --tcplisten 0.0.0.0:11000 --env DEBUGGER_FD=3`
   * Open VS Code inside the `src/ConfidentialTrading` directory (e.g., run `code .` from there)
   * Ensure you have the [Mono Debugger](https://marketplace.visualstudio.com/items?itemName=ms-vscode.mono-debug) extension installed
   * Set a breakpoint somewhere in the .NET code
   * Go into VS Code's "Run and debug" tool and click *Attach*
   * You should see the console output `Accepted connection from client, socket fd=5` and then, shortly after, `Now listening on...`
   * At this point, if you do something to cause the breakpoint to be hit, you should see it in VS Code

## Caveats

The [`Wasi.Sdk`](https://github.com/SteveSandersonMS/dotnet-wasi-sdk) is a very early experimental preview, and many things aren't yet implemented. In particular, .NET's garbage collection is not enabled at all, so over time the memory usage will grow indefinitely until the application terminates. This happens after handling several hundred HTTP requests. This is obviously something we intend to fix soon.

Another issue you may encounter is that wasmtime's `sock_accept` support has some bugs. For example if a client disconnects ungracefully while a TCP connection is open, then the `wasmtime` process will terminate (this is particularly the case on Windows).

## About the included package binaries

Normally people don't bundle the `.nupkg` package binaries with their application sources, so you may be wondering why there are ~15 MB of binaries in the `packages/` directory in this repo. It's simply because the version of `Wasi.Sdk` that supports debugging isn't yet published to the public NuGet feed. Once the latest package builds are published, it would not be necessary to have the `packages/` directory in this repo at all.