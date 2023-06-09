# Dotnet Telemetry Example

## Example app architecture

The example app has two rest services `WebApp` and `Worker`.
`WebApp` is a rest API that exposes 1 endpoint `POST /orders`.
`Worker` Is also a rest API, but its primary function is to process
messages off of rabbitmq that originate from `WebApp`.

We make use of the following infrastructure

1. ElasticSearch / Kibana for logs
   - We push logs directly to elastic via http for this setup, but using
   fluentd or logstash via kube logs is recommended in production.
   - Logs are also sent to the console in a plain text format.
   - We use Serilog
     - Serilog uses a json format to export logs to elasticsearch (structured logging)
     - It attaches the traceId and spanId from OpenTelemetry to the logs.
2. Grafana to display traces, metrics and logs
   - OpenTelemetry libraries are used to instrument code with trace contexts.
3. Tempo to store traces
4. Prometheus to store metrics
5. RabbitMQ as the message broker for MassTransit

#### Create Order Flow

![Order Flow Diagram](./img/OrderFlow.png)

The create order flow is an intentionally complex workflow that demonstrates
how distributed tracing can help visualize these types of complicated flows.

When `POST /orders` is called the `WebApp` service creates a record in 
its sqlite database. It then sends a message on the message broker to
be processed by `Worker`. Finally, it polls `Worker` to check whether
it has completed processing the order record.

`Worker` listens on the `orders` queue (using MassTransit) for order
creation messages. When it consumes a message it creates its own `OrderState`
record, it then sleeps for 5 seconds and finally updates that record indicating
it has completed processing. Once this process is complete its API will return that
the order has completed processing.

Finally, the WebApp will see the order has completed processing and will
return its response.

### Result

First you can see a the example swagger request / response.

![Swagger Request](./img/example-swagger-request.png)

You can see the generated traceId that is used to find the distributed trace in grafana / tempo.

Then you can see the generated trace.

![Example Trace](./img/example-trace.png)

Finally you can see the generated trace with correlated logs.

![Example Trace with Correlated Logs](./img/example-correlated-logs.png)

### Running

1. Migrate databases
    - `cd WebApp && dotnet ef database update`
    - `cd Worker && dotnet ef database update`
2. Run the elastic stack docker-compose setup `docker-compose up setup`
3. Run docker compose `docker compose up -d`
4. Run the two dotnet services (WebApp & Worker) 
   - `cd WebApp && dotnet run`
   - `cd Worker && dotnet run`
5. Make a request to the `/orders` endpoint on WebApp


