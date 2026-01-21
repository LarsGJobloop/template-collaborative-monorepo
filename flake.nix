{
  description = "Collaborative Monorepo Template";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs?ref=nixos-unstable";
  };

  outputs =
    { self, nixpkgs }:
    let
      # The host systems we (intend to) support.
      supportedSystems = [
        "x86_64-linux" # Linux on x86_64
        "aarch64-darwin" # macOS on aarch64
        # TODO! Not verified, but should work.
        # Move them up when verified.
        "x86_64-darwin" # macOS on x86_64
        "aarch64-linux" # Linux on aarch64
      ];

      # Helper functions for generating the various shells.
      withSystem = nixpkgs.lib.genAttrs supportedSystems;
      withPkgs = callback: withSystem (system: callback (import nixpkgs { inherit system; }));
    in
    {
      # The set of predefined development shells in use by the project.
      devShells = withPkgs (pkgs: rec {
        # The default development shell for the project.
        default = development;

        # The standard development shell for the project.
        development = pkgs.mkShell {
          packages = with pkgs; [
            nodejs_24
            dotnetCorePackages.sdk_10_0-bin
            opentofu
          ];
        };
      });

      # The Nix file formatter for the project.
      formatter = withPkgs (pkgs: pkgs.nixfmt-rfc-style);
    };
}
