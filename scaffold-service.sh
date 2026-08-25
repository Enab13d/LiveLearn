#!/usr/bin/env bash
set -euo pipefail

# Scaffolds a new microservice with Clean Architecture layering:
#   Domain <- Application <- Infrastructure
#                          <- API
#
# Usage:
#   bash ./scaffold-service.sh <ServiceName> [SolutionFile] [--controllers]
#
# Examples:
#   bash ./scaffold-service.sh Enrollment
#   bash ./scaffold-service.sh Payment LiveLearn.sln --controllers

SERVICE_NAME="${1:?Usage: scaffold-service.sh <ServiceName> [SolutionFile] [--controllers]}"
shift

SOLUTION_NAME=LiveLearn
SOLUTION_FILE="${SOLUTION_NAME}.sln"
USE_CONTROLLERS=false

for arg in "$@"; do
  case "$arg" in
    --controllers) USE_CONTROLLERS=true ;;
    *) SOLUTION_FILE="$arg" ;;
  esac
done

PACKAGES_PROPS="Directory.Packages.props"

SERVICE_ROOT="src/services/${SERVICE_NAME}"
DOMAIN="${SERVICE_ROOT}/${SOLUTION_NAME}.${SERVICE_NAME}.Domain"
APPLICATION="${SERVICE_ROOT}/${SOLUTION_NAME}.${SERVICE_NAME}.Application"
INFRASTRUCTURE="${SERVICE_ROOT}/${SOLUTION_NAME}.${SERVICE_NAME}.Infrastructure"
API="${SERVICE_ROOT}/${SOLUTION_NAME}.${SERVICE_NAME}.API"
API_CSPROJ="${API}/${SOLUTION_NAME}.${SERVICE_NAME}.API.csproj"

# Portable in-place sed: BSD/macOS sed requires the backup suffix attached
# directly to -i (no space) or it silently swallows the next flag (e.g. -E)
# as the suffix instead. -i.bak works identically on GNU and BSD sed.
sed_inplace() {
  local expr="$1" file="$2"
  sed -i.bak -E "${expr}" "${file}"
  rm -f "${file}.bak"
}

echo "==> Scaffolding ${SERVICE_NAME} at ${SERVICE_ROOT}"

# --no-restore: skip the template's automatic post-creation restore. With
# Central Package Management on, that restore runs before we've stripped the
# Version="..." attributes below and always fails noisily with NU1008.
dotnet new classlib -o "${DOMAIN}" -n "${SOLUTION_NAME}.${SERVICE_NAME}.Domain" --no-restore
dotnet new classlib -o "${APPLICATION}" -n "${SOLUTION_NAME}.${SERVICE_NAME}.Application" --no-restore
dotnet new classlib -o "${INFRASTRUCTURE}" -n "${SOLUTION_NAME}.${SERVICE_NAME}.Infrastructure" --no-restore

if [[ "${USE_CONTROLLERS}" == true ]]; then
  dotnet new webapi -o "${API}" -n "${SOLUTION_NAME}.${SERVICE_NAME}.API" --use-controllers --no-restore
else
  dotnet new webapi -o "${API}" -n "${SOLUTION_NAME}.${SERVICE_NAME}.API" --no-restore
fi

echo "==> Normalizing ${API_CSPROJ} for Central Package Management"
# The webapi template pins Version="..." on each PackageReference. Since versions
# are centrally managed via Directory.Packages.props, strip them here so CPM owns them.
sed_inplace 's/(<PackageReference[^>]*) Version="[^"]*"/\1/' "${API_CSPROJ}"

# Add Microsoft.OpenApi to the Api project (version resolved centrally via CPM)
if ! grep -q 'PackageReference Include="Microsoft.OpenApi"' "${API_CSPROJ}"; then
  sed_inplace "s#</Project>#\n  <ItemGroup>\n    <PackageReference Include=\"Microsoft.OpenApi\" />\n  </ItemGroup>\n</Project>#" "${API_CSPROJ}"
fi

echo "==> Ensuring Microsoft.OpenApi is centrally versioned in ${PACKAGES_PROPS}"
if [[ -f "${PACKAGES_PROPS}" ]]; then
  if ! grep -q 'PackageVersion Include="Microsoft.OpenApi"' "${PACKAGES_PROPS}"; then
    # Inserts into the first ItemGroup found - verify placement if you have multiple
    # ItemGroups (e.g. split by category) in Directory.Packages.props.
    sed_inplace '0,/<\/ItemGroup>/{s|</ItemGroup>|  <PackageVersion Include="Microsoft.OpenApi" Version="2.11.0" />\n  </ItemGroup>|}' "${PACKAGES_PROPS}"
  else
    echo "    Already present - skipping."
  fi
else
  echo "    WARNING: ${PACKAGES_PROPS} not found at repo root."
  echo "    Add manually: <PackageVersion Include=\"Microsoft.OpenApi\" Version=\"2.11.0\" />"
fi

echo "==> Wiring project references"
dotnet add "${APPLICATION}" reference "${DOMAIN}"
dotnet add "${INFRASTRUCTURE}" reference "${APPLICATION}"
dotnet add "${API}" reference "${APPLICATION}"
dotnet add "${API}" reference "${INFRASTRUCTURE}"

# Uncomment if every service should reference the shared infra plumbing lib
# dotnet add "${API}" reference src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj
dotnet add "${APPLICATION}" reference src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj
dotnet add "${DOMAIN}" reference src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj

# Uncomment if service contain producers
dotnet add "${APPLICATION}" reference src/Shared/LiveLearn.Contracts/LiveLearn.Contracts.csproj
# Uncomment if service contain consumers
dotnet add "${INFRASTRUCTURE}" reference src/Shared/LiveLearn.Contracts/LiveLearn.Contracts.csproj

echo "==> Adding to solution: ${SOLUTION_FILE}"
dotnet sln "${SOLUTION_FILE}" add "${DOMAIN}" "${APPLICATION}" "${INFRASTRUCTURE}" "${API}"

echo "==> Restoring"
dotnet restore "${SOLUTION_FILE}"

echo "==> Done. Created:"
echo "    ${DOMAIN}"
echo "    ${APPLICATION}"
echo "    ${INFRASTRUCTURE}"
echo "    ${API}"