# Boot.Metrics

Boot.Metrics is a plugin that integrates [OpenTelemetry metrics](https://github.com/open-telemetry/opentelemetry-dotnet) into [Impostor](https://github.com/Impostor/Impostor) using a plugin.


# Installation

1. Set up an Impostor server. See the release notes of the version of Boot.Metrics you're installing for the minimum version requirements.
2. Download the latest release and extract it into your Impostor folder. Make sure the `plugins` and `libraries` folders are next to the `Impostor.Server` executable. 
3. If you're using a reverse proxy for Impostor, make sure to expose the `/metrics` path. We recommend limiting access to your Prometheus instance only.
4. Add a scraping job to Prometheus. To properly use multi-server support, you should add a server_name label:

    ```yml
    - targets: ['localhost:22023']
      labels:
        server_name: your_awesome_server_name_here # but you may wish to keep it short
        # If you're using https on your reverse proxy, add the following line.
        # __scheme__: https
        # If you're remapping paths in a reverse proxy, add the following line:
        # __metrics_path__: /alternative/path/to/metrics
    ```

5. Add [our dashboard](https://github.com/miniduikboot/Boot.Metrics/blob/main/resources/dashboard.json) to Grafana as a base to start from, alternatively create your own.

Setting up [Impostor](https://github.com/Impostor/Impostor/tree/master/docs), [Prometheus](https://prometheus.io/docs/introduction/overview/) and [Grafana](https://grafana.com/docs/grafana/latest/) is out of scope for this guide, but do check out their documentation if you get stuck.

# Support

This plugin is actively used in my production environment. Sometimes an update is necessary to support something new, but this plugin is feature complete at this point, so please don't worry if it has not been "updated" for the latest version of Impostor.

I'm open for pull requests, but please communicate with me beforehand in case I'm already working on something similar and to check if your feature fits in with my vision for this plugin.

If you want to support me in creating plugins like this one or want to sponsor a custom feature, please check out my [Github Sponsors page](https://github.com/sponsors/miniduikboot).

