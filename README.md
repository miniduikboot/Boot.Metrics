# Boot.Metrics

Boot.Metrics is a plugin that integrates [OpenTelemetry metrics](https://github.com/open-telemetry/opentelemetry-dotnet) into [Impostor](https://github.com/Impostor/Impostor) using a plugin.


# Installation

1. Set up an Impostor server. See the release notes of the version of Boot.Metrics you're installing for the minimum version requirements.
2. Download the latest release and extract it into your Impostor folder. Make sure the `plugins` and `libraries` folders are next to the `Impostor.Server` executable. 
3. If you're using a reverse proxy for Impostor, make sure to expose the `/metrics` path. We recommend limiting 
4. Add a scraping job to Prometheus. To properly use multi-server support, you should add 
5. Add [our dashboard](https://github.com/miniduikboot/Boot.Metrics/blob/main/resources/dashboard.json) to Grafana as a base to start from, alternatively create your own.
