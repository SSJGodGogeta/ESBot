{
  description = "C# development environment with ASP.NET Core 9 for REST APIs";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = nixpkgs.legacyPackages.${system};
        
        # .NET 9
        dotnetSdk = pkgs.dotnet-sdk_9;
        
        # Development tools (only essentials)
        devTools = with pkgs; [
          dotnetSdk
          dotnet-runtime_9
          netcoredbg         # Debugger
          git                # Version control (lightweight, can be removed)
          curl               # For testing the API
          jmeter             # For performance testing the api
        ];
        
      in
      {
        devShells.default = pkgs.mkShell {
          buildInputs = devTools;
          
          shellHook = ''
            echo "🔧 C# / .NET 9 Development Environment"
            echo "📦 Dotnet SDK: $(dotnet --version)"
            echo ""
            echo "🚀 To create a new REST API:"
            echo "  mkdir MyApi && cd MyApi"
            echo "  dotnet new webapi -n MyApi"
            echo "  dotnet run"
            echo ""
            echo "📝 Build & run existing project:"
            echo "  dotnet build"
            echo "  dotnet run"
            echo ""
            echo "➕ Add NuGet package:"
            echo "  dotnet add package [PackageName]"
            echo ""
            
            # .NET Source Generators support
            export DOTNET_EnableWriteXorExecute=0
          '';
          
          # Development environment
          ASPNETCORE_ENVIRONMENT = "Development";
        };
      });
}
