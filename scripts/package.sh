#!/usr/bin/env bash
set -euo pipefail

version="${1:-1.0.0.0}"
repository_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
archive_directory="${repository_root}/artifacts"
archive_path="${archive_directory}/NativeHoldKey-${version}.zip"
assembly_path="${repository_root}/HoldKeyPlugin/bin/Release/net8.0/HoldKeyPlugin.dll"

dotnet test "${repository_root}/HoldKeyPlugin.sln" --configuration Release
dotnet build "${repository_root}/HoldKeyPlugin/HoldKeyPlugin.csproj" --configuration Release --no-restore
mkdir -p "${archive_directory}"

working_directory="$(mktemp -d)"
trap 'rm -rf "${working_directory}"' EXIT
cp "${assembly_path}" "${working_directory}/HoldKeyPlugin.dll"

(
  cd "${working_directory}"
  zip -q "${archive_path}" HoldKeyPlugin.dll
)

shasum -a 256 "${archive_path}"
