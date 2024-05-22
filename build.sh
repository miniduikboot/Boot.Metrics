#!/usr/bin/env bash

set -euxo pipefail

TAG=$(git describe --tags --long --dirty --always)

dotnet restore --locked-mode
dotnet publish -c Release -p VersionPrefix="${TAG:1}"

PACKDIR=$(mktemp -d)
mkdir "$PACKDIR"/libraries
mkdir "$PACKDIR"/plugins

cp Boot.Metrics/bin/Release/net*/publish/Boot.Metrics.dll "$PACKDIR"/plugins
cp Boot.Metrics/bin/Release/net*/publish/OpenTelemetry*.dll "$PACKDIR"/libraries

pushd "$PACKDIR"
zip -9r Boot.Metrics-"${TAG}".zip libraries plugins
popd

mkdir -p output
cp "${PACKDIR}"/Boot.Metrics-"${TAG}".zip output

if [[ -v GITHUB_ENV ]]
then
    echo "{TAG_NAME}={${TAG}}" >> "$GITHUB_OUTPUT"
fi
